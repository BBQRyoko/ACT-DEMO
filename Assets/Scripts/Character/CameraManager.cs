using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
    Transform targetTransform; //需要跟随的目标(玩家)
    public Transform cameraPivotTransform; //相机pivot
    Transform cameraTransform; //相机object的位置
    public LayerMask ignoreLayers; //除了选定的层外都可以穿透
    float defaultPosition; //相机的初始Z点
    Vector3 cameraFollowVelocity = Vector3.zero; //ref
    Vector3 cameraVectorPosition;

    public static CameraManager singleton;

    public float cameraCollisionRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;
    public float cameraFollowSpeed = 0.1f;
    public float cameraLookSpeed = 0.1f;
    public float cameraPivotSpeed = 0.08f;

    float lookAngle; //视角左右
    float pivotAngle; //视角上下
    public float minPivotAngle = -35; 
    public float maxPivotAngle = 35;

    //锁定系统
    public Transform currentLockOnTarget;
    public Image lockOnPrefab;
    Image lockOnMark;

    //相机前方的有效单位
    public List<CharacterManager> availableTarget = new List<CharacterManager>();
    public Transform nearestLockOnTarget;
    public Transform leftLockTarget;
    public Transform rightLockTarget;
    public float maxLockOnDistance = 30;
    public bool isLockOn; 

    private void Awake()
    {
        singleton = this;
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        lockOnMark = Instantiate(lockOnPrefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
        //ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void HandleAllCameraMovement() 
    {
        float delta = Time.fixedDeltaTime;
        FollowTarget(delta);
        RotateCamera(delta);
        HandleCameraCollisions(delta);
        HandleLockOnMark();
    }

    public void FollowTarget(float delta)  //相机跟随
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, delta / cameraFollowSpeed);
        transform.position = targetPosition;
    }

    public void RotateCamera(float delta) //相机转动
    {
        //if (!inputManager.lockOn_Flag )
        //{
        if (!inputManager.baGua_Input) 
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
        //}
        //else
        //{
        //    float velocity = 0;

        //    Vector3 dir = currentLockOnTarget.position - transform.position;
        //    dir.Normalize();
        //    dir.y = 0;

        //    Quaternion targetRotation = Quaternion.LookRotation(dir);
        //    transform.rotation = targetRotation;

        //    dir = currentLockOnTarget.position - cameraPivotTransform.position;
        //    dir.Normalize();

        //    targetRotation = Quaternion.LookRotation(dir);
        //    Vector3 eulerAngle = targetRotation.eulerAngles;
        //    eulerAngle.y = 0;
        //    cameraPivotTransform.localEulerAngles = eulerAngle;
        //}
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
    } //相机与非指定的物件碰撞时的拉近功能

    public void HandleLockOn() //相机锁定
    {
        float shortestDistance = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

        for (int i = 0; i < colliders.Length; i++) 
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();

            if (character != null) 
            {
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                if (character.transform.root != targetTransform.transform.root && viewableAngle > -50 && viewableAngle < 50 && distanceFromTarget <= maxLockOnDistance) 
                {
                    availableTarget.Add(character);
                }
            }
        }

        for (int k = 0; k < availableTarget.Count; k++) 
        {
            float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTarget[k].transform.position);
            float shortestDistanceOfLeftTarget = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            if (distanceFromTarget < shortestDistance) 
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = availableTarget[k].lockOnTransform;
            }

            if (inputManager.lockOn_Flag) 
            {
                Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTarget[k].transform.position);
                var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTarget[k].transform.position.x;
                var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTarget[k].transform.position.x;

                if (relativeEnemyPosition.x > 0.00 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                    leftLockTarget = availableTarget[k].lockOnTransform;
                }
                
                if (relativeEnemyPosition.x<0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget) 
                {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;
                    rightLockTarget = availableTarget[k].lockOnTransform;
                }
            }
        }
    }
    public void HandleLockOnMark() 
    {
        if (!currentLockOnTarget)
        {
            lockOnMark.gameObject.SetActive(false);
        }
        else 
        {
            lockOnMark.gameObject.SetActive(true);
            lockOnMark.transform.position = Camera.main.WorldToScreenPoint(new Vector3(currentLockOnTarget.position.x, currentLockOnTarget.position.y + 1, currentLockOnTarget.position.z));
        }
    }
    public void ClearLockOnTargets() 
    {
        availableTarget.Clear();
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }
}
