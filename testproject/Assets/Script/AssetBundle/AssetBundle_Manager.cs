using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class AssetBundle_Manager : Singleton<AssetBundle_Manager>
{
    public int _AssetBundle_Size = 0;
    //현재 씬에서 사용할 번들
    public Dictionary<string, AssetBundle> _Cur_AssetBundle = new Dictionary<string, AssetBundle>();

    //사용할 파일의 확장자 저장
    public Dictionary<string, string> _Asset_Extensions = new Dictionary<string, string>();

    #region 씬별 번들정보
    //씬별로 사용할 번들정보
    private Dictionary<int, Dictionary<string, AssetBundle_Info>> _Scene_Use_Bundle = new Dictionary<int, Dictionary<string, AssetBundle_Info>>();
    public void Load_Bundle_Use_Info()
    {
        var asset = Resources.Load<TextAsset>(AssetBundle_Define.String.Get_Scene_Bundle_Path());
        var sreader = new StreamReader(new MemoryStream(asset.bytes));
        
        while (true)
        {
            string __data = sreader.ReadLine();

            if (string.IsNullOrEmpty(__data))
                break;

            string[] __arr_data = __data.Split('|');

            AssetBundle_Info __bundleinfo = new AssetBundle_Info();
            int __key = Int32.Parse(__arr_data[0]);
            __bundleinfo._name = __arr_data[1];

            string __url = AssetBundle_Define.String.Get_FTP_Path(__bundleinfo._name);

            __bundleinfo._version = 0;

            if (!_Scene_Use_Bundle.ContainsKey(__key))
            {
                _Scene_Use_Bundle.Add(__key, new Dictionary<string, AssetBundle_Info>());
            }
            _Scene_Use_Bundle[__key].Add(__bundleinfo._name, __bundleinfo);
        }
        sreader.Close();
    }
    #endregion



    #region Load AssetBundle

    public Action<float, string, int, int> _ProgressCallBack;
    public Action _AssetBundle_End_CallBack;
    public Action<string> _FTP_Bundle_Version;

    //확인할 번들 리스트
    private Dictionary<string, AssetBundle_Info> _Download_List = new Dictionary<string, AssetBundle_Info>();

    // 서버에서받아온 스트링 번들버전 정보
    private string _AssetBundle_String;

    // 다운로드 완료 확인 변수
    private bool _is_Loading_WWW;

    //서버에셋번들 다운로드
    public void AssetBundle_Download()
    {
        StartCoroutine(CR_Get_Cur_FTP_AssetBundle_Version(AssetBundle_Define.AssetBundle_Load_Type.SERVER));
    }

    //씬에 필요한 번들로드
    public void Scene_Bundle_Download(int p_SceneIndex)
    {
        _Cur_AssetBundle.Clear();
        _Asset_Extensions.Clear();
        //다운받을 리스트 생성
        _Download_List = _Scene_Use_Bundle[p_SceneIndex];
        StartCoroutine(DownLoad_AssetBundle(AssetBundle_Define.AssetBundle_Load_Type.SCENE));
    }

    public void Get_FTP_Version()
    {
        StartCoroutine(CR_Get_Cur_FTP_AssetBundle_Version(AssetBundle_Define.AssetBundle_Load_Type.TOOL));
    }

    //FTP에 있는 번들버전 정보를 받아온다
    private IEnumerator CR_Get_Cur_FTP_AssetBundle_Version(AssetBundle_Define.AssetBundle_Load_Type p_type)
    {
        //메모장 내용 읽어옴
        var WWW = new WWW(AssetBundle_Define.String.Get_FTP_Path(AssetBundle_Define.String.FTP_VersionFile));
        yield return WWW;

        if (p_type != AssetBundle_Define.AssetBundle_Load_Type.TOOL)
        {
            //MD5 체크 
            Check_MD5(WWW.text);
        }
        else
        {
            if (_FTP_Bundle_Version != null)
            {
                _FTP_Bundle_Version(WWW.text);
                _FTP_Bundle_Version = null;
            }
        }
        WWW.Dispose();
    }
    private void Check_MD5(string p_version)
    {
        //서버에 있는 버전정보받아옴
        _AssetBundle_String = p_version;

        //다운받을 리스트 생성
        _Download_List = AssetBundle_Manager.Instance.Get_Version(_AssetBundle_String);

        //다운로드 시작
        StartCoroutine(DownLoad_AssetBundle(AssetBundle_Define.AssetBundle_Load_Type.SERVER));
    }

    //정해진 리스트를 다운로드
    private IEnumerator DownLoad_AssetBundle(AssetBundle_Define.AssetBundle_Load_Type p_type)
    {
        //용량 체크
        Caching.maximumAvailableDiskSpace = 1000 * 1024 * 1024;

        if (!CheckCachFreeMemory())
        {
            Application.Quit();
        }

        //번들 다운받는 카운트
        int __load_count = 1;

        //다운로드 시작
        foreach (KeyValuePair<string, AssetBundle_Info> pair in _Download_List)
        {
            while (_is_Loading_WWW)
                yield return null;
            //url 생성
            string __url = AssetBundle_Define.String.Get_FTP_Path(pair.Key);

            if (p_type == AssetBundle_Define.AssetBundle_Load_Type.SERVER)
            {
                //이미 받아져 있는지 검사
                if (Caching.IsVersionCached(__url, pair.Value._version))
                {
                    //자동 삭제 방지를 위해 정보를 갱신한다 (내부적으로 날짜가 갱신된다)
                    if (Caching.MarkAsUsed(__url, pair.Value._version) == false)
                    {
                        Debug.LogError("MarkAsUsed Error : " + __url + " ver : " + pair.Value._version);
                    }
                }
                else
                {
                    _is_Loading_WWW = true;
                    StartCoroutine(CR_Load_Bundle(p_type, __url, pair.Value, __load_count));
                }
            }
            else
            {
                _is_Loading_WWW = true;
                StartCoroutine(CR_Load_Bundle(p_type, __url, pair.Value, __load_count));
            }

            __load_count++;
        }


        if (_AssetBundle_End_CallBack != null)
        {
            _AssetBundle_End_CallBack();
        }
        _is_Loading_WWW = false;
        yield break;
    }
    private IEnumerator CR_Load_Bundle(AssetBundle_Define.AssetBundle_Load_Type p_type, string p_url, AssetBundle_Info p_bundle_info, int p_count)
    {
        WWW __www = WWW.LoadFromCacheOrDownload(p_url, p_bundle_info._version);

        if (__www == null)
        {
            yield break;
        }

        while (!__www.isDone && __www.error == null)
        {
            //다운로드중일때 프로그래스 처리
            if (_ProgressCallBack != null)
            {
                _ProgressCallBack(__www.progress, p_bundle_info._name, p_count, _Download_List.Count);
            }
            yield return null;
        }

        if (__www.error == null)
        {
            if (p_type == AssetBundle_Define.AssetBundle_Load_Type.SCENE)
            {
                _Cur_AssetBundle.Add(p_bundle_info._name.ToLower(), __www.assetBundle);

                string[] __assetnames = __www.assetBundle.GetAllAssetNames();

                for (int i = 0; i < __assetnames.Length; ++i)
                {
                    string[] __arr_names = __assetnames[i].Split('.');

                    string __key = __arr_names[0];
                    string __extension = __arr_names[1];

                    if (__extension != "fbx" && __extension != "mat"
                        && __extension != "controller" && __extension != "anim")
                    {
                        if (!_Asset_Extensions.ContainsKey(__key))
                        {
                            _Asset_Extensions.Add(__key, "." + __extension);
                        }
                    }
                    else
                    {
                        //Debug.LogError("Delete File : " + __assetnames[i]);
                    }
                }

                //_AssetBundle_Size = _AssetBundle_Size + __www.bytes.Length;
            }
            else
            {
                //_AssetBundle_Size = _AssetBundle_Size + __www.bytes.Length;
                __www.assetBundle.Unload(true);
            }
        }
        else if (__www != null && __www.error != null)
        {
            Debug.LogError("WWW is Error : " + __www.error);
        }
        //로드가 완료되면
        _is_Loading_WWW = false;
        yield break;
    }

    private bool CheckCachFreeMemory()
    {
        Caching.maximumAvailableDiskSpace = 1000 * 1024 * 1024;
        if (FreeSpece >= 1000 * 1024 * 1024) //TODO: ljh 추후 상수가 환경설정값으로 작업 되어지는 쪽으로 수정 
        {
            return true;
        }

        return false;
    }
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

    // 서버에서 받아온 번들정보를 컨버팅
    public Dictionary<string, AssetBundle_Info> Get_Version(string p_bundle_info)
    {
        Dictionary<string, AssetBundle_Info> __assetbundle = new Dictionary<string, AssetBundle_Info>();

        if (string.IsNullOrEmpty(p_bundle_info))
        {
            Debug.LogWarning("playerPrefas empty assetbundle info");
            return __assetbundle;
        }

        p_bundle_info = p_bundle_info.Replace("\r\n", "&");

        string[] FTP_MD5_Info = p_bundle_info.Split('&');

        for (int i = 0; i < FTP_MD5_Info.Length; ++i)
        {
            FTP_MD5_Info[i] = FTP_MD5_Info[i].Replace("\r", "");
            if (!string.IsNullOrEmpty(FTP_MD5_Info[i]))
            {
                AssetBundle_Info __info = new AssetBundle_Info(FTP_MD5_Info[i].Split('|'));
                __assetbundle.Add(__info._name, __info);
            }
        }

        return __assetbundle;
    }
    #endregion
}

