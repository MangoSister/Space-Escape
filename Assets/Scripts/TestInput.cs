using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestInput : MonoBehaviour
{
    public GridSystem gridSystem;

    bool flag;
    // Update is called once per frame

    private void Start()
    {
        HashSet<PlatformGroup> init = new HashSet<PlatformGroup>();
        init.Add(gridSystem.PlacePlatform(0, 0));
        init.Add(gridSystem.PlacePlatform(0, 1));

        init.Add(gridSystem.PlacePlatform(1, 2));
        init.Add(gridSystem.PlacePlatform(2, 2));

        init.Add(gridSystem.PlacePlatform(3, 3));
        init.Add(gridSystem.PlacePlatform(3, 4));
        init.Add(gridSystem.PlacePlatform(4, 3));

        //PlatformGroup.Restructure(init);
        //var player = gridSystem.PlacePlatform(2, 2);
        //PlatformGroup.Combine(temp1, player);


    }
}
