using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class AssetBundle_Tool : EditorWindow
{
    //플랫폼 선택 인덱스
#if UNITY_ANDROID
    private int _Select_Platform = 1;
#elif UNITY_IOS
    private int _Select_Platform = 0;
#endif

    //국가 선택 인덱스
    private int _Select_Global = 0;
    //옵션 정의
    private string[] _Platform_Array = new string[] { "ios", "android" }; // 플랫폼
    private string[] _Global_Array = new string[] { "KR", "JP", "EN" }; // 국가코드
    private const string _subtype = ".unity3d";

    //기존번들 버전리스트
    private Dictionary<string, AssetBundle_Info> _AssetBundle_Info = new Dictionary<string, AssetBundle_Info>();

    //GUI옵션
    private Vector2 mScroll;

    [MenuItem("INI/AssetBundleTool")]
    public static void Init()
    {
        AssetBundle_Tool ABT = (AssetBundle_Tool)EditorWindow.GetWindow(typeof(AssetBundle_Tool), false, "Bundle Tool", true);
        ABT.minSize = new Vector2(700, 450);
        ABT.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal("box");
        {
            GUILayout.BeginVertical("box", GUILayout.Height(25));
            {
                GUILayout.Label("Platform Type", GUILayout.Height(25), GUILayout.Width(200));
                this._Select_Platform = GUILayout.SelectionGrid(this._Select_Platform, this._Platform_Array, 2, GUILayout.Height(25));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box", GUILayout.Height(25));
            {
                GUILayout.Label("Global Type", GUILayout.Height(25), GUILayout.Width(200));
                this._Select_Global = GUILayout.SelectionGrid(this._Select_Global, this._Global_Array, 3, GUILayout.Height(25));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box", GUILayout.Height(25));
            {
                if (GUILayout.Button("Player Pref Clear"))
                {
                    PlayerPrefs.DeleteAll();
                }
                if (GUILayout.Button("Delete Cache"))
                {
                    Caching.CleanCache();
                }
                if (GUILayout.Button("Open Output"))
                {
                    string __output_path = Application.dataPath;
                    __output_path = __output_path.Replace("/Assets", "/output/");
                    __output_path += _Global_Array[_Select_Global].ToLower();

                    if (_Select_Platform == 1)
                        __output_path += "/android";
                    else
                        __output_path += "/ios";

                    if (Directory.Exists(__output_path))
                        System.Diagnostics.Process.Start(__output_path);
                    else
                        Debug.LogError("empty Output Path!!  --  " + __output_path);

                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh", GUILayout.Height(25)))
        {
            //번들로 생성할 파일 리스트 생성
            Create_AssetBundle_List();
        }

        if (_AssetBundle_Info != null && _AssetBundle_Info.Count != 0)
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal("box", GUILayout.Height(25));
                GUILayout.Label("BundleName " + _AssetBundle_Info.Count.ToString(), GUILayout.Width(700));
                GUILayout.Label("Version", GUILayout.Width(150));
                GUILayout.Label("Count", GUILayout.Width(150));
                GUILayout.EndHorizontal();

                this.mScroll = GUILayout.BeginScrollView(this.mScroll);
                {
                    foreach (KeyValuePair<string, AssetBundle_Info> pair in _AssetBundle_Info)
                    {
                        GUILayout.BeginVertical("box", GUILayout.Height(25));
                        {
                            GUILayout.BeginHorizontal("box", GUILayout.Height(25));
                            GUILayout.Label(pair.Value._name, GUILayout.Width(700));
                            GUILayout.Label(pair.Value._version.ToString(), GUILayout.Width(150));
                            GUILayout.Label(pair.Value._filepaths.Count.ToString(), GUILayout.Width(150));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal("box", GUILayout.Height(25));
                            GUILayout.Label(pair.Value._md5, GUILayout.Width(700));
                            GUILayout.Label(pair.Value._size.ToString(), GUILayout.Width(150));
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            if (GUILayout.Button("Create & Update Bundle", GUILayout.Height(40)))
            {
                //번들 생성시작
                this.createAssetBundle();
            }

        }
    }

    private void Create_AssetBundle_List()
    {
        _AssetBundle_Info.Clear();
        AssetBundle_Manager.Instance._FTP_Bundle_Version = CB_AssetBundle_Version;
        AssetBundle_Manager.Instance.Get_FTP_Version();
    }

    private void CB_AssetBundle_Version(string p_version)
    {
        _AssetBundle_Info = AssetBundle_Manager.Instance.Get_Version(p_version);
        Get_File(AssetBundle_Define.String.AssetBundle_FolderPath);
    }



    //재귀함수를 돌면서 Resources_Bundle폴더 안에있는 모든 파일을 검사
    private void Get_File(string p_path)
    {
        //파일리스트 가져옴
        string[] files = Directory.GetFiles(p_path);
        //폴더리스트 가져옴
        string[] folders = Directory.GetDirectories(p_path);

        //파일리스트 검사
        foreach (string file in files)
        {
            //풀 경로가져옴
            string __full_path = Path.GetFullPath(file);
            if (__full_path.Contains(".meta"))
                continue;

            //+구분자로 폴더경로 구분
            string __key = AssetBundle_Define.String.Get_BundleName(__full_path,
                this._Platform_Array[this._Select_Platform], this._Global_Array[this._Select_Global]);

            //키값이 나오면 검사 후 리스트에 추가
            if (!_AssetBundle_Info.ContainsKey(__key))
            {
                AssetBundle_Info __info = new AssetBundle_Info();
                __info._version = 1;
                __info._name = __key;
                __info._filepaths = new List<string>();
                __info._size = 0;
                __info._md5 = "";
                _AssetBundle_Info.Add(__key, __info);
            }
            __full_path = AssetBundle_Define.String.Get_BundlePath(__full_path);
            _AssetBundle_Info[__key]._filepaths.Add(__full_path);
        }

        //폴더는 재귀함수를 탄다.
        foreach (string folder in folders)
        {
            string fullpath = Path.GetFullPath(folder);
            Get_File(fullpath);
        }
    }

    private void createAssetBundle()
    {
        int i = 0;
        //빌드 파이프라인에 필요한 에쎗번들빌드 배열
        AssetBundleBuild[] build = new AssetBundleBuild[this._AssetBundle_Info.Count];

        // 번들 묶는 부분
        //위에서 생성한 딕셔너리값을 사용하여 생성
        // 키가 번들의 이름이 된다.
        // 주의 
        // assetNames값은 딕셔너리 value값을 넣게된다( 경로 )
        // 경로의 시작은 프로젝트폴더가 시작지점이다 (Assets 폴더가 있는 경로)
        foreach (string key in this._AssetBundle_Info.Keys)
        {
            build[i] = new AssetBundleBuild();
            build[i].assetBundleName = this.getOutputPath() + key;
            build[i].assetNames = this._AssetBundle_Info[key]._filepaths.ToArray();
            i++;
        }
        BuildPipeline.BuildAssetBundles("output", build,
            BuildAssetBundleOptions.None
            , this.getBuildTarget());

        Create_Prefab_List();
    }

    private string getOutputPath()
    {
        string path = this._Global_Array[this._Select_Global] + "//";
        if (this._Select_Platform == 0)
        {
            path += "ios//";
        }
        else
        {
            path += "android//";
        }

        return path;
    }
    private BuildTarget getBuildTarget()
    {
        if (this._Select_Platform == 0)
        {
            return BuildTarget.iOS;
        }

        return BuildTarget.Android;
    }

    public void Create_Prefab_List()
    {
        string __path = Application.dataPath.Replace("\\", "/");
        __path = __path.Replace("Assets", "output/");
        __path = __path + this._Global_Array[this._Select_Global].ToLower() +
            (this._Select_Platform == 0 ? "/ios" : "/android");

        StreamWriter swriter = null;
        swriter = new StreamWriter(File.Open(__path + "/BundleInfo.txt", FileMode.Create, FileAccess.Write));

        string[] files = Directory.GetFiles(__path);

        foreach (string file in files)
        {
            string __name = Path.GetFileName(file);

            if (__name.Contains(".manifest") || __name.Contains(".txt"))
                continue;

            string __fullpath = Path.GetFullPath(file);

            //파일용량을 가져오기 위함
            FileInfo __fileinfo = new FileInfo(__fullpath);

            //해당파일의 MD5 생성
            string __MD5 = Get_MD5(__fullpath);
            int __version = 1;

            //이미 기존 데이터가 있는경우
            if (_AssetBundle_Info.ContainsKey(__name))
            {
                //MD5값이 다르면 버전증가 
                //동일시엔 그대로 버전 유지
                if (__MD5 != _AssetBundle_Info[__name]._md5)
                {
                    _AssetBundle_Info[__name]._version += 1;
                }
                __version = _AssetBundle_Info[__name]._version;
            }

            swriter.WriteLine(string.Format("{0}|{1:000000000}|{2}|{3}", __MD5, __fileinfo.Length, __version, __name));

        }
        swriter.Close();

        System.Diagnostics.Process.Start(__path);
    }

    public string Get_MD5(string p_path)
    {
        StringBuilder strMD5 = new StringBuilder();
        FileStream fs = new FileStream(p_path, FileMode.Open);
        byte[] byteResult = (new MD5CryptoServiceProvider()).ComputeHash(fs);
        fs.Close();

        for (int i = 0; i < byteResult.Length; i++)
        {
            strMD5.Append(byteResult[i].ToString("X2"));
        }

        return strMD5.ToString();
    }
}