using UnityEngine;
using System.Collections;

public class platformLight_Ctrl : MonoBehaviour {

	public GameObject m_platformLights;
	MeshRenderer m_platformLightsRenderer;

	Color onColor = new Color (0.49f, 1.0f, 0, 1.0f);
	Color offColor = new Color (0.26f, 0.26f, 0.26f, 1.0f);
	
	void Start () {
		m_platformLightsRenderer = m_platformLights.GetComponent<MeshRenderer> ();
		SetLightOn ();
	}
	
	public void SetLightOff(){
		m_platformLightsRenderer.material.SetColor ("_Color", offColor);
	}

	public void SetLightOn(){
		m_platformLightsRenderer.material.SetColor ("_Color", onColor);
	}
	

}
