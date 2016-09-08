using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngineInternal;
using System;

public class Resources_Manager : MonoBehaviour
{

    private static Resources_Manager mInstance = null;
    public static Resources_Manager Instance
    {
        get
        {
            if (mInstance == null)
            {
                var obj = new GameObject("ResourceManager");
                mInstance = obj.AddComponent<Resources_Manager>();
            }
            return mInstance;
        }
    }
    void Awake()
    {
        mInstance = this;
        this.gameObject.SetActive(false);
    }


    private static Dictionary<string, Resources_Bundle> _resourcebundles = new Dictionary<string, Resources_Bundle>();

    public static UnityEngine.Object Load(string p_filename, Type p_type)
    {
        if (IsLoad(p_filename))
        {
            return _resourcebundles[p_filename].Create();
        }

        var newResourceBundle = CreateResourceBundle(p_filename, p_type);
        return newResourceBundle == null ? null : newResourceBundle.Create();
    }

    public static UnityEngine.Object Load(string p_filename)
    {
        if (IsLoad(p_filename))
        {
            return _resourcebundles[p_filename].Create();
        }

        var newResourceBundle = CreateResourceBundle(p_filename);
        return newResourceBundle == null ? null : newResourceBundle.Create();
    }

    private static Resources_Bundle CreateResourceBundle(string p_file_path, Type p_type = null)
    {
        if (IsLoad(p_file_path))
        {
            return _resourcebundles[p_file_path];
        }

        var originalObject = AssetBundle_Loader.Instance.Load(p_file_path, p_type);

        if (originalObject == null)
        {
            return null;
        }

        var newResourceBundle = new Resources_Bundle(p_file_path, originalObject);
        _resourcebundles.Add(p_file_path, newResourceBundle);
        return newResourceBundle;
    }

    private static bool IsLoad(string p_filename)
    {
        return _resourcebundles.ContainsKey(p_filename);
    }
}