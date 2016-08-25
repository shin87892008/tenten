using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Ios_Payment : Payment_Base
{
    public override void Init(List<string> p_product)
    {

    }
    public override void Get_Product_List(Action<List<Product_Data>> p_callback)
    {
    }

    public override void Get_Purchase_List(Action<List<Purchase_Data>> p_callback)
    {
    }
    public override void Purchase(string p_sku, Action<bool, Purchase_Data> p_callback)
    {

    }
    public override void Consume(string p_sku, Action p_callback)
    {

    }
}
