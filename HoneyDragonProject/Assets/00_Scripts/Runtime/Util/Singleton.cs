using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Singleton Implementation
    private static bool shuttingDown = false;
    private static T instance;

    public static T Instance
    {
        get
        {
            if(shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                     "' already destroyed. Returning null.");
                return null;
            }
            if(instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if(instance == null)
                {
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + "(Singleton)";

                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        shuttingDown = true;
    }
    #endregion
}