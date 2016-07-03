using System;
using System.Collections.Generic;
using UnityEngine;

public class Static_Value
{
    public class Strings
    {
        public static string ROOT_MANAGER_NAME = "Managers";
        public static string ASSET_BUNDLE_HOST = "ftp://seongha@119.197.57.131:8181/shingha/unity/unity_bundle";
    }

    public class ints
    {
        public static float ASPECT = (float)720 / (float)Screen.width;
    }
}


[Serializable]
public class AssetBundleVersionDTO
{
    public int type;                           // 플랫폼
    public string country_code;                 // 국가코드
    public string name;                         // 번들이름 -> 폴더명
    public int version;                         // 번들버전
}

[Serializable]
public class Bingo
{
    public int index;
    public Bingo_Axis Axis;
}

public enum Bingo_Axis
{
    None,
    Horizontal,
    Vertical,
}