using UnityEngine;
using System.Collections;

public class AmbientSingleton : MonoBehaviour
{
    private static AmbientSingleton instance = null;
    public static AmbientSingleton Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
