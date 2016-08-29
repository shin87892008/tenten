using UnityEngine;
using System.Collections;
using System;

public class Platform_Google : Base_Platform
{
    public override void Init()
    {
        //GCM Init
        GoogleCloudMessageService.ActionCouldMessageLoaded += OnMessageLoaded;
        GoogleCloudMessageService.ActionCMDRegistrationResult += HandleActionCMDRegistrationResult;
        GoogleCloudMessageService.Instance.Init();
        is_Init = true;
    }
    public override void SignIn(Action<bool, Platform_Data> p_SignIn)
    {
        base.SignIn(p_SignIn);
        GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
        GooglePlayConnection.Instance.Connect();
    }
    private void ActionConnectionResultReceived(GooglePlayConnectionResult result)
    {
        Debug.Log("google SignIn :" + result.IsSuccess.ToString());
        Platform_Data user_data = new Platform_Data();
        if (result.IsSuccess)
        {
            GooglePlayerTemplate current_player = GooglePlayManager.Instance.player;
            user_data.Platform_type = Platform_Type.AOS_GOOGLEPLUS;
            user_data.User_Id = current_player.playerId;
            user_data.User_Name = current_player.name;
        }
        else
        {
            Debug.LogError("Connection failed with code: " + result.code.ToString());
            
        }
        CB_SignIn(result.IsSuccess, user_data);
        /*
         *에러 코드
        CANCELED = 13,
        DATE_INVALID = 12,
        DEVELOPER_ERROR = 10,
        DRIVE_EXTERNAL_STORAGE_REQUIRED = 1500,
        INTERNAL_ERROR = 8,
        INTERRUPTED = 15,
        INVALID_ACCOUNT = 5,
        LICENSE_CHECK_FAILED = 11,
        NETWORK_ERROR = 7,
        RESOLUTION_REQUIRED = 6,
        SERVICE_DISABLED = 3,
        SERVICE_INVALID = 9,
        SERVICE_MISSING = 1,
        SERVICE_VERSION_UPDATE_REQUIRED = 2,
        SIGN_IN_REQUIRED = 4,
        SUCCESS = 0,
        TIMEOUT = 14
*/
    }
    public override void SignOut(Action<bool> p_SignOut)
    {
        base.SignOut(p_SignOut);
    }



    //+ GCM--------------
    private void HandleActionCMDRegistrationResult(GP_GCM_RegistrationResult res)
    {
        if (res.IsSucceeded)
        {
            Debug.Log("GCM REG ID: " + GoogleCloudMessageService.instance.registrationId);
        }
        else
        {
            Debug.LogError("GCM Registration failed :(");
        }
    }

    public override void Get_Push_Message(Action<string> p_push_callback)
    {
        base.Get_Push_Message(p_push_callback);

        if (string.IsNullOrEmpty(GoogleCloudMessageService.instance.registrationId))
        {
            GoogleCloudMessageService.instance.RgisterDevice();
            return;
        }

        GoogleCloudMessageService.instance.LoadLastMessage();
    }

    private void OnMessageLoaded(string lastMessage)
    {
        Debug.Log("Last GCM Message: " + GoogleCloudMessageService.instance.lastMessage);
        CB_Push(GoogleCloudMessageService.instance.lastMessage);
    }
}

