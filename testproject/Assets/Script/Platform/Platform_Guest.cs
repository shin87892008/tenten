using UnityEngine;
using System.Collections;
using System;

public class Platform_Guest : Base_Platform
{ 
    public override void Init()
    {
        is_Init = true;
    }
    public override void SignIn(Action<bool, Platform_Data> p_SignIn)
    {
        base.SignIn(p_SignIn);

        Platform_Data __data = new Platform_Data();
        __data.Platform_type = Platform_Type.GUEST;
        __data.User_Id = PlayerPrefs.GetString("Guest_UUID", System.Guid.NewGuid().ToString());
        CB_SignIn(true,    __data);
    }
}

