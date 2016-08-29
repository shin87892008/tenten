using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Platform_Manager : Singleton<Platform_Manager>
{
    private Dictionary<Platform_Type, Base_Platform> Dic_Platform = new Dictionary<Platform_Type, Base_Platform>();

    public void Platform_Init()
    {
        Dic_Platform.Add(Platform_Type.AOS_GOOGLEPLUS, new Platform_Google());
        Dic_Platform.Add(Platform_Type.FACEBOOK, new Platform_Facebook());

        foreach (Base_Platform value in Dic_Platform.Values)
        {
            value.Init();
        }
    }

    public void Platform_SignIn(Platform_Type p_type, Action<bool, Platform_Data> p_CB_SignIn)
    {
        Dic_Platform[p_type].SignIn(p_CB_SignIn);
    }
    public void Get_Push_Message(Platform_Type p_type, Action<string> p_callback)
    {
        Dic_Platform[p_type].Get_Push_Message(p_callback);
    }
}
