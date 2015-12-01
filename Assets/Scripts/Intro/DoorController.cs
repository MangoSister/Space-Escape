using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    private Animator doorAnim;
    public GameObject leftDoorCollider;
    public GameObject rightDoorCollider;
    public AudioSource openDoorSound;
    public AudioSource closeDoorSound;

	public bool triggered;

    public float slideDist;
    public float slideTime; //slide along local Z axis
    public float waitTime;

    private void Start()
    {
		triggered = false;
        doorAnim = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        doorAnim.SetTrigger("OpenDoor");
        doorAnim.ResetTrigger("CloseDoor");
        StartCoroutine(MoveDoorCoroutine(true, true));
        StartCoroutine(MoveDoorCoroutine(false, true));
        if (closeDoorSound.isPlaying)
            closeDoorSound.Stop();
        openDoorSound.Play();
    }

    public void CloseDoor()
    {
        doorAnim.SetTrigger("CloseDoor");
        doorAnim.ResetTrigger("OpenDoor");
        StartCoroutine(MoveDoorCoroutine(true, false));
        StartCoroutine(MoveDoorCoroutine(false, false));
        if (openDoorSound.isPlaying)
            openDoorSound.Stop();
        closeDoorSound.Play();
    }

    public void SlideAutoDoor()
    {
        StartCoroutine(AutoDoorCoroutine(waitTime));
    }

    private IEnumerator AutoDoorCoroutine(float waitTime)
    {
        OpenDoor();
        yield return new WaitForSeconds(waitTime);
        CloseDoor();
    }

    private IEnumerator MoveDoorCoroutine(bool right, bool open)
    {
        Vector3 currVelo = Vector3.zero;
        GameObject halfDoor = right ? rightDoorCollider : leftDoorCollider;
        Vector3 target = halfDoor.transform.position +
            halfDoor.transform.forward * slideDist * (right ? -1f : 1f) * (open ? 1f : -1f);
        
        while (Vector3.Distance(halfDoor.transform.position, target) > 0.01f)
        {
            halfDoor.transform.position = 
                Vector3.SmoothDamp(halfDoor.transform.position, target, ref currVelo, slideTime);
            yield return null;
        }
    }
}
