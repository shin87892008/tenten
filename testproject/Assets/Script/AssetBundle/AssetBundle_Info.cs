using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AssetBundle_Info
{
    public string _md5;
    public ulong _size;
    public string _name;
    public int _version;
    public List<string> _filepaths;

    public AssetBundle_Info()
    {

    }
    public AssetBundle_Info(string[] p_info)
    {
        _md5 = p_info[0];
        _size = UInt64.Parse(p_info[1]);
        _version = Int32.Parse(p_info[2]);
        _name = p_info[3];
        _filepaths = new List<string>();
    }

    public bool Check_Sync(AssetBundle_Info p_info)
    {
        return _md5.Contains(p_info._md5) && _size == p_info._size &&
            _name.Contains(p_info._name) && _version == p_info._version;
    }
}