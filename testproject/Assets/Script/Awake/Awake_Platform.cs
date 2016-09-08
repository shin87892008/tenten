using UnityEngine;
using System.Collections;

public class Awake_Platform : Awake_Base
{
    public GameObject i_Agree_Obj;
    public GameObject i_Select_Platform_Obj;

    public override void Init()
    {
    }

    public override void Proccess_Start()
    {
        Platform_Manager.Instance.Platform_Init();
        Debug.Log("Platfrom Login");
        Platform_Type platform_type = (Platform_Type)PlayerPrefs.GetInt("Platform_Type", 0);

        //로그인 기록이 없다, 최초 실행
        if (platform_type == Platform_Type.NONE)
        {
#if UNITY_EDITOR
            platform_type = Platform_Type.FACEBOOK;
#elif UNITY_ANDROID
            platform_type = Platform_Type.AOS_GOOGLEPLUS;
#elif UNITY_IOS
            platform_type = Platform_Type.IOS_GAMECENTER;
#else
            platform_type = Platform_Type.FACEBOOK;
#endif

            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                //약관동의 팝업작업
                i_Agree_Obj.SetActive(true);
                i_Agree_Obj.GetComponent<WebView>().Open_WebView("http://www.naver.com");
                return;
            }
        }
        Platform_Manager.Instance.Platform_SignIn(platform_type, CB_SignIn);
    }
    public void Platform_Agree(int p_index)
    {
        i_Agree_Obj.GetComponent<WebView>().Close_WebView();
        i_Agree_Obj.SetActive(false);
        if (p_index == 0)
        {
#if UNITY_EDITOR
            Platform_Manager.Instance.Platform_SignIn(Platform_Type.FACEBOOK, CB_SignIn);
#elif UNITY_ANDROID
            Platform_Manager.Instance.Platform_SignIn(Platform_Type.AOS_GOOGLEPLUS, CB_SignIn);
#elif UNITY_IOS
            Platform_Manager.Instance.Platform_SignIn(Platform_Type.IOS_GAMECENTER, CB_SignIn);
#else
            Platform_Manager.Instance.Platform_SignIn(Platform_Type.FACEBOOK, CB_SignIn);
#endif
        }
        else
        {
            //게임종료
            Application.Quit();
        }
    }
    void CB_SignIn(bool p_isSignIn, Platform_Data p_userdata)
    {
        if (p_isSignIn)
        {
            Debug.Log(p_userdata.ToString());
            PlayerPrefs.SetInt("Platform_Type", (int)p_userdata.Platform_type);
            PlayerPrefs.SetString("Platform_ID", p_userdata.User_Id);
            is_finish = true;
        }
        else
        {
            i_Select_Platform_Obj.SetActive(true);
        }
    }
    public void Platform_Select_Click(int p_Index)
    {
        i_Select_Platform_Obj.SetActive(false);
        Platform_Type select_platform = (Platform_Type)p_Index;
        Platform_Manager.Instance.Platform_SignIn(select_platform, CB_SignIn);
    }
}
