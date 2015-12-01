using UnityEngine;
using System.Collections;

public class DoorSwitch : MonoBehaviour
{
    public DoorController door;
    public bool triggered { get; private set; } //one-time usage

    public delegate void OnOpenHandler();
    public event OnOpenHandler OnOpen;

    private void Start()
    {
        OnOpen += door.SlideAutoDoor;
        triggered = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Beam") && !triggered)
        {
            triggered = true;
            OnOpen();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            triggered = true;
            OnOpen();
        }
    }
}
