using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIForm
{
    void OnInit();
    void OnOpen(object userData);
    void OnClose();
}
