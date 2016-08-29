using UnityEngine;
using System.Collections;
using System;

public abstract class Base_Platform : MonoBehaviour {

    protected bool is_Init = false;

    public virtual void Init()
    {
    }

    public virtual void SignIn(Action<bool, Platform_Data> p_SignIn)
    {
        CB_SignIn = p_SignIn;
    }
    protected Action<bool, Platform_Data> CB_SignIn;

    public virtual void SignOut(Action<bool> p_SignOut)
    {
        CB_SignOut = p_SignOut;
    }
    protected Action<bool> CB_SignOut;


    protected Action<string> CB_Push;
    public virtual void Get_Push_Message(Action<string> p_push_callback)
    {
        CB_Push = p_push_callback;
    }
}
