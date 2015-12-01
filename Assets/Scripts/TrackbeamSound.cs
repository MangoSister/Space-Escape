using UnityEngine;
using System.Collections;

public class TrackbeamSound : MonoBehaviour
{
    public AudioSource trackbeamSound;

    public void PlaySound()
    {
        trackbeamSound.Play();
    }

}
