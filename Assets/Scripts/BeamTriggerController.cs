using UnityEngine;
using System.Collections;

public class BeamTriggerController : MonoBehaviour {
	
	public static GridSystem gridSystem;// { get { return LevelController.Instance.gridSystem; } }
	public GameObject particles;
	
	public static Int2 finalCoord;
	public static Int2 playerCoord;

	private bool particlesOn;

	// Use this for initialization
	void Start () {
		particlesOn = false;
	}
	
	// Update is called once per frame
	void Update () {
		particlesOn = HandleController.beamOn;
		if (!particlesOn) {
			particles.SetActive (false);
		}
	}

	void OnTriggerStay(Collider other) {
		//Debug.Log (other.gameObject.name);
		//Debug.Log (other.gameObject.layer);
		if (other.gameObject.layer == Utility.ToLayerNumber (gridSystem.movablePfLayer)) {
			if (!particlesOn) {
				HandleController.activePlatform = other.gameObject.GetComponent<Platform> ();
			}

			if (HandleController.activePlatform != null) {
				particles.transform.rotation = Quaternion.LookRotation (HandleController.activePlatform.transform.position - particles.transform.position);
				particles.SetActive (particlesOn);
			}
		}

	}


}
