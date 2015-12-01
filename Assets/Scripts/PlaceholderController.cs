using UnityEngine;
using System.Collections;

public class PlaceholderController : MonoBehaviour {
	
	public GameObject particle;

	// Use this for initialization
	void Start () {
	
	}

	void Update() {
		if (Input.GetButtonDown("Fire1")) {
			Debug.Log (Input.mousePosition);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				Instantiate(particle, ray.GetPoint(hit.distance), transform.rotation);
			}
		}
	}
}
