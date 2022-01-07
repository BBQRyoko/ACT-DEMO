using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BackpackItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private int m_ItemId;
    public int ItemId { get { return m_ItemId; } }
    private string m_Des;

    private Image m_Icon;
    private Text m_CountContent;
    private Image m_PitchOn;
    private BackpackForm m_Form;
    private Button m_Btn;

    public void OnInit(BackpackForm form)
    {
        m_Icon = transform.Find("ItemIcon").GetComponent<Image>();
        m_CountContent = transform.Find("HeapUpCount").GetComponent<Text>();
        m_PitchOn = transform.Find("PitchOn").GetComponent<Image>();
        m_PitchOn.enabled = false;
        m_Btn = GetComponent<Button>();
        m_Btn.onClick.AddListener(BtnClick);

        m_ItemId = -1;

        m_Form = form;
    }

    private void BtnClick()
    {
        m_Form.OpenOptions(this);
    }

    public void OnOpen(InventoryItemData item)
    {
        gameObject.SetActive(true);
        m_ItemId = item.Source.Id;
        m_Icon.sprite = item.Icon;
        m_Icon.enabled = true;
        if (item.HasHeapUp)
        {
            m_CountContent.text = item.Count.ToString(); ;
            m_CountContent.enabled = true;
        }
        else
        {
            m_CountContent.text = string.Empty;
            m_CountContent.enabled = false;
        }

        m_Des = item.Des;
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        m_Icon.enabled = false;
        m_CountContent.enabled = false;
        m_ItemId = -1;
        m_PitchOn.enabled = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_Form.OnEndDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_Form.OnDrag();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_Form.OnBeginDrag(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_Form.HideDescript();
        m_PitchOn.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Form.ShowDescript(m_Des);
        m_PitchOn.enabled = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        m_Form.OnDrop(this);
    }
}
