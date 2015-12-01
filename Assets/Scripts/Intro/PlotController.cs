using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlotController : MonoBehaviour
{
    public DroneRobotController droneRobot;
    public DoorController doorController;
    public Player player;
    public GridSystem gridSystem;
    public AlarmTrigger alarmTrigger;
    public HandleController handleController;
    //public FadeInOut fadeInOut;
    public fadeInOut_ImageEffect fadeEffect;

	public GameObject highlight;

	private float highlightScaleSize = 0.1f;
	private float highlightScaleSpeed = 2.0f;

	public Image hintBubble;
	public Image hintBubble2;
	public float hintBubbleSwitchTime = 0.75f;
	private float currentTime;
    private Queue<string> plotQueue;
    private Queue<object> paramQueue;
    private bool currPlotEnd;
    private bool tutorialFinished;
    private bool meteoriteHit;

    private void OnDestroy()
    {
        HandleController.gridSystem = null;
        PlatformGroup.gridSystem = null;
        PlatformGroup.OnGroupMoved -= OnTutorialFinish;
        BeamTriggerController.gridSystem = null;
    }

    private void Start()
	{		
		currentTime = Time.time;
		Debug.Assert(droneRobot != null);
        plotQueue = new Queue<string>();
        paramQueue = new Queue<object>();

        HandleController.gridSystem = gridSystem;
        PlatformGroup.gridSystem = gridSystem;
        PlatformGroup.OnGroupMoved += OnTutorialFinish;
		BeamTriggerController.gridSystem = gridSystem;

        alarmTrigger.onMeteoriteHit += OnMeteoriteHit;
        ManualInitGrid();

        //wait for player opening the door
        plotQueue.Enqueue("DoorOpenCoroutine");
        paramQueue.Enqueue(new object());

        //wait for door sliding
        plotQueue.Enqueue("WaitCoroutine");
        paramQueue.Enqueue(new WaitParam(2f));

        //player step into the room
        plotQueue.Enqueue("PlayerMoveCoroutine");
        paramQueue.Enqueue(new MoveParam(Vector3.forward * 3f, 4f));

        //interaction, not sure what to do 
        plotQueue.Enqueue("RobotWelcomeCoroutine");
        paramQueue.Enqueue(new object());

        plotQueue.Enqueue("RobotRotateCoroutine");
        paramQueue.Enqueue(new RobotRotateParam(Quaternion.LookRotation(Vector3.forward, Vector3.up), 2f));


        //Robot move, player follow
        plotQueue.Enqueue("MoveFollowCoroutine");
        paramQueue.Enqueue(new MoveFollowParam(Vector3.forward * 6f, 12f, 0.5f));


        //tutorial
        plotQueue.Enqueue("TutorialCoroutine");
        paramQueue.Enqueue(new object());


        plotQueue.Enqueue("MoveFollowCoroutine");
        paramQueue.Enqueue(new MoveFollowParam(Vector3.forward * 10f, 20f, 0.5f));

        plotQueue.Enqueue("TransitionCoroutine");
        paramQueue.Enqueue(new object());

        currPlotEnd = true;
        tutorialFinished = false;
        meteoriteHit = false;
    }

    private void Update()
    {
        if (currPlotEnd && plotQueue.Count > 0)
        {
            string coroutine = plotQueue.Dequeue();
            object param = paramQueue.Dequeue();
            currPlotEnd = false;
            StartCoroutine(coroutine, param);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            gridSystem[4, 8].group.StartMoveGroup
                (PlatformMoveType.AxisX, gridSystem.ComputeIdx(new Vector2(droneRobot.transform.position.x, droneRobot.transform.position.z)));
        }
    }

    private IEnumerator DoorOpenCoroutine(object param)
    {
        fadeEffect.beginFadeIn_WithBlur(0.25f, 0f, null);
		droneRobot.ChangeExpression(DroneRobotController.Expression.Stoic);
		hintBubble.enabled = false;
        hintBubble2.enabled = false;
		highlight.SetActive (false);
        while (!doorController.triggered)
            yield return null;
        handleController.gameObject.SetActive(false);
        currPlotEnd = true;
    }

    private IEnumerator PlayerMoveCoroutine(object param)
    {
        MoveParam p = param as MoveParam;
        player.Move(p.delta, p.period);
        while (player.ManualIsMoving)
            yield return null;
        currPlotEnd = true;
    }

    private IEnumerator RobotMoveCoroutine(object param)
    {  
        MoveParam p = param as MoveParam;
        droneRobot.Move(p.delta, p.period);
        while (droneRobot.IsMoving)
            yield return null;
        currPlotEnd = true;
    }

    private IEnumerator MoveFollowCoroutine(object param)
    {
        
        MoveFollowParam p = param as MoveFollowParam;
        droneRobot.Move(p.delta, p.period);
        yield return new WaitForSeconds(p.delay);
        player.Move(p.delta, p.period);
        while (player.ManualIsMoving && !meteoriteHit)
            yield return null;
        currPlotEnd = true;
    }

    private IEnumerator RobotWelcomeCoroutine(object param)
    {
        droneRobot.Wave();
        droneRobot.ChangeExpression(DroneRobotController.Expression.Happy);
        yield return new WaitForSeconds(2f);
        droneRobot.Celebrate();
        yield return new WaitForSeconds(3.5f);
        droneRobot.ChangeExpression(DroneRobotController.Expression.Smile);
        currPlotEnd = true;        
    }

    private IEnumerator RobotRotateCoroutine(object param)
    {
        RobotRotateParam p = param as RobotRotateParam;
        droneRobot.RotateTo(p.target, p.period);
        while (droneRobot.IsMoving)
            yield return null;
        currPlotEnd = true;

    }

    private IEnumerator WaitCoroutine(object param)
    {
        yield return new WaitForSeconds((param as WaitParam).time);
        currPlotEnd = true;
    }

    private IEnumerator TutorialCoroutine(object param)
    {
		highlight.SetActive(true);
		hintBubble.enabled = true;
        handleController.gameObject.SetActive(true);
        droneRobot.Talk();
        while (!tutorialFinished) {
			float scale = 1 + Mathf.Abs(Mathf.Sin(Time.time * highlightScaleSpeed) * highlightScaleSize);
			highlight.transform.localScale = new Vector3 (scale, highlight.transform.localScale.y, scale);
			if (currentTime + hintBubbleSwitchTime < Time.time) {
				hintBubble.enabled = !hintBubble.enabled;
				hintBubble2.enabled = !hintBubble2.enabled;
				currentTime = Time.time;
			}
			yield return null;
		}
		hintBubble.enabled = false;
		hintBubble2.enabled = false;
		highlight.SetActive(false);
		handleController.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        currPlotEnd = true;
    }

    private IEnumerator TransitionCoroutine(object param)
    {
        yield return new WaitForSeconds(2f);
        //float fadeTime = fadeInOut.BeginFade(1);
        //yield return new WaitForSeconds(fadeTime);
		fadeEffect.enabled = true;
        fadeEffect.beginFadeOut(5f, () => { Application.LoadLevel("main"); });
        currPlotEnd = true;
    }

    private void OnTutorialFinish()
    {
        tutorialFinished = true;
    }

    private void OnMeteoriteHit()
    {
        meteoriteHit = true;       
    }

    private void ManualInitGrid()
    {
        const int sizeX = 7;
        const int sizeZ = 23;
        const float cellSize = 1f;

        gridSystem.Init(new Int2(sizeX, sizeZ), cellSize, new Int2(3,22));
        int[,] map = new int[sizeX, sizeZ]
            {
                { 0,0,0,0,0,0,0,0,0,0 , 0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 , 0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 1,1,1,1,1,1,1,1,1,1 , 1,1,1,1,1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,0,1 , 1,1,1,1,1,1,1,1,1,1,1,1,1 },
                { 0,0,0,0,0,0,0,0,1,0 , 0,0,0,0,0,0,0,0,0,0,0,0,0  },
                { 0,0,0,0,0,0,0,0,0,0 , 0,0,0,0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 , 0,0,0,0,0,0,0,0,0,0,0,0,0 }
            };

        HashSet<PlatformGroup> init = new HashSet<PlatformGroup>();
        for (int x = 0; x < sizeX; x++)
            for (int z = 0; z < sizeZ; z++)
            {
                if (map[x, z] == 1)
                    init.Add(gridSystem.PlacePlatform(x, z));
            }

        PlatformGroup.Restructure(init, new Int2(3, 0));
    }

    private class MoveParam
    {
        public Vector3 delta;
        public float period;
        public MoveParam(Vector3 d, float p) { delta = d; period = p; }
    }

    private class RobotRotateParam
    {
        public Quaternion target;
        public float period;
        public RobotRotateParam(Quaternion t, float p) { target = t; period = p; }
    }

    private class WaitParam
    {
        public float time;
        public WaitParam(float t) { time = t; }
    }

    private class MoveFollowParam
    {
        public Vector3 delta;
        public float period;
        public float delay;
        public MoveFollowParam(Vector3 d, float p, float de) { delta = d; period = p; delay = de; }
    }


}
