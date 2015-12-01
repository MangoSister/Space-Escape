using UnityEngine;
using System.Collections;

public class PlatformSound : MonoBehaviour
{
    public AudioSource moveSoundHead;
    public AudioSource moveSoundBody;
    public AudioSource moveSoundEnd;

    public void StartMoveSound()
    {
        StartCoroutine(StartMoveSoundCoroutine());
    }

    private IEnumerator StartMoveSoundCoroutine()
    {
        moveSoundHead.Play();
        while (moveSoundHead.isPlaying)
            yield return null;
        moveSoundBody.Play();
    }

    public void endMoveSound()
    {
        if (moveSoundBody.isPlaying)
            moveSoundBody.Stop();
        moveSoundEnd.Play();
    }
}
