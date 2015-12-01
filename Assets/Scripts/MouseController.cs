using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {
	
	private static GridSystem gridSystem { get { return LevelController.Instance.gridSystem; } }
	private Vector3 mousePosition;
	public float moveSpeed = 0.1f;
	public Camera mainCam;
	public GameObject handle, beam;
	public GameObject particles;
	public GameObject spotlight;
	
	public float beamMaxDist = 5.0f;
	public float beamWidth = 1.0f;
	public float beamNoise = 1.0f;
	public float beamNoiseDistance = 0.1f;
	public Color beamColor = Color.blue;
	public ParticleSystem endEffect;

	
	//LineRenderer lineRenderer;
	float beamLength;
	Vector3[] positions;
	Transform endEffectTransform;
	Transform handleTransform;
	Vector3 offset;
	private bool particlesOn;

	public Platform activePlatform;

	// Use this for initialization
	void Start () {
		particles.SetActive (false);
		particlesOn = false;
		//lineRenderer = beam.gameObject.GetComponent<LineRenderer> ();
		//lineRenderer.SetWidth (beamWidth, beamWidth);
		handleTransform = transform;
		offset = new Vector3 (0, 0, 0);
		endEffect = GetComponentInChildren<ParticleSystem> ();
		if (endEffect) {
			endEffectTransform = endEffect.transform;
		}
	}

	void Update() {

		particlesOn = Input.GetButton ("Fire1");

		Ray ray = mainCam.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, beamMaxDist)) {
			handle.transform.rotation = Quaternion.LookRotation (hit.point - handle.transform.position);

			if (Physics.Raycast (ray, out hit, beamMaxDist, gridSystem.movablePfLayer.value)) {
				activePlatform = hit.collider.gameObject.GetComponent<Platform> ();
				//particles.SetActive (particlesOn);
			} else {
				//particles.SetActive (false);
			}
		} else {
			//particles.SetActive(false);
		}
	}

	void renderBeam(Transform hitTransform) {
		updateBeamLength (hitTransform);
		
		//lineRenderer.SetColors (beamColor, beamColor);
		for (int i = 0; i < (int)(beamLength * 10); i++) {
			offset.x = handleTransform.position.x + i * beamNoiseDistance * handleTransform.forward.x + Random.Range (-beamNoise, beamNoise);
			offset.z = handleTransform.position.z + i * beamNoiseDistance * handleTransform.forward.z + Random.Range (-beamNoise, beamNoise);
			positions[i] = offset;
			positions[0] = handleTransform.position;
			
			//lineRenderer.SetPosition(i, positions[i]);
		}
	}
	
	void updateBeamLength(Transform hitTransform) {
		float hitDistance = (hitTransform.position - handle.transform.position).magnitude;
		beamLength = hitDistance;
		Debug.Log ("Beam: " + beamLength.ToString());
		positions = new Vector3[(int) (beamLength * 10)];

		//lineRenderer.SetVertexCount((int) (beamLength * 10));
	
		/*
		if (endEffect) {
			if (endEffect.isPlaying) {
				endEffect.Stop ();
			}
		}
		*/
		
	}



}
