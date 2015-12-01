using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelController : MonoBehaviour
{
    public int sizeX, sizeZ;
    public float cellSize;
    public int startX, startZ;
    public int goalX, goalZ;

	public GridSystem gridSystem;
    public RobotController robotController;
    public Player player;
    public Goal goal;
    public LevelTimer levelTimer;
    public HandleController handleController;
    public fadeInOut_ImageEffect fadeEffect;
    public ExplosionManager expManager;
	public GameObject emergency;
	private float lightRotateSpeed = 2f;

    private static LevelController _instance = null;
    public static LevelController Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {           
            _instance = this;
            _instance.Init();
        }
        //DontDestroyOnLoad(this.gameObject);
    }
    private void OnDestroy()
    {
        PlatformGroup.gridSystem = null;
        HandleController.gridSystem = null;
        BeamTriggerController.gridSystem = null;
        PlatformGroup.OnGroupMoved -= robotController.replanPath;
    }

    private void Init ()
    {
        handleController.gameObject.SetActive(false);
        robotController.transform.position =
            new Vector3(gridSystem.transform.position.x + startX * cellSize, 
            gridSystem.transform.position.y + 0.125f, 
            gridSystem.transform.position.z + startZ * cellSize);
        player.transform.position = robotController.transform.position - Vector3.forward + 0.5f * Vector3.up * 3f;
        player.transform.LookAt(goal.transform);
        robotController.setFinalPosition (new Int2 (goalX, goalZ));
        PlatformGroup.gridSystem = gridSystem;
        HandleController.gridSystem = gridSystem;
		BeamTriggerController.gridSystem = gridSystem;
        PlatformGroup.OnGroupMoved += robotController.replanPath;

        ManualInitGrid();

        Vector3 goalPos = goal.transform.position;
        goalPos.x = gridSystem.transform.position.x + goalX * cellSize;
        goalPos.z = gridSystem.transform.position.z + goalZ * cellSize;
        goal.transform.position = goalPos;

        StartCoroutine(EnterCoroutine());
    }


    private void ManualInitGrid()
    {
        gridSystem.Init(new Int2(sizeX, sizeZ), cellSize, new Int2(goalX, goalZ));
        //gridSizeX = 7
        //gridSizeZ = 20
        if (gridSystem.gridSizeX != 7 || gridSystem.gridSizeZ != 22)
            throw new UnityException("gridSystem has a different size  than that of manual initialization ");

        int[,] map = new int[7, 22]
            {
                { 0,0,0,1,0,0,0,0,0,1,1,1,0,1,0,1,1,1,0,1,1,1},
                { 0,0,0,1,0,1,1,0,1,0,0,0,0,1,0,1,0,0,1,0,1,0 },
                { 0,1,1,0,1,0,1,0,0,1,0,1,1,0,0,0,1,0,0,1,0,0 },
                { 1,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,1,1 },
                { 0,0,0,1,0,1,0,0,1,0,1,0,0,1,0,1,0,0,1,0,0,0 },
                { 0,1,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,1,0,1,1 },
                { 0,1,1,0,1,1,0,1,1,0,0,1,1,0,1,1,0,1,1,0,1,0 }
            };


        HashSet<PlatformGroup> init = new HashSet<PlatformGroup>();
        for (int x = 0; x < 7; x++)
            for (int z = 0; z < 22; z++)
            {
                if (map[x, z] == 1)
                    init.Add(gridSystem.PlacePlatform(x, z));
            }

        PlatformGroup.Restructure(init, gridSystem.ComputeIdx(new Vector2(robotController.transform.position.x, robotController.transform.position.z)));
    }


    private void Update()
    {
        if(emergency.activeSelf)
		    emergency.transform.RotateAround (emergency.transform.position, emergency.transform.up, lightRotateSpeed);
    }


    private IEnumerator EnterCoroutine()
    {
        //fade in
        fadeEffect.beginFadeIn_WithBlur(0.25f, 0.03f, null);
        //robot animation
        robotController.BehaveScare();
        yield return new WaitForSeconds(3f);
        robotController.PointExit();
        handleController.gameObject.SetActive(true);
        yield return null;
    }
}