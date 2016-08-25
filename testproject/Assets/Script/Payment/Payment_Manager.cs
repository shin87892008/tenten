using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Payment_Manager : Singleton<Payment_Manager> {

    private Payment_Base Payment;

    public void Init(List<string> p_product)
    {
#if UNITY_ANDROID
        Payment = this.gameObject.AddComponent<Aos_Payment>();
#elif UNITY_IOS
        Payment = this.gameObject.AddComponent<Ios_Payment>();
#else
#endif
        Payment.Init(p_product);
    }

    public void Get_Product_List(Action<List<Product_Data>> p_callback)
    {
        Payment.Get_Product_List(p_callback);
    }

    public void Get_Purchase_List(Action<List<Purchase_Data>> p_callback)
    {
        Payment.Get_Purchase_List(p_callback);
    }

    public void Purchase(string p_sku, Action<bool, Purchase_Data> p_callback)
    {
        Payment.Purchase(p_sku, p_callback);
    }
    public void Consume(string p_sku, Action p_callback)
    {
        Payment.Consume(p_sku, p_callback);
    }
}
