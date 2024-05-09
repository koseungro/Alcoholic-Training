/// 작성자: 백인성 
/// 작성일: 2018-08-17 
/// 수정일: 2018-08-17
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 용  도: 단일 클래스 접속을 위해 사용합니다.
/// 사용법: 사용하고 싶은 클래스가 이 싱글톤을 상속받아 사용해야 합니다. 단일 객체여야 합니다.
/// 수정이력 
/// 

using UnityEngine;
/// <summary>
/// 싱글톤 클래스 입니다.
/// </summary>
public class FNIVR_Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static object _lock = new object();
    private static bool applicationIsQuitting = false;

    /// <summary>
    /// 씬이 전환 될때 삭제되지 않는 오브젝트로 설정하고 싶으면
    /// 상속받은 클래스에서 이 변수를 true로 만들어 줘야 합니다.
    /// </summary>
    protected static bool dontDestroy = false;
    /// <summary>
    /// 상속받은 클래스가 존재하지 않는 경우 새로운 오브젝트를 생성하고 상속받은 클래스를 붙여주는 역할을 합니다.
    /// </summary>
    protected static bool needNewGameObject = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    T[] objects = FindObjectsOfType(typeof(T)) as T[];
                    if (objects.Length > 1)
                    {
                        Debug.Log("Find : " + _instance.name);
                        Debug.LogError("Singleton: " + objects.Length.ToString() + "가 존재함. " + _instance.name);

                        return _instance;
                    }
                    
                    if (needNewGameObject)
                    {
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "_" + typeof(T).ToString() + "_";
                            //DontDestroyOnLoad(singleton);
                        }
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this as T;

                if(dontDestroy)
                    DontDestroyOnLoad(this.gameObject);
            }
            else if (_instance != this.GetComponent<T>())
            {
                Destroy(this.gameObject);
            }
        }
    }

    public virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;

        if (dontDestroy)
            _instance = null;
    }
}
