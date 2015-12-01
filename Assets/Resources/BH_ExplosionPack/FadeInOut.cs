using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

	public Texture2D m_fadeOutTexture;
	public float m_fadeSpeed = 0.4f;
	public float m_fadeTime = 5.0f;

	int m_drawDepth = -1000;
	float m_alpha = 1.0f;
	int m_fadeDir = -1;

	void OnGUI(){
		m_alpha += m_fadeDir * m_fadeSpeed * Time.deltaTime;
		m_alpha = Mathf.Clamp01 (m_alpha);

		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, m_alpha);
		GUI.depth = m_drawDepth;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), m_fadeOutTexture);
	}

	public float BeginFade(int _direction){
		m_fadeDir = _direction;
		return(m_fadeTime);
	}

	
	void CompleteFade(){
		BeginFade (-1);
	}


}
