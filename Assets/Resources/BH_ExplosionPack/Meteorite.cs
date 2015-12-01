using UnityEngine;
using System.Collections;

public class Meteorite : MonoBehaviour {

	public GameObject m_bluryPlate;
			MeshRenderer m_bluryPlateRenderer;
	public float m_MaximumDistance;
	public float m_MinimumDistance;
	public float m_MaximumAlpha = 0.5f;

	public float m_meteoSpeed;
	public GameObject m_meteoLight1;
	public GameObject m_meteoLight2;
	public GameObject m_meteoLight3;

	public bool m_bStartImpact = false;
		   bool m_bProcessImpact = false;
            bool m_bAfterImpact = false;
    //trigger event (modified by Yang)
    public AudioSource audioSource;
    //public AudioClip explosionSound;
    public AudioClip approachingSound;

    public delegate void OnHitHandler();
    public event OnHitHandler onHit;

	void Start () {
		m_bluryPlate.SetActive(false);
		m_bluryPlateRenderer = m_bluryPlate.GetComponent<MeshRenderer> ();
		m_meteoLight1.GetComponent<Light> ().enabled = false;
		m_meteoLight2.GetComponent<Light> ().enabled = false;
		m_meteoLight3.GetComponent<Light> ().enabled = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = approachingSound;
        audioSource.loop = false;
        audioSource.Play();

        m_bAfterImpact = false;
	}

	void Update ()
    {
	    
		if (m_bStartImpact && !m_bProcessImpact)
        {
			//Light Activate
			m_meteoLight1.GetComponent<Light> ().enabled = true;
			m_meteoLight2.GetComponent<Light> ().enabled = true;
			m_meteoLight3.GetComponent<Light> ().enabled = true;

			m_bProcessImpact = true;
			m_bluryPlate.SetActive(true);
		
		}
        else if(m_bStartImpact && m_bProcessImpact)
        {
			//Calculate Distance/ if,distance close = activate. else, none
			Vector3 dir =  (m_bluryPlate.transform.position ) - this.transform.position ;
            
            float dist = dir.magnitude;
            //Debug.Log(dist);
            if ( m_MinimumDistance < dist && dist <=  m_MaximumDistance)
            {
				//Translate Light bulbs
				//this.transform.position += dir * Time.deltaTime * m_meteoSpeed;
				dir += m_bluryPlate.transform.forward * 5.0f * -1;
				this.transform.Translate( dir.normalized * m_meteoSpeed * Time.deltaTime);

				//Change the bluryPlate alpha in order to make light blur effect
				float alpha = 1 -  (dist/100);

				if(alpha < 0){
					alpha = 0.0f;
				} else if( alpha > m_MaximumAlpha ){
					alpha  = m_MaximumAlpha;
				}
			
				Color matColor = m_bluryPlateRenderer.material.GetColor("_Color");
				matColor.a = alpha;
				m_bluryPlateRenderer.material.SetColor ("_Color", matColor);

			}
            else if( dist <= m_MinimumDistance){
                //ResetValue
                //Reset();

                //activate Blur Effect

                //disable the blury Plate
                //m_bluryPlate.SetActive(false);

                //trigger event (modified by Yang)
                if (!m_bAfterImpact)
                {
                    m_bAfterImpact = true;
                    //audioSource.Stop();
                    //audioSource.clip = explosionSound;
                    //audioSource.loop = false;
                    //audioSource.Play();
                    onHit();
                }
			}

		}


	}

	void Reset(){
		m_bStartImpact = false;
		m_bProcessImpact = false;
		m_meteoLight1.GetComponent<Light> ().enabled = false;
		m_meteoLight2.GetComponent<Light> ().enabled = false;
		m_meteoLight3.GetComponent<Light> ().enabled = false;
	}
}
