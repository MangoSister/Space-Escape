using UnityEngine;
using System.Collections;

public class Utility
{
    public static int ToLayerNumber(LayerMask mask)
    {
        for (int i = 0; i < 32; i++)
        {
            if ((1 << i) == mask.value)
                return i;
        }
        return -1;
    }

    public static float GaussianRandom(float mean, float stdDev)
    {  
        float u1 = Random.value; //these are uniform(0,1) random doubles
        float u2 = Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                     Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
        float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return randNormal;
    }
}
