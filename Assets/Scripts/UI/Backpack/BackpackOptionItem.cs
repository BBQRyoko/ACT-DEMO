using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackOptionItem : MonoBehaviour
{
    private Button m_Btn;
    private Text m_Text;
    private System.Action<string> m_BtnCallback;

    public void OnInit(System.Action<string> callback)
    {
        m_Btn = transform.Find("Btn").GetComponent<Button>();
        m_Btn.onClick.AddListener(BtnClick);
        m_Text = m_Btn.GetComponentInChildren<Text>();
        m_BtnCallback = callback;
    }

    public void OnOpen(string optionName)
    {
        gameObject.SetActive(true);
        m_Text.text = optionName;
    }

    private void BtnClick()
    {
        m_BtnCallback?.Invoke(m_Text.text);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}
