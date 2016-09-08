using System;
using UnityEngine;
using System.Collections;

public class AssetBundle_Loader : Singleton<AssetBundle_Loader>
{
    private bool IS_BUNDLE = false;
    private string PLATFORM = "android";
    private string GLOBAL_CODE = "kr";

    private string Get_Extension(string p_filename)
    {
        string __extension_key = p_filename.ToLower();

        if (!AssetBundle_Manager.Instance._Asset_Extensions.ContainsKey(__extension_key))
        {
            Debug.LogError(p_filename);
            return null;
        }
        else
        {
        }

        string __extension = AssetBundle_Manager.Instance._Asset_Extensions[__extension_key];

        return p_filename + __extension;
    }

    public UnityEngine.Object Load(string p_filename, Type p_type)
    {
        GC.Collect();

        //번들을 사용하려면 캐쉬로드
        if (IS_BUNDLE)
        {
            return LoadBundle(p_filename, p_type);
        }
#if UNITY_EDITOR
        //유니티에서 사용하려면 Resources_Bundle 사용
        else
        {
            string __path = AssetBundle_Define.String.UnityResourcePath + p_filename;
            __path = Get_Extension(__path);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(__path);
        }
#endif
        return null;
    }

    private UnityEngine.Object LoadBundle(string p_filename, Type p_type)
    {
        string __bundle_name = AssetBundle_Define.String.Get_BundleName(p_filename, PLATFORM, GLOBAL_CODE);
        string __asset_name = "Assets/Resources_Bundle/" + p_filename;
        __asset_name = Get_Extension(__asset_name);

        if (AssetBundle_Manager.Instance._Cur_AssetBundle.ContainsKey(__bundle_name) &&
            AssetBundle_Manager.Instance._Cur_AssetBundle[__bundle_name] != null &&
            AssetBundle_Manager.Instance._Cur_AssetBundle[__bundle_name].Contains(__asset_name))
        {
            UnityEngine.Object obj = null;

            if (p_type != null)
            {
                obj = AssetBundle_Manager.Instance._Cur_AssetBundle[__bundle_name].LoadAsset(__asset_name, p_type);
            }
            else
            {
                obj = AssetBundle_Manager.Instance._Cur_AssetBundle[__bundle_name].LoadAsset(__asset_name);
            }

            return obj;
        }

        Debug.LogError("Bundle Not Found : " + p_filename);
        return null;
    }
}
