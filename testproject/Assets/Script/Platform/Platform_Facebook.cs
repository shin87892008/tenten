using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Platform_Facebook : Base_Platform
{
    [Serializable]
    class FB_Data
    {
        public List<string> permissions;
        public string expiration_timestamp;
        public string access_token;
        public string user_id;
        public string last_refresh;
        public List<string> granted_permissions;
        public List<string> declined_permissions;
        public string callback_id;
    }

    public override void Init()
    {
        SPFacebook.OnInitCompleteAction += OnInit;
        SPFacebook.Instance.Init();
    }
    private void OnInit()
    {
        is_Init = true;
    }
    public override void SignIn(Action<bool, Platform_Data> p_SignIn)
    {
        base.SignIn(p_SignIn);
        this.StartCoroutine(CR_Login());
    }

    private IEnumerator CR_Login()
    {
        if (!is_Init)
        {
            Init();
        }

        while (!is_Init)
            yield return null;

        if (SPFacebook.instance.IsLoggedIn)
        {
            Return_SignIn(SPFacebook.instance.IsLoggedIn);
        }
        else
        {
            SPFacebook.OnAuthCompleteAction += OnAuth;
            SPFacebook.instance.Login();
        }
    }

    private void OnAuth(FB_Result result)
    {
        SPFacebook.OnAuthCompleteAction -= OnAuth;
        if ( !SPFacebook.instance.IsLoggedIn)
        {
            Debug.LogError("Connection failed with code: " + result.Error);
        }
        Return_SignIn(SPFacebook.instance.IsLoggedIn);
    }

    private void Return_SignIn(bool p_isSignIn)
    {
        Debug.Log("Facebook SignIn :" + p_isSignIn.ToString());
        Platform_Data user_data = new Platform_Data();
        if (p_isSignIn)
        {
            user_data.Platform_type = Platform_Type.FACEBOOK;
            user_data.User_Id = SPFacebook.instance.UserId;
            user_data.User_Name = SPFacebook.instance.name;
        }
        CB_SignIn(p_isSignIn, user_data);
    }

    public override void SignOut(Action<bool> p_SignOut)
    {
        base.SignOut(p_SignOut);
    }
}
