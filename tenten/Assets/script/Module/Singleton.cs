using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static T _Instance;

    public static T Instance 
    {
        get
        {
            if( _Instance == null)
            {
                _Instance = (T)FindObjectOfType(typeof(T));

                if(_Instance != null && FindObjectsOfType(typeof(T)).Length > 1)
                {
                    return _Instance;
                }

                if( _Instance == null)
                {
                    var obj = new GameObject();
                    _Instance = obj.AddComponent<T>();
                    _Instance.name = typeof(T).ToString();

                    var root = GameObject.Find(Static_Value.Strings.ROOT_MANAGER_NAME);

                    if( root == null)
                    {
                        root = new GameObject(Static_Value.Strings.ROOT_MANAGER_NAME);
                        //SetAsFirstSibling = 하이어라키 순서 변경 함수
                        root.transform.SetAsFirstSibling();
                        //DontDestroyOnLoad =   새로운 레벨이 로드될때 이전레벨의 오브젝트가
                        //                      삭제되지 않게 하기위한 함수
                        DontDestroyOnLoad(root);
                    }

                    _Instance.transform.parent = root.transform;
                }
            }

            return _Instance;
        }

    }

    protected virtual void Awake()
    {
        if( _Instance == null)
        {
            _Instance = this as T;
        }
        DontDestroyOnLoad(gameObject);
    }
    
}


