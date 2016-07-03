using UnityEngine;
using System.Collections;

public class UI_Main : UIBaseFrame
{
    protected override void Start()
    {
        base.Start();
    }

    public void Btn_Start()
    {
        Application.LoadLevel(1);
    }
}
