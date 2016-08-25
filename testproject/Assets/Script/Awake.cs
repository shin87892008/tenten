using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public class Platform_Data
{
    public Platform_Type Platform_type;
    public string User_Id;
    public string User_Name;

    public string ToString()
    {
        return "Platform_type : " + Platform_type.ToString() + "\nUser_Id : " + User_Id + "\nUser_Name : " + User_Name;

    }
}

public enum Platform_Type
{
    NONE,
    KAKAO = 1,
    NAVER_LINE = 2,
    NAVER_BAND = 3,
    FACEBOOK = 4,
    GUEST = 5,
    IOS_GAMECENTER = 6,
    AOS_GOOGLEPLUS = 7,
    NOT_PLATFROM = 99,
}
public class Awake : MonoBehaviour {

    public enum AWAKE_STEP
    {
        SERVER,
        ASSETBUNDLE,
        LOGIN_CHECK,
        PLATFROM,
        LENGTH,
    }

    public GameObject i_Agree_Obj;
    public GameObject i_Select_Platform_Obj;

    private List<Action> Init_Action_List = new List<Action>();
    private bool is_Complete;

    void Start()
    {
        Init_Action_List.Add(InitStep_Get_ServerInfo);
        Init_Action_List.Add(InitStep_AssetBundle);
        Init_Action_List.Add(InitStep_Inapp);
        //Init_Action_List.Add(InitStep_SignIn_Check);
        StartCoroutine(InitStep());
    }

    private IEnumerator InitStep()
    {
        for (int i = 0; i < Init_Action_List.Count; ++i)
        {
            is_Complete = false;

            Init_Action_List[i]();

            while (is_Complete == false)
                yield return null;
        }
        yield break;
    }
    void Finish_Step()
    {
        is_Complete = true;
    }
    void InitStep_Get_ServerInfo()
    {
        Debug.Log("Main : InitStep_Get_ServerInfo");
        Did_InitStep0_Get_ServerInfo();
        //JWWW.Request_ServerInfo(Did_InitStep0_Get_ServerInfo);
    }
    void Did_InitStep0_Get_ServerInfo()
    {
        Finish_Step();
    }
    void InitStep_AssetBundle()
    {
        Debug.Log("Main :AssetBundle");
        Finish_Step();
    }

    void InitStep_SignIn_Check()
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
        if(p_isSignIn)
        {
            Debug.Log(p_userdata.ToString());
            //PlayerPrefs.SetInt("Platform_Type", (int)p_userdata.Platform_type);
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








    //test
    public void InitStep_Inapp()
    {
        List<string> __product = new List<string>();
        __product.Add("goddess_item_1");
        __product.Add("goddess_item_2");
        Payment_Manager.Instance.Init(__product);
    }
    public void Product_List()
    {
        Payment_Manager.Instance.Get_Product_List(CB_Product_List);
    }
    public void CB_Product_List(List<Product_Data> p_data)
    {
        for (int i = 0; i < p_data.Count; ++i)
        {
            p_data[i].ToString();
        }
    }

    public void Purchase_List()
    {
        Payment_Manager.Instance.Get_Purchase_List(CB_Purchase_List);
    }
    public void CB_Purchase_List(List<Purchase_Data> p_data)
    {
        for(int i = 0; i < p_data.Count; ++i)
        {
            p_data[i].ToString();
        }
    }
    public void Purchase(string p_sku)
    {
        Payment_Manager.Instance.Purchase(p_sku, CB_Purchase);
    }
    public void CB_Purchase(bool is_Success, Purchase_Data p_purchase)
    {
        if(is_Success)
        {
            Debug.Log("Purchase is : " + is_Success.ToString() + "\nInfo : " + p_purchase.ToString());
        }
        else
        {
            Debug.Log("Purchase is : " + is_Success.ToString());
        }
    }
    public void Consume(string p_sku)
    {
        Payment_Manager.Instance.Consume(p_sku, CB_Consume);
    }
    public void CB_Consume()
    {

    }
}
