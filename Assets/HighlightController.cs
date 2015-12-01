using UnityEngine;
using System.Collections;

public class HighlightController : MonoBehaviour {

	public float yScale = 0.05f;

	float count = 0;
	float step = 0.02f;
	float expansion = 0.1f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		count += step;
		pulse ();
		//StartCoroutine (pulse());
	}

	private void pulse() {
		float scale = 1 + Mathf.Abs(expansion * Mathf.Sin (count));
		transform.localScale = new Vector3(scale, yScale, scale);
	}

	/*
	private IEnumerator pulse() {
		Vector3 currVel = new Vector3 (0, 0, 0);
		Debug.Log (transform.localScale);
		if (tick) {
			while (transform.localScale.x > 1.01f) {
				transform.localScale = Vector3.SmoothDamp (transform.localScale, new Vector3 (1f, 0.05f, 1), ref currVel, 25f * Time.deltaTime);
				yield return null;
			}
			tick = false;
		} else {
			while (transform.localScale.x < 1.09f) {
				transform.localScale = Vector3.SmoothDamp (transform.localScale, new Vector3 (1.1f, 0.05f, 1.1f), ref currVel, 25f * Time.deltaTime);
				yield return null;
			}
			tick = true;
		}
	}
		*/
}
