using UnityEngine;
using System.Collections;

public class fadeInOut_ImageEffect : MonoBehaviour {

	public Texture2D m_TargerTex;
	
	private Material m_matFade;
	private Material m_matBlur;

	public bool m_bFadeIn = false;
	public bool m_bFadeOut = false;
	public float m_fadeSpeed;

	private float m_startFadeInOffset = 1;
	private float m_startFadeOutOffset = 0;
	private float m_startBlurAmout = 0.005f;

	public delegate void Fade_callback();
	Fade_callback m_FadeCallback;

	// Use this for initialization
	void Start () {
		m_matFade = new Material(Shader.Find("Custom/Fade"));
		m_matBlur = new Material(Shader.Find("Custom/GuassianBlur"));
	}
	
	void OnRenderImage (RenderTexture src, RenderTexture dst) {

		if (m_bFadeIn) {
			m_startFadeInOffset -= m_fadeSpeed * Time.deltaTime;
			m_startBlurAmout -= 0.05f * Time.deltaTime * m_fadeSpeed;

			float offsetPercentage = (1 - m_startFadeInOffset)/ 100;
			//m_startBlurAmout = m_startBlurAmout * offsetPercentage;

			if(m_startBlurAmout < 0){
				m_startBlurAmout = 0;
			}

			if(m_startFadeInOffset <= 0){

                if (m_FadeCallback != null)
                    m_FadeCallback();
                else
                    Debug.Log("Fade No Callback");
                this.enabled = false;

            }

		}
        else if (m_bFadeOut)
        {
			m_startFadeOutOffset += m_fadeSpeed * Time.deltaTime;

            if (m_startFadeOutOffset >= 1)
            {

                if (m_FadeCallback != null)
                    m_FadeCallback();
                else
                    Debug.Log("Fade No Callback");
                this.enabled = false;
            }
		}

		if (!m_bFadeIn && !m_bFadeOut) {
			return;
		} 
			

		RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height);

		m_matFade.SetTexture("_MainTex", src);
		m_matFade.SetTexture("_TargetTex", m_TargerTex);

		if (m_bFadeIn) {
			m_matFade.SetFloat("_Offset", m_startFadeInOffset);
		} else {
			m_matFade.SetFloat("_Offset", m_startFadeOutOffset);
		}

		Graphics.Blit(src, rt, m_matFade);
		
		m_matBlur.SetTexture ("_MainTex", dst);
		m_matBlur.SetFloat("_Offset", m_startBlurAmout);
		Graphics.Blit(rt, dst, m_matBlur);
		
		RenderTexture.ReleaseTemporary(rt);

	}

	
	void Initalize(float _fadeSpeed, Fade_callback _callback){
		this.enabled = true;
		m_FadeCallback = _callback;
		m_fadeSpeed = _fadeSpeed;
        m_startFadeInOffset = 1;
        m_startFadeOutOffset = 0;
    }

	public void beginFadeIn_WithBlur(float _fadeSpeed, float blurAmount, Fade_callback _callback){
		m_bFadeIn = true;
        m_bFadeOut = false;
		m_startBlurAmout = blurAmount;//typical: 0.03f
        Initalize (_fadeSpeed, _callback);
	}
	
	public void beginFadeOut(float _fadeSpeed, Fade_callback _callback){
		m_bFadeOut = true;
        m_bFadeIn = false;
		m_startBlurAmout = 0;
        Initalize (_fadeSpeed, _callback);
	}
	






}