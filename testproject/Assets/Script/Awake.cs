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
    public Platform_Data()
    {

    }
    public Platform_Data(Platform_Type p_type, string p_id)
    {
        Platform_type = p_type;
        User_Id = p_id;
    }
}

public class Awake : MonoBehaviour {
    
    public GameObject i_Agree_Obj;
    public GameObject i_Select_Platform_Obj;

    private List<Awake_Base> Awake_Work_List = new List<Awake_Base>();
    private bool is_Complete;

    void Start()
    {
        Awake_Work_List.Add(this.GetComponent<Awake_ServerInfo>());
        Awake_Work_List.Add(this.GetComponent<Awake_AssetBundle>());
        Awake_Work_List.Add(this.GetComponent<Awake_Platform>());
        StartCoroutine(InitStep());
    }

    private IEnumerator InitStep()
    {
        for (int i = 0; i < Awake_Work_List.Count; ++i)
        {
            Awake_Work_List[i].Init();
            Awake_Work_List[i].Proccess_Start();

            while (false == Awake_Work_List[i].is_finish)
                yield return null;
        }
        yield break;
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
    

    public void GCM_Get_LastMassage()
    {
#if UNITY_ANDROID
        Platform_Manager.Instance.Get_Push_Message(Platform_Type.AOS_GOOGLEPLUS, CB_GCM_Message);
#elif UNITY_IOS
        Platform_Manager.Instance.Get_Push_Message(Platform_Type.IOS_GAMECENTER, CB_GCM_Message);
#else
#endif
    }
    public void CB_GCM_Message(string p_message)
    {
        Debug.LogError(p_message);
    }
}
