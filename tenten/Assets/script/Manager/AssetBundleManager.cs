using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class AssetBundleManager : MonoBehaviour {

    //번들다운받을 주소
    private string Bundle_Host;
    //서버에서 받은 
    private List<AssetBundleVersionDTO> mVersionList = new List<AssetBundleVersionDTO>();
    //로드 된 list
    private Dictionary<string, AssetBundle> mAssetBundleHashMap = new Dictionary<string, AssetBundle>();

    public static long FreeSpece
    {
#if UNITY_EDITOR

        // Use this for initialization
        get
        {
            return 10000000000000;
        }
#elif UNITY_ANDROID
       
        get
        {
            using (AndroidJavaObject statFs = new AndroidJavaObject("android.os.StatFs", Application.persistentDataPath))
            {                
                return (long)statFs.Call<int>("getBlockSize") * (long)statFs.Call<int>("getAvailableBlocks");
            }
        }
#endif

    }


    void Start()
    {
        Bundle_Host = Static_Value.Strings.ASSET_BUNDLE_HOST;
        mVersionList.Clear();
        mAssetBundleHashMap.Clear();

        if (CheckCachFreeMemory())
        {
        }
    }

    private bool CheckCachFreeMemory()
    {
        //Caching.maximumAvailableDiskSpace = 해당변수에 정한용량을 초과할 경우
        //                                    이전파일을 삭제
        Caching.maximumAvailableDiskSpace = 557842432;
        if (FreeSpece >= 557842432) //TODO: ljh 추후 상수가 환경설정값으로 작업 되어지는 쪽으로 수정 
        {
            return true;
        }
        // 용량이 부족하다는 메세지를 띄운다 .

        return false;
    }

    private void LoadAssetBundle()
    {

    }

}
