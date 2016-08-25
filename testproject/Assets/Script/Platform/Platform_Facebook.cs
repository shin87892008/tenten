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
        SPFacebook.OnAuthCompleteAction += OnAuth;
        SPFacebook.instance.Login("public_profile", "email", "user_friends");
        is_Init = true;
    }
    public override void SignIn(Action<bool, Platform_Data> p_SignIn)
    {
        base.SignIn(p_SignIn);
        if( !is_Init)
        {
            Init();
        }
        else
        {
            SPFacebook.instance.Login();
        }
    }
    private void OnAuth(FB_Result result)
    {
        Debug.Log("Facebook SignIn :" + SPFacebook.instance.IsLoggedIn.ToString());
        Platform_Data user_data = new Platform_Data();
        if ( SPFacebook.instance.IsLoggedIn)
        {
            var FB_data = JsonUtility.FromJson<FB_Data>(result.RawData);
            user_data.Platform_type = Platform_Type.FACEBOOK;
            user_data.User_Id = FB_data.user_id;
            user_data.User_Name = FB_data.access_token;
        }
        else
        {
            Debug.LogError("Connection failed with code: " + result.Error);
        }
        CB_SignIn(SPFacebook.instance.IsLoggedIn, user_data);
    }
    public override void SignOut(Action<bool> p_SignOut)
    {
        base.SignOut(p_SignOut);
    }
}
