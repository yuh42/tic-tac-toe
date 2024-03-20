using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public bool Global = true;
    
    private static T _instance = default;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (Global)
        {
            if (_instance != null && _instance != this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            _instance = this.gameObject.GetComponent<T>();
        }

        this.InitAwake();
    }
    
    protected virtual void InitAwake(){}//子类重写此方法当作Awake
}
