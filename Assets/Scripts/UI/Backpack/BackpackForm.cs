using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// 背包界面UI和逻辑
/// </summary>
public class BackpackForm : MonoBehaviour, IUIForm
{

    private BackpackItem[] m_AllItems;
    private Button m_CloseBtn;
    private Text m_DesContent;
    private Image m_DragIcon;
    private CanvasGroup m_CG;
    private Button m_OptionGroup;
    private RectTransform m_OptionsParent;
    private BackpackOptionItem m_OptionItemTemplate;

    private Dictionary<int, InventoryItemData> m_DataItemDict;
    private IList<InventoryItemData> m_DataSources;
    private BackpackItem m_DragItem;
    private List<BackpackOptionItem> m_AllOptionItems;

    private static Dictionary<int, string[]> m_OptionDict = new Dictionary<int, string[]>()
    {
        {0, new string[]{"使用", "检查", "丢弃"} },
        {1, new string[]{"检查", "丢弃"} }
    };

    private void Awake()
    {
        OnInit();
        UIManager.RegisterUIForm("BackpackForm", this);

    }

    public void OnInit()
    {
        m_AllItems = GetComponentsInChildren<BackpackItem>();
        foreach (var item in m_AllItems)
        {
            item.OnInit(this);
        }
        m_CloseBtn = transform.Find("CloseBtn").GetComponent<Button>();
        m_CloseBtn.onClick.AddListener(OnClose);
        m_DesContent = transform.Find("Right/Text").GetComponent<Text>();
        m_DragIcon = transform.Find("DragParent/DragIcon").GetComponent<Image>();
        m_DragIcon.transform.position = new Vector3(9999, 9999, 9999);
        m_CG = GetComponent<CanvasGroup>();
        m_OptionGroup = transform.Find("Options").GetComponent<Button>();
        m_OptionGroup.onClick.AddListener(CloseOptions);
        m_OptionsParent = transform.Find("Options/Background").GetComponent<RectTransform>();
        m_OptionItemTemplate = transform.Find("Options/OptionItemsTemplate").GetComponent<BackpackOptionItem>();

        m_DataItemDict = new Dictionary<int, InventoryItemData>();
        m_AllOptionItems = new List<BackpackOptionItem>();
    }

    public void OnOpen(object userData)
    {
        m_CG.alpha = 1;
        m_CG.interactable = true;
        m_CG.blocksRaycasts = true;
        //gameObject.SetActive(true);
        m_DataItemDict.Clear();
        m_DataSources = (IList<InventoryItemData>)userData;
        foreach (var item in m_DataSources)
        {
            m_DataItemDict.Add(item.Source.Id, item);
        }
        RefreshShow();
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    private void RefreshShow()
    {
        //todo,暂时未处理无限滚动问题，之后再说
        for (int i = 0; i < m_DataSources.Count; i++)
        {
            m_AllItems[i].OnOpen(m_DataSources[i]);
        }
        for (int i = m_DataSources.Count; i < m_AllItems.Length; i++)
        {
            m_AllItems[i].OnClose();
        }
    }

    public void ShowDescript(string text)
    {
        m_DesContent.enabled = true;
        m_DesContent.text = text;
    }

    public void HideDescript()
    {
        m_DesContent.enabled = false;
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="item"></param>
    public GameObject OnBeginDrag(BackpackItem item)
    {
        m_DragItem = item;
        var itemData = m_DataItemDict[item.ItemId];
        m_DragIcon.sprite = itemData.Icon;
        return m_DragIcon.gameObject;
    }

    /// <summary>
    /// 拖拽中鼠标跟随
    /// </summary>
    public void OnDrag()
    {
        m_DragIcon.transform.position = Mouse.current.position.ReadValue();
    }

    /// <summary>
    /// 拖拽到某个物体上
    /// </summary>
    /// <param name="item"></param>
    public void OnDrop(BackpackItem item)
    {
        //如果是他自己
        if (item == m_DragItem) return;
        //如果有物体，进行交换
        var dragData = m_DataItemDict[m_DragItem.ItemId];
        var dropData = m_DataItemDict[item.ItemId];
        m_DragItem.OnOpen(dropData);
        item.OnOpen(dragData);
        //原始数据交换位置，就这样先做着
        int dragIndex = m_DataSources.IndexOf(dragData);
        int dropIndex = m_DataSources.IndexOf(dropData);
        m_DataSources.Remove(dragData);
        m_DataSources.Insert(dragIndex, dragData);
        m_DataSources.Remove(dropData);
        m_DataSources.Insert(dropIndex, dropData);
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    public void OnEndDrag()
    {
        m_DragIcon.transform.position = new Vector3(9999, 9999, 9999);
        m_DragItem = null;
    }

    public void OpenOptions(BackpackItem item)
    {
        var itemData = m_DataItemDict[item.ItemId];
        if (m_OptionDict.TryGetValue(itemData.ExecutableOperation, out var names))
        {
            m_OptionGroup.gameObject.SetActive(true);
            UnityUtil.AnchorTargetByScreen(m_OptionsParent, item.GetComponent<RectTransform>());
            for (int i = m_AllOptionItems.Count; i < names.Length; i++)
            {
                m_AllOptionItems.Add(CreateOptionItem());
            }
            for (int i = 0; i < names.Length; i++)
            {
                m_AllOptionItems[i].OnOpen(names[i]);
            }
            for (int i = names.Length; i < m_AllOptionItems.Count; i++)
            {
                m_AllOptionItems[i].OnClose();
            }
        }
        else
        {
            Debug.LogError($"找不到Option = {itemData.ExecutableOperation}的选项。物品Id = {item.ItemId}");
        }
    }

    private BackpackOptionItem CreateOptionItem()
    {
        var res = Instantiate(m_OptionItemTemplate, m_OptionsParent);
        res.OnInit(OnOptionBtnClick);
        return res;
    }

    private void OnOptionBtnClick(string optionName)
    {
        Debug.Log($"选项---->{optionName}被点击了。");
        CloseOptions();
    }

    private void CloseOptions()
    {
        m_OptionGroup.gameObject.SetActive(false);
    }


    public void OnClose()
    {
        m_CG.alpha = 0;
        m_CG.interactable = false;
        m_CG.blocksRaycasts = false;
        //gameObject.SetActive(false);
        foreach (var item in m_AllItems)
        {
            item.OnClose();
        }
        m_DesContent.text = string.Empty;
        m_DragIcon.transform.position = new Vector3(9999, 9999, 9999);
    }
}
