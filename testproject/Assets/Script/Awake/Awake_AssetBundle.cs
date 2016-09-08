using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Awake_AssetBundle : Awake_Base
{
    public Awake_Bundle_Loading i_AssetBundle_Panel;

    public override void Init()
    {
        i_AssetBundle_Panel.gameObject.SetActive(true);
    }
    public override void Proccess_Start()
    {
        Debug.Log("Main :AssetBundle");
        AssetBundle_Manager.Instance._ProgressCallBack += i_AssetBundle_Panel.Set_Progress;
        AssetBundle_Manager.Instance._AssetBundle_End_CallBack += AssetBundle_End;
        AssetBundle_Manager.Instance.AssetBundle_Download();
    }
    private void AssetBundle_End()
    {
        AssetBundle_Manager.Instance._ProgressCallBack -= i_AssetBundle_Panel.Set_Progress;
        AssetBundle_Manager.Instance._AssetBundle_End_CallBack -= AssetBundle_End;
        AssetBundle_Manager.Instance.Load_Bundle_Use_Info();

        AssetBundle_Manager.Instance._AssetBundle_End_CallBack += Test;
        AssetBundle_Manager.Instance.Scene_Bundle_Download(3);
    }
    private void Test()
    {
        AssetBundle_Manager.Instance._AssetBundle_End_CallBack -= Test;
        //GameObject obj = Resources_Manager.Load("(Test_Dummy)/MazeEdgefbx1_Prefab.prefab") as GameObject;
        //obj.transform.parent = this.transform;

        i_AssetBundle_Panel.gameObject.SetActive(false);
        is_finish = true;
    }
}

