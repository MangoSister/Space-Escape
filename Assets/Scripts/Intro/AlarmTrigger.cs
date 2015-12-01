using UnityEngine;
using System.Collections;

public class AlarmTrigger : MonoBehaviour
{
    public Vector3 meteoriteSpawnPos;
    public Meteorite meteoritePrefab;
    public GameObject blurryPlatePrefab;
    public Player player;

    private Meteorite meteorite;
    public event Meteorite.OnHitHandler onMeteoriteHit;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Robot"))
            return;
        //other.gameObject.GetComponent<RobotSound>().PlayMajorSound(RobotSound.MajorSoundType.Scared);
        //spawn meteorite
        Debug.Log("boom");
        meteorite = (Instantiate(meteoritePrefab, meteoriteSpawnPos, Quaternion.identity)) as Meteorite;
        meteorite.onHit += OnMeteoriteHit;
        meteorite.m_bluryPlate = Instantiate(blurryPlatePrefab,player.transform.position + Vector3.forward * 0.4f, Quaternion.identity) as GameObject;       
    }

    private void Update()
    {
        if (meteorite != null)
            meteorite.m_bluryPlate.transform.position = player.transform.position + Vector3.forward * 0.4f;
    }

    private void OnMeteoriteHit()
    {
        onMeteoriteHit();
    }
}
