using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public DoorController door;
    public bool triggered { get; private set; } //one-time usage

    public delegate void OnOpenHandler();
    public event OnOpenHandler OnOpen;

    private void Start()
    {
        OnOpen += door.SlideAutoDoor;
		door.triggered = false;
        triggered = false;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Beam") && HandleController.beamOn)
        {
			door.triggered = true;
            triggered = true;
            OnOpen();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
			door.triggered = true;
            triggered = true;
            OnOpen();
        }
    }
}
