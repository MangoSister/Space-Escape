using UnityEngine;
using System.Collections;

public class PSEyeImageDisplay : MonoBehaviour {
	
	private Texture2D tex;
	
	
	// Use this for initialization
	void Start () {
		tex = new Texture2D(640,480,TextureFormat.ARGB32,false);
		GetComponent<Renderer>().material.mainTexture = tex;
	}
	
	// Update is called once per frame
	void Update () {
		Color32[] image = PSMoveInput.GetCameraImage();
		if(image != null && image.Length == 640*480) {
			tex.SetPixels32(image);
			tex.Apply(false);
		}
	}
	
}