using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Purchase_Data
{
    public string SKU;
    public string Token;

    public string ToString()
    {
        return "SKU : " + SKU + "\nToken : " + Token;
    }
}


public class Product_Data
{
    public string Title;
    public string Price;
    public string ToString()
    {
        return "Title : " + Title + "\nPrice : " + Price;
    }
}

public abstract class Payment_Base : MonoBehaviour {

    protected bool Is_Init = false;
    
    public abstract void Init(List<string> p_product);

    protected Action<List<Product_Data>> CB_Product_List;
    public virtual void Get_Product_List(Action<List<Product_Data>> p_callback)
    {
        CB_Product_List = p_callback;
    }

    protected Action<List<Purchase_Data>> CB_Purchase_List;
    public virtual void Get_Purchase_List(Action<List<Purchase_Data>> p_callback)
    {
        CB_Purchase_List = p_callback;
    }

    protected Action<bool, Purchase_Data> CB_Purchase;
    protected Action CB_Consume;
    public virtual void Purchase(string p_sku, Action<bool, Purchase_Data> p_callback)
    {
        CB_Purchase = p_callback;
    }
    public virtual void Consume(string p_sku, Action p_callback)
    {
        CB_Consume = p_callback;
    }

}
