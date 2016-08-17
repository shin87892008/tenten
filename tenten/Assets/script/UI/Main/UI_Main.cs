using UnityEngine;
using System.Collections;

public class UI_Main : UIBaseFrame
{
    protected override void Start()
    {
        base.Start();
        SocialManager.Instance.Init();
        DBManager.Instance.Init();
    }

    public void Btn_Start()
    {
        Application.LoadLevel(1);
    }

    public void Btn_Facebook()
    {
        SocialManager.Instance.Login();
    }

}
