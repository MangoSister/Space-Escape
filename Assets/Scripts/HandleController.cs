using UnityEngine;
using System.Collections;

public class HandleController : MonoBehaviour {

    public static GridSystem gridSystem;// { get { return LevelController.Instance.gridSystem; } }

    public GameObject handle;
    public GameObject beam;
	public GameObject particle;
	public GameObject robot;//Change player to robot (modified by Yang)

	private int controllerNumber = -1;
	
	public float zOffset = 20;
	public static Vector3 handlePositionOffset = new Vector3 (0.0f, -0.5f, -1.75f);
	public float handlePositionSensitivity = 0.1f;
	private float speedThreshold = 3.0f;
	private float angledSpeedThreshold = 0.2f;
	private float reconnectDelay = 3.0f;
	private float beamMaxDist = 100.0f;

	public static Vector3 handlePos;
	public static bool beamOn;
	public static Platform activePlatform;
	private float initialTime;

	Quaternion temp = new Quaternion(0,0,0,0);

	// Use this for initialization
	void Start () {
		initialTime = Time.time;
		//beam.SetActive(false);
		beamOn = false;
	}

	void Update() {

	}

	// Update is called once per frame
	void FixedUpdate () {
		if (PSMoveInput.IsConnected) {
			MoveData moveData = new MoveData();
			if (controllerNumber != -1 && PSMoveInput.MoveControllers [controllerNumber].Connected) {
				moveData = PSMoveInput.MoveControllers [controllerNumber].Data;
			} else {
				bool found = false;
				for (int i = 0; i < PSMoveInput.MoveControllers.Length; i++) {
					if (PSMoveInput.MoveControllers[i].Connected) {
						moveData = PSMoveInput.MoveControllers[i].Data;
						controllerNumber = i;
						found = true;
						break;
					}
				}
				if (!found) {
					return;
				}
			}
				
			handlePos = moveData.HandlePosition * handlePositionSensitivity + handlePositionOffset;
			handle.transform.localPosition = new Vector3 (handlePos.x, handlePos.y, -handlePos.z);
			handle.transform.localRotation = Quaternion.Euler (-moveData.Orientation);
				
			if (moveData.ValueT > 0) {
				//If tractor beam is not lit up, light it up
				if (!beamOn) {
					Debug.Log ("Beam on!");
					//beam.SetActive(true);
					beamOn = true;
					
					//add sound effect here (modified by Yang)
					handle.GetComponent<TrackbeamSound>().PlaySound();
				}

				//If speed is above speed threshold
				if (moveData.Velocity.magnitude >= speedThreshold && activePlatform != null 
				    && activePlatform.gameObject.layer == Utility.ToLayerNumber( gridSystem.movablePfLayer)) {
					float xVel = moveData.Velocity.x;
					Vector3 oVec = activePlatform.gameObject.transform.position;
					Vector3 pVec = robot.transform.position;
					Int2 pIdx = gridSystem.ComputeIdx (new Vector2 (pVec.x, pVec.z));
					Debug.Log (pVec);
					Debug.Log (oVec);
					int dX = Mathf.RoundToInt (pVec.x) - Mathf.RoundToInt (oVec.x);
					int dZ = Mathf.RoundToInt (pVec.z) - Mathf.RoundToInt (oVec.z);
					if (dX == 0) {
						activePlatform.group.StartMoveGroup (PlatformMoveType.AxisZ, pIdx);
					} else if (dZ == 0) {
						activePlatform.group.StartMoveGroup (PlatformMoveType.AxisX, pIdx);
					} else {
						if (dX < 0) {
							if (dZ > 0) {
								if (xVel > angledSpeedThreshold) {
									activePlatform.group.StartMoveGroup (PlatformMoveType.AxisX, pIdx);
								} else if (xVel < -angledSpeedThreshold) {
									activePlatform.group.StartMoveGroup (PlatformMoveType.AxisZ, pIdx);
								}
							} else {
								if (xVel > angledSpeedThreshold) {
									activePlatform.group.StartMoveGroup (PlatformMoveType.AxisZ, pIdx);
								} else if (xVel < -angledSpeedThreshold) {
									activePlatform.group.StartMoveGroup (PlatformMoveType.AxisX, pIdx);
								}
							}
						} else {
							if (dZ > 0) {
								if (xVel > angledSpeedThreshold) {
									activePlatform.group.StartMoveGroup (PlatformMoveType.AxisZ, pIdx);
								} else if (xVel > -angledSpeedThreshold) {
									activePlatform.group.StartMoveGroup (PlatformMoveType.AxisX, pIdx);
								}
							} else {
								if (xVel > angledSpeedThreshold) {
									activePlatform.group.StartMoveGroup (PlatformMoveType.AxisX, pIdx);
								} else if (xVel > -angledSpeedThreshold) {
									activePlatform.group.StartMoveGroup (PlatformMoveType.AxisZ, pIdx);
								}
							}
						}
					}
				}
			} else {
				if (beamOn) {
					Debug.Log ("Beam off!");
					beamOn = false;
					if (activePlatform != null) {
						if (activePlatform.group != null) {
							activePlatform.group.Deactivate();
						}
						activePlatform = null;
					}
				}

			}

		}
	}

	
}
