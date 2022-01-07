using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIManager
{
    private static Dictionary<string, IUIForm> m_Dict;
    private static Dictionary<string, bool> m_Flags;

    static UIManager()
    {
        m_Dict = new Dictionary<string, IUIForm>();
        m_Flags = new Dictionary<string, bool>();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += (i) =>
        {
            if (i == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                m_Dict.Clear();
                m_Flags.Clear();
            }
        };
#endif
    }

    public static void RegisterUIForm(string formName, IUIForm form)
    {
        //这里目前不知道什么问题，在运行和编辑器时，静态类不重置
        if (m_Dict.ContainsKey(formName))
        {
            return;
        }
        m_Dict.Add(formName, form);
        m_Flags.Add(formName, false);
        form.OnClose();
    }

    public static void OpenOrCloseUIForm(string formName, object userData)
    {
        //暂时就先这么做
        if (m_Dict.TryGetValue(formName, out var form))
        {
            if (m_Flags[formName])
            {
                form.OnClose();
                Debug.Log("321");
            }
            else
            {
                form.OnOpen(userData);
                Debug.Log("123");
            }
            m_Flags[formName] = !m_Flags[formName];
        }
        else
        {
            Debug.LogError($"界面>>{formName}<<未注册");
        }
    }
}
