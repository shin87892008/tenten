using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Aos_Payment : Payment_Base
{
    public override void Init(List<string> p_product)
    {
        for(int i = 0; i < p_product.Count; ++i)
        {
            AndroidInAppPurchaseManager.Client.AddProduct(p_product[i]);
        }

        AndroidNativeSettings.Instance.base64EncodedPublicKey = ConstValue.String.Payment_Key;
        AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;
        AndroidInAppPurchaseManager.ActionProductConsumed += OnProductConsumed;

        AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
        AndroidInAppPurchaseManager.Client.Connect(ConstValue.String.Payment_Key);
    }

    private void OnBillingConnected(BillingResult result)
    {
        AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;

        if( result.IsSuccess)
        {
            Debug.Log("OnBillingConnected : True");
            Is_Init = true;
            
        }
        else
        {
            Debug.Log("OnBillingConnected : False");
        }
    }


    //*Product
    
    public override void Get_Product_List(Action<List<Product_Data>> p_callback)
    {
        base.Get_Product_List(p_callback);
        AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
        AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
    }

    private void OnRetrieveProductsFinised(BillingResult result)
    {
        AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;

        if (result.IsSuccess)
        {
            List<Product_Data> __Inventory = new List<Product_Data>();
            foreach (GoogleProductTemplate tpl in AndroidInAppPurchaseManager.Client.Inventory.Products)
            {
                Product_Data __data = new Product_Data();
                __data.Title = tpl.Title;
                __data.Price = tpl.LocalizedPrice;
                __Inventory.Add(__data);
            }
            CB_Product_List(__Inventory);
        }
        else
        {
            Debug.LogError("Connection Response\n" + result.Response.ToString() + " " + result.Message);
        }
    }


    //+Purchase

    public override void Get_Purchase_List(Action<List<Purchase_Data>> p_callback)
    {
        base.Get_Purchase_List(p_callback);
        AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
        AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
    }

    private void OnRetrievePurchaseFinised(BillingResult result)
    {
        AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
        
        if (result.IsSuccess)
        {
            List<Purchase_Data> __Inventory = new List<Purchase_Data>();
            foreach (GooglePurchaseTemplate tpl in AndroidInAppPurchaseManager.Client.Inventory.Purchases)
            {
                Purchase_Data __data = new Purchase_Data();
                __data.SKU = tpl.SKU;
                __data.Token = tpl.Token;
                __Inventory.Add(__data);
            }
            CB_Purchase_List(__Inventory);
        }
        else
        {
            Debug.LogError("Connection Response\n" + result.Response.ToString() + " " + result.Message);
        }
    }
    public override void Purchase(string p_sku, Action<bool, Purchase_Data> p_callback)
    {
        base.Purchase(p_sku, p_callback);
        Debug.Log("Purchase   " + p_sku);
        AndroidInAppPurchaseManager.Client.Purchase(p_sku);
    }
    private void OnProductPurchased(BillingResult result)
    {
        if(result.IsSuccess)
        {
            Purchase_Data __data = new Purchase_Data();
            __data.SKU = result.Purchase.SKU;
            __data.Token = result.Purchase.Token;
            CB_Purchase(result.IsSuccess, __data);
        }
        else
        {
            Debug.LogError(result.Response.ToString() + " " + result.Message);
            CB_Purchase(result.IsSuccess, null);
            //if( result.Response == 7)
            //{
            //}
        }
    }


    //+Consume
    public override void Consume(string p_sku, Action p_callback)
    {
        base.Consume(p_sku, p_callback);
        AndroidInAppPurchaseManager.Client.Consume(p_sku);
    }

    private void OnProductConsumed(BillingResult result)
    {
        CB_Consume();
    }
}
