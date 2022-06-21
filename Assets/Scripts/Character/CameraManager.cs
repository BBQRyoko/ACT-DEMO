using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    PlayerManager playerManager;
    InputManager inputManager;
    public Transform targetTransform; //需要跟随的目标(玩家)
    public Transform targetTransformWhileAiming; //弓箭瞄准时
    [SerializeField] Canvas mainCanvas;
    public Transform cameraPivotTransform; //相机pivot
    public Transform cameraTransform; //相机object的位置
    public LayerMask ignoreLayers; //除了选定的层外都可以穿透
    float defaultPosition; //相机的初始Z点
    Vector3 cameraFollowVelocity = Vector3.zero; //ref
    [SerializeField]Vector3 cameraVectorPosition;

    public static CameraManager singleton;

    public float cameraCollisionRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;

    public float cameraFollowSpeed = 0.1f;
    public float cameraAimingLookSpeed = 5f;
    public float cameraLookSpeed = 0.1f;
    public float cameraAimingPivotSpeed = 5f;
    public float cameraPivotSpeed = 0.08f;

    float lookAngle; //视角左右
    float pivotAngle; //视角上下
    public float minPivotAngle = -25;
    public float maxPivotAngle = 25;
    public float minPivotAngleWhileLockOn = -10;
    public float maxPivotAngleWhileLockOn = 5;

    //八卦相关
    public bool cameraLock;

    //敌人血条
    [SerializeField] Transform enemyUIs;
    public EnemyFillingUI enemyHealthUI;

    //警觉系统
    public EnemyFillingUI enemyFillingUI;

    //互动提示
    public InteractPromptUI interactPrompt;
    public Transform interactTransforms;

    //锁定系统
    public LayerMask visionBlockLayer;
    public Transform currentLockOnTarget;
    public Image lockOnPrefab;
    Image lockOnMark;
    [SerializeField] float maxLockingResetDistance = 20f;
    float lockOnResetTimer;

    //瞄准相关
    [SerializeField] Transform cameraAimingForward;

    //处决系统
    public Transform curExuectionTarget;
    public Image executePrefab;
    Image executeMark;

    //预警信号
    public Image dangerMark_Prefab;
    Image dangerMark;
    EnemyManager curEnemy;

    //相机前方的有效单位
    public List<CharacterManager> availableTarget = new List<CharacterManager>();
    public Transform nearestLockOnTarget;
    public Transform leftLockTarget;
    public Transform rightLockTarget;
    public float maxLockOnDistance = 18f;
    public bool isLockOn;

    private void Awake()
    {
        singleton = this;
        playerManager = FindObjectOfType<PlayerManager>();
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        lockOnMark = Instantiate(lockOnPrefab, enemyUIs.transform).GetComponent<Image>();
        executeMark = Instantiate(executePrefab, enemyUIs.transform).GetComponent<Image>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
        //ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }
    public void HandleAllCameraMovement()
    {
        float delta = Time.fixedDeltaTime;
        FollowTarget(delta);
        CameraReset();
        HandleCameraRotation(delta);
        HandleCameraCollisions(delta);
        HandleLockOnMark();
        HandleExecutingMark();
        LockOnDistanceChecking();

        if (currentLockOnTarget)
        {
            if (currentLockOnTarget.GetComponentInParent<EnemyStats>().currHealth <= 0) 
            {
                inputManager.lockOn_Flag = false;
                currentLockOnTarget = null;
            }
        }
        else 
        {
            inputManager.lockOn_Flag = false;
            currentLockOnTarget = null;
        }

        if (dangerMark != null)
        {
            dangerMark.transform.position = Camera.main.WorldToScreenPoint(new Vector3(curEnemy.transform.position.x, curEnemy.transform.position.y + 1.5f, curEnemy.transform.position.z));
        }
    }
    public void FollowTarget(float delta)  //相机跟随
    {
        if (playerManager.isAiming)
        {
            Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransformWhileAiming.position, ref cameraFollowVelocity, delta / (cameraFollowSpeed/5));
            transform.position = targetPosition;
            inputManager.lockOn_Input = false;
            inputManager.lockOn_Flag = false;
            ClearLockOnTargets();
        }
        else 
        {
            Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, delta / cameraFollowSpeed);
            transform.position = targetPosition;
        }
    }
    public void HandleCameraRotation(float delta) 
    {
        if (playerManager.gameStart || playerManager.gameComplete) return;
        if (!playerManager.isAiming)
        {
            RotateCamera(delta);
        }
        else 
        {
            RotateAimingCamera();
        }
    }
    public void RotateCamera(float delta) //相机转动
    {
        if (!inputManager.lockOn_Flag)
        {
            if (!cameraLock)
            {
                lookAngle += (inputManager.cameraInputX * cameraLookSpeed) / delta;
                pivotAngle -= (inputManager.cameraInputY * cameraPivotSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta / cameraLookSpeed);

                rotation = Vector3.zero;
                rotation.x = pivotAngle;

                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, targetRotation, delta / cameraPivotSpeed);
            }
        }
        else
        {
            if (currentLockOnTarget) 
            {
                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                //dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta / (cameraLookSpeed * 2));

                dir = currentLockOnTarget.position - cameraPivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                eulerAngle.x = Mathf.Clamp(eulerAngle.x, minPivotAngleWhileLockOn, maxPivotAngleWhileLockOn);
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }
    }
    public void RotateAimingCamera() 
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        cameraPivotTransform.rotation = Quaternion.Euler(0, 0, 0);

        Quaternion targetRotationX;
        Quaternion targetRotationY;

        Vector3 cameraRotationX = Vector3.zero;
        Vector3 cameraRotationY = Vector3.zero;

        lookAngle += ((inputManager.cameraInputX * cameraAimingLookSpeed)) * Time.deltaTime;
        pivotAngle -= ((inputManager.cameraInputY * cameraAimingPivotSpeed)) * Time.deltaTime;

        cameraRotationY.y = lookAngle;
        targetRotationY = Quaternion.Euler(cameraRotationY);
        targetRotationY = Quaternion.Slerp(transform.rotation, targetRotationY, 1);
        transform.localRotation = targetRotationY;

        cameraRotationX.x = pivotAngle;
        targetRotationX = Quaternion.Euler(cameraRotationX);
        targetRotationX = Quaternion.Slerp(cameraTransform.localRotation, targetRotationX, 1);
        cameraTransform.localRotation = targetRotationX;

    }
    public void ResetAimingCameraRotation() 
    {
        Quaternion targetResetRotation;
        Vector3 resetRotation = Vector3.zero;

        resetRotation.x = 0;
        targetResetRotation = Quaternion.Euler(resetRotation);
        targetResetRotation = Quaternion.Slerp(transform.rotation, targetResetRotation, 1);
        cameraTransform.localRotation = targetResetRotation;
    }
    private void CameraReset()
    {
        if (cameraLock && !inputManager.baGua_Input) 
        {
            if (inputManager.cameraInputX <= 0.1 && inputManager.cameraInputY <= 0.1) 
            {
                cameraLock = false;
            }
        }
    }
    private void HandleCameraCollisions(float delta)
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minCollisionOffset)
        {
            targetPosition = -minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    } //相机与非指定的物件碰撞时的拉近
    public void HandleLockOn() //相机锁定
    {
        availableTarget.Clear();
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;
        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                if (character != null)
                {
                    Vector3 lockTargetDir = character.transform.position - transform.position;
                    float lockTargetDis = Vector3.Distance(transform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDir, transform.forward);

                    if (character.transform.root != targetTransform.transform.root && viewableAngle > -50 && viewableAngle < 50 && lockTargetDis <= maxLockOnDistance)
                    {
                        PlayerStats playerStats = inputManager.GetComponent<PlayerStats>();
                        EnemyStats enemyStats = character.GetComponent<EnemyStats>();
                        Vector3 targetDir = new Vector3(enemyStats.eyePos.position.x - playerStats.eyePos.transform.position.x, enemyStats.eyePos.position.y - playerStats.eyePos.transform.position.y, enemyStats.eyePos.position.z - playerStats.eyePos.transform.position.z);
                        float distance = Vector3.Distance(playerStats.eyePos.transform.position, enemyStats.eyePos.position);
                        bool hitInfo = Physics.Raycast(playerStats.eyePos.position, targetDir, distance, visionBlockLayer);
                        if (!hitInfo)
                        {
                            availableTarget.Add(character);
                        }
                    }
                }
            }      
        }
        if (availableTarget.Count > 0)
        {
            for (int k = 0; k < availableTarget.Count; k++)
            {
                float distFromTarget = Vector3.Distance(transform.position, availableTarget[k].transform.position);

                if (distFromTarget < shortestDistance)
                {
                    shortestDistance = distFromTarget;
                    nearestLockOnTarget = availableTarget[k].lockOnTransform;
                }

                if (inputManager.lockOn_Flag)
                {
                    //This is the big change, from enemy position to player
                    Vector3 relativePlayerPostion = transform.InverseTransformPoint(availableTarget[k].transform.position);

                    var distanceFromLeftTarget = 1000f; //_currentLockOnTarget.transform.position.x - _availableTargets[k].transform.position.x;
                    var distanceFromRightTarget = 1000f; //_currentLockOnTarget.transform.position.x + _availableTargets[k].transform.position.x;

                    if (relativePlayerPostion.x < 0.00)
                    {
                        distanceFromLeftTarget = Vector3.Distance(currentLockOnTarget.position, availableTarget[k].transform.position);
                    }
                    else if (relativePlayerPostion.x > 0.00)
                    {
                        distanceFromRightTarget = Vector3.Distance(currentLockOnTarget.position, availableTarget[k].transform.position);
                    }

                    if (relativePlayerPostion.x < 0.00 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTarget[k].lockOnTransform;
                    }

                    if (relativePlayerPostion.x > 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = availableTarget[k].lockOnTransform;
                    }
                }
            }
        }
    }
    public void HandleLockOnMark()
    {
        if (!currentLockOnTarget)
        {
            if (playerManager.isAiming)
            {
                inputManager.GetComponent<PlayerManager>().shooting_Target = cameraAimingForward;
            }
            else 
            {
                lockOnMark.gameObject.SetActive(false);
                inputManager.GetComponent<PlayerManager>().shooting_Target = inputManager.GetComponent<PlayerManager>().straightLineNullTarget;
            }
        }
        else 
        {
            EnemyManager enemyManager = currentLockOnTarget.gameObject.GetComponentInParent<EnemyManager>();
            lockOnMark.gameObject.SetActive(true);
            lockOnMark.transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemyManager.targetMarkTransform.position.x, enemyManager.targetMarkTransform.position.y, enemyManager.targetMarkTransform.position.z));
            if (playerManager.isAiming)
            {
                inputManager.GetComponent<PlayerManager>().shooting_Target = cameraAimingForward;
            }
            else 
            {
                inputManager.GetComponent<PlayerManager>().shooting_Target = enemyManager.targetMarkTransform;
            }
        }
    }
    public void HandleExecutingMark()
    {
        if (!inputManager.GetComponent<PlayerManager>().isDead) 
        {
            if (!curExuectionTarget)
            {
                executeMark.gameObject.SetActive(false);
            }
            else
            {
                if (curExuectionTarget.GetComponentInParent<EnemyManager>().canBeExecuted)
                {
                    EnemyManager enemyManager = curExuectionTarget.GetComponentInParent<EnemyManager>();
                    executeMark.gameObject.SetActive(true);
                    executeMark.transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemyManager.targetMarkTransform.position.x, enemyManager.targetMarkTransform.position.y, enemyManager.targetMarkTransform.position.z));
                }
                else
                {
                    curExuectionTarget = null;
                }
            }
        }
    }
    public void GenerateAlertIcon(EnemyManager enemyManager)
    {
        var aleartIcon = Instantiate(enemyFillingUI, enemyUIs);
        aleartIcon.SetEnemyManager(enemyManager);
    }
    public void GenerateUIBar(EnemyManager enemyManager) 
    {
        if (!enemyManager.isBoss) 
        {
            var healthBar = Instantiate(enemyHealthUI, enemyUIs);
            healthBar.SetEnemyManager(enemyManager);
        }
    }
    public void GenerateInteractPrompt(InteractSystem interact, string text = "Interact")
    {
        var promptUp = Instantiate(interactPrompt, interactTransforms);
        promptUp.SetInteractObject(interact, text);
    }
    public void ClearLockOnTargets() 
    {
        availableTarget.Clear();
        lockOnMark.gameObject.SetActive(false);
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }
    public void LockOnDistanceChecking() 
    {
        if (currentLockOnTarget) 
        {
            float distanceFromTarget = Vector3.Distance(currentLockOnTarget.transform.position, targetTransform.transform.position);
            if (distanceFromTarget > maxLockingResetDistance)
            {
                if (lockOnResetTimer < 1f)
                {
                    lockOnResetTimer += Time.deltaTime;
                }
                else
                {
                    lockOnResetTimer = 0;
                    inputManager.lockOn_Flag = false;
                    availableTarget.Clear();
                    ClearLockOnTargets();
                }
            }
        }
    }
    public void DangerWarning(EnemyManager enemyManager) 
    {
        dangerMark = Instantiate(dangerMark_Prefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        curEnemy = enemyManager;
        Destroy(dangerMark.gameObject, 1f);
    }
}
