using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WhaleFall
{
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
    protected static T m_sInstance = null;
    public static T GetInstance()
    {
        if (m_sInstance == null)
        {
            CreateSingleton();
        }
        return m_sInstance;
    }

    public static bool HasInstance()
    {
        return (m_sInstance != null);
    }

    public static T ins { get { return GetInstance(); } }

    protected static void CreateSingleton()
    {
        if (m_onApplicationQuit) return;
        GameObject root = GameObject.Find("GameMain");
        if (root == null)
        {
            root = new GameObject("GameMain");
            DontDestroyOnLoad(root);
        }

        GameObject singleton = new GameObject(typeof(T).ToString());
        singleton.transform.parent = root.transform;
        m_sInstance = singleton.AddComponent<T>();
        //m_sInstance.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
    }

    protected virtual void Awake()
    {
        //Debug.Log( "Singleton awake " );
        if (m_sInstance != null)
        {
            //Debug.Log( "!!!" );
            if (this != m_sInstance)
                Destroy(this.gameObject);
            return;
        }

        try
        {
            m_sInstance = this as T;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(transform.root.gameObject);
        Init();
    }

    private static bool m_onApplicationQuit = false;
    protected virtual void OnApplicationQuit()
    {
        m_onApplicationQuit = true;
    }

    public virtual void Init() { }

	public virtual void OnDestroy()
	{
		if (m_sInstance == this)
		{
			m_sInstance = null;
		}

	}
}
}