using UnityEngine;
using System.Collections;

public class PSControllerSingleton : MonoBehaviour
{
    private static PSControllerSingleton instance = null;
    public static PSControllerSingleton Instance { get { return instance; } }
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
