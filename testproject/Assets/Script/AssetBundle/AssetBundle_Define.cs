using UnityEngine;



public class AssetBundle_Define
{

    public enum AssetBundle_Load_Type
    {
        SERVER,
        SCENE,
        TOOL,
    }

    public class String
    {
        private const string _AssetBundle_Host = "ftp://metallemon:0427@192.168.0.50/AssetBundle_Test/KR/";

        //+ 경로 정리
        private const string _Path_Scene_Bundles = "bundle_use";
        private const string _Path_FTP_Version_FileName = "BundleInfo.txt";
        private const string _Base_Resources_Path = "/Resources_Bundle/";

        //BundleInfo.txt
        public static string FTP_VersionFile
        {
            get
            {
                return _Path_FTP_Version_FileName;
            }
        }

        //Assets/Resources_Bundle/
        public static string UnityResourcePath
        {
            get
            {
                return "Assets" + _Base_Resources_Path;
            }
        }

        //C:/project/tenten/testproject/Assets/Resources_Bundle/
        public static string AssetBundle_FolderPath
        {
            get
            {
                return Application.dataPath + _Base_Resources_Path;
            }
        }


        //씬번들용 메모장 경로
        public static string Get_Scene_Bundle_Path()
        {
            return _Path_Scene_Bundles;
        }

        //FTP 호스트 경로
        public static string Get_FTP_Path(string p_filename)
        {
            string __url = _AssetBundle_Host;
            __url += "version_" + "0.0.1/";
            return __url + p_filename;
        }

        // ex > C:\\project\\tenten\\testproject\\Assets\\Resources_Bundle\\(Test_Dummy)\\MazeEdgefbx1.FBX
        // converting
        // ex > C:/project/tenten/testproject/Assets/Resources_Bundle/(Test_Dummy)/MazeEdgefbx1.FBX
        // ex > (Test_Dummy)/MazeEdgefbx1.FBX
        // ex > (Test_Dummy)+MazeEdgefbx1.FBX
        // ex > +(Test_Dummy)
        // ex > (Test_Dummy)
        // ex > (Test_Dummy).unity3d
        public static string Get_BundleName(string p_path, string p_platform, string p_global)
        {
            p_path = p_path.Replace("\\", "/");
            p_path = p_path.Replace(Application.dataPath + _Base_Resources_Path, "");
            p_path = p_path.Replace("/", "+");

            string __key = "";
            //+구분자로 폴더경로 구분
            string[] __arr_path = p_path.Split('+');
            //폴더경로로 키값을 지정한다.
            //최대3단계까지의 폴더를 키로 적용하고 
            //그 이후단계는 상위폴더와 함께 묶는다
            for (int i = 0; i < __arr_path.Length - 1; ++i)
            {
                if (i > 1)
                    break;
                __key = string.Format("{0}+{1}", __key, __arr_path[i]);
            }

            if (!string.IsNullOrEmpty(__key))
            {
                __key = __key.Substring(1);
            }
            else
            {
                __key = "empty";
            }
            __key = p_platform + "+" + p_global + "+" + __key;

            return (__key + ".unity3d").ToLower();
        }

        //유니티 에쎗에 들어가는 리소스 경로
        // ex > C:\\project\\tenten\\testproject\\Assets\\Resources_Bundle\\(Test_Dummy)\\MazeEdgefbx1.FBX
        // ex > C:/project/tenten/testproject/Assets/Resources_Bundle/(Test_Dummy)/MazeEdgefbx1.FBX
        // ex > Assets/Resources_Bundle/(Test_Dummy)/MazeEdgefbx1.FBX
        public static string Get_BundlePath(string p_path)
        {
            p_path = p_path.Replace("\\", "/");
            p_path = p_path.Replace(Application.dataPath + _Base_Resources_Path, "Assets/Resources_Bundle/");
            return p_path;
        }
    }
}
