using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Goal : MonoBehaviour
{
    public DroneRobotController droneRobotPrefab;
    public RobotController robot;
    public DoorController exitDoor;

    public GameObject dome;

    public Transform friendsParent;
    public List<Transform> friendsTransform;
    public List<Color> friendsColor;
    public List<AnimationClip> friendsClips;

    public AudioSource musicSource;
    public AudioClip endingClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Robot")
            return;
        StartCoroutine(PlayEnding());
    }

    private IEnumerator PlayEnding()
    {
        LevelController.Instance.emergency.SetActive(false);
        LevelController.Instance.expManager.enabled = false;
        LevelController.Instance.levelTimer.enabled = false;
        dome.SetActive(true);
        if (musicSource.isPlaying)
            musicSource.Stop();
        musicSource.clip = endingClip;
        musicSource.loop = false;
        musicSource.Play();

        for (int i = 0; i < friendsTransform.Count; i++)
        {
            var drone = Instantiate(droneRobotPrefab, friendsTransform[i].position, friendsTransform[i].rotation) as DroneRobotController;
            drone.transform.parent = friendsParent;
            drone.ChangeColor(friendsColor[i]);
            drone.ChangeExpression(DroneRobotController.Expression.Happy);
            drone.Celebrate();

            drone.robotAnimation.AddClip(friendsClips[i], friendsClips[i].name);
            drone.robotAnimation.clip = friendsClips[i];
            drone.PlayPresetAnimation();
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        }
        exitDoor.OpenDoor();

		robot.body.transform.LookAt (exitDoor.transform.position);
		robot.ChangeExpression (RobotController.Expression.Happy);
        robot.robotAnim.SetTrigger("Celebrate");
        StartCoroutine(MoveToExitCoroutine());
    }

    private IEnumerator MoveToExitCoroutine()
    {
        Vector3 old = robot.transform.position;
        Vector3 target = exitDoor.gameObject.transform.position;
        target.y = old.y;
        float timer = 0f;
        while (Vector3.Distance(old, target) > 0.5f)
        {
            timer += Time.deltaTime;
            robot.transform.position = Vector3.Lerp(old, target, Mathf.Clamp01(timer / 2f));
            yield return null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(PlayEnding());
        }
    }
}