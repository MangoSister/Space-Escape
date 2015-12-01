using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
    //private static GridSystem gridSystem { get { return LevelController.Instance.gridSystem; } }
	public GameObject robot;
    public bool ManualIsMoving { get; private set; }

    public bool autoFollow;
    public Vector3 autoFollowOffset;
	private float proximity = 0.3f;
	private float smooth = 1f;
    private PlatformGroup prevGroup;

    private void Update()
    {
        if (autoFollow)
			follow (robot.transform.position);
    }

	private void follow(Vector3 followPos) {
		float distance = (robot.transform.position - transform.position).magnitude;
		if (distance > proximity) {
			Vector3 finalPos = robot.transform.position + Vector3.Normalize(transform.position - robot.transform.position) * proximity + transform.up * 1.25f;
			transform.position = Vector3.Lerp(transform.position, finalPos, smooth * Time.deltaTime);
		}
	}

    public void Move(Vector3 delta, float period)
    {
        ManualIsMoving = true;
        StartCoroutine(MoveCoroutine(delta, period));
    }

    private IEnumerator MoveCoroutine(Vector3 delta, float period)
    {
        Vector3 old = transform.position;
        float timer = 0f;
        while (Vector3.Distance(transform.position, old + delta) > 0.05f)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(old, old + delta, Mathf.Clamp01(timer / period));
            yield return null;
        }
        transform.position = old + delta;
        ManualIsMoving = false;  
    }

}
