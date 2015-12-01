using UnityEngine;
using System.Collections;

public class HandleControllerSingleton : MonoBehaviour
{
    private static HandleControllerSingleton instance = null;
    public static HandleControllerSingleton Instance { get { return instance; } }
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
