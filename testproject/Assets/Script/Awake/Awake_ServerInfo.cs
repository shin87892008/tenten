using UnityEngine;
using System.Collections;

public class Awake_ServerInfo : Awake_Base
{
    
    public override void Init()
    {

    }
    public override void Proccess_Start()
    {
        Debug.Log("Main : InitStep_Get_ServerInfo");
        CB_ServerInfo();
    }

    private void CB_ServerInfo()
    {
        is_finish = true;
    }
}
