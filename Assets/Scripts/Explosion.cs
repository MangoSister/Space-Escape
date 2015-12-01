using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    public float lifeSpan;
    private float lifeTimer;

    public AudioSource audioSource;

    private void Start()
    {
        lifeTimer = 0f;
        audioSource.Play();
    }
    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeSpan)
            Destroy(gameObject);
        audioSource.volume = 1f - Mathf.InverseLerp(0, lifeSpan, lifeTimer);
    }
}
