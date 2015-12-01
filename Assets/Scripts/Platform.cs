using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// The Platform class describes the movement of each platform
/// Call Move() function and pass one Platform.PlatformMoveType to move a platform (aligning to the grid)
/// </summary>

public enum PlatformMoveType
    {
        AxisX, AxisZ,
    }

public class Platform : MonoBehaviour
{

    public GridSystem gridSystem;
    public PlatformGroup group;
    public Int2 index;

    public float transitionTime = 0.5f;
    public float truncateDist = 0.01f;
    private Vector3 currVelocity;
    public bool IsMoving { get; private set; }
	
	public GameObject platformLights;
	MeshRenderer platformLightsRenderer;
	
	Color onColor = new Color (0.49f, 1.0f, 0, 1.0f);
	Color offColor = new Color (0.26f, 0.26f, 0.26f, 1.0f);


    private PlatformSound sound;

	public void OnTriggerEnter() {
		if (this.gameObject.layer == Utility.ToLayerNumber (gridSystem.movablePfLayer)) {
			group.Activate ();
		}
	}

	public void OnTriggerExit() {
		group.Deactivate ();
	}

	// Use this for initialization
	private void Start ()
    {
        currVelocity = Vector3.zero;
        IsMoving = false;
        sound = GetComponent<PlatformSound>();
		Debug.Assert(sound != null);
		platformLightsRenderer = platformLights.GetComponent<MeshRenderer> ();
		SetLightOff ();
	}

	public void Activate() {
		SetLightOn ();
	}
	
	public void Deactivate() {
		SetLightOff ();
	}

	public void SetLightOff(){
		platformLightsRenderer.material.SetColor ("_EmissionColor", offColor);
	}
	
	public void SetLightOn(){
		platformLightsRenderer.material.SetColor ("_EmissionColor", onColor);
	}


    public void StartMove(PlatformMoveType type, int units)
    {
        switch (type)
        {
            case PlatformMoveType.AxisX:
                {
                    Vector3 target = transform.position + units * Vector3.right * gridSystem.cellSize;
                    IsMoving = true;
                    StartCoroutine(MoveCoroutine(target, type));
                    //else
                    break;
                }
            case PlatformMoveType.AxisZ:
                {
                    Vector3 target = transform.position + units * Vector3.forward * gridSystem.cellSize;
                    IsMoving = true;
                    StartCoroutine(MoveCoroutine(target, type));
                    //else
                    break;
                }
            default: throw new NotImplementedException();
        }

    }

    private IEnumerator MoveCoroutine(Vector3 target, PlatformMoveType type)
    {
        sound.StartMoveSound();    
        while (Vector3.Distance(transform.position, target) > truncateDist)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref currVelocity, transitionTime);
            yield return null;
        }
        transform.position = target;
        IsMoving = false;
        sound.endMoveSound();
    }
}
