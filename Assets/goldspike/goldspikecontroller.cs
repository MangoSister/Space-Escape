using UnityEngine;
using System.Collections;

public class goldspikecontroller : MonoBehaviour {

	public AudioClip hover;
	public float speed = 3;
	private Rigidbody rb;
	private AudioSource audioSource;
	public Camera cam;

	private float pch = 1;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		audioSource = GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		
		rb.AddForce (movement * speed);
		pch = 1 + rb.velocity.magnitude;
		audioSource.pitch = pch;
		if (!audioSource.isPlaying) {
			audioSource.PlayOneShot (hover);
		}
		cam.transform.position = transform.position + new Vector3 (0, 1, -1.33f);

	}


}
