using UnityEngine;
using System.Collections;

//-------------------------
// 2015.9.16 1:51 PM // LBH 
//-------------------------

public class ExplosionEffect : MonoBehaviour {

	//Streamer
	public GameObject m_streamer;
	public float 	m_deleteAfter = 0.0f;
	public float 	m_streamerUpAmount = 0.0f;
	public int 		m_streamerCountMin = 0;
	public int 		m_streamerCountMax = 0;
	public float 	m_streamerRadius = 0.0f;

	//Flare
	public Light 	m_explosionFlare;
	public float 	m_flareStartTime;
	public float 	m_flareAccel;
	public float 	m_RangeScaleUnit;
	public float 	m_MaximumRange;
			bool 	bIsDecrease = false;

	//Timer & Directions
	float 		timer = 0.0f;
	Vector3[] 	Directions = { new Vector3 (0, 0, 1), new Vector3 (0, 0, -1), new Vector3 (1, 0, 0), new Vector3 (-1, 0, 0), new Vector3 (0, 1, 0), new Vector3 (0, -1, 0) } ;
	RaycastHit 	hit;



	void Start () {
		//Streamer Init----
		if(m_streamer)
		{
			int streamerCount = Random.Range(m_streamerCountMin, m_streamerCountMax);
			Vector3 toCam = (Camera.main.transform.position - transform.position).normalized * 1.35f;
			int i = 0;
			int dirIndex = 0;
			while(i < streamerCount)
			{
				Vector3 random = Random.onUnitSphere;
				
				if(dirIndex >= Directions.Length) dirIndex = 0;
				Vector3 dir= Directions[dirIndex];
				
				Vector3 randomv = ( (dir + random + toCam) + (transform.TransformDirection(Vector3.forward) * m_streamerUpAmount) ).normalized * m_streamerRadius;
				Instantiate(m_streamer, transform.position + randomv, Quaternion.LookRotation(randomv));
				dirIndex ++;
				i++;
			}
		}

		//Flare Init----
		m_explosionFlare.enabled = false;
		m_explosionFlare.range = 0;
	}

	void Update () {

		//Basic ----
		if (timer > m_deleteAfter) {
			transform.DetachChildren ();
			//Destroy (gameObject);
		} else if (timer >= m_flareStartTime) {
			m_explosionFlare.enabled = true;
		}
		
		//Flare-----
		if (0 <= m_explosionFlare.range && m_explosionFlare.range < m_MaximumRange && !bIsDecrease) {
			m_explosionFlare.range += m_RangeScaleUnit * m_flareAccel * Time.deltaTime;

			//For Transition of status;;
		} else if ((m_MaximumRange <= m_explosionFlare.range) &&( m_explosionFlare.range <= (m_MaximumRange + m_RangeScaleUnit)) && !bIsDecrease) {
			bIsDecrease = true;
		}else if(bIsDecrease){
			m_explosionFlare.range -= m_RangeScaleUnit * m_flareAccel * 1.5f * Time.deltaTime;

			if(m_explosionFlare.range == 0 ){ // Explosion Over and then Every Object attached this Destroyed;;
				m_explosionFlare.enabled = false;
				Destroy (gameObject);
			}
		} 

		timer += Time.deltaTime;
	}


}



