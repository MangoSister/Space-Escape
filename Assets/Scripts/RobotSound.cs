using UnityEngine;
using System.Collections;

public class RobotSound : MonoBehaviour {

    public AudioSource audioMajor;
    public AudioClip walkHead;
    public AudioClip walkBody;
    public AudioClip walkTail;
    public AudioClip scared;
    public AudioClip idle;
    public AudioClip happy;
    public MajorSoundType currMajorType;

    public AudioSource audioOther;
    public AudioClip talk;
    public AudioClip wave;
    public AudioClip pointing;

    public void PlayMajorSound(MajorSoundType type)
    {
        StartCoroutine(PlayMajorSoundCoroutine(type));
    }

    public void PlayOtherSound(OtherSoundType type)
    {
        StartCoroutine(PlayOtherSoundCoroutine(type));
    }

    private IEnumerator PlayMajorSoundCoroutine(MajorSoundType type)
    {
        if (currMajorType == MajorSoundType.Walk &&
            audioMajor.isPlaying)
        {
            audioMajor.clip = walkTail;
            audioMajor.loop = false;
            audioMajor.volume = 0.3f;
            audioMajor.Play();
            yield return new WaitForSeconds(walkTail.length);
        }
        currMajorType = type;
        audioMajor.Stop();
        switch (type)
        {
            case MajorSoundType.Walk: yield return StartCoroutine(StartMoveSoundCoroutine()); break;
            case MajorSoundType.Idle:
                {
                    audioMajor.clip = idle;
                    audioMajor.loop = true;
                    audioMajor.volume = 1f;
                    audioMajor.Play();
                    break;
                }
            case MajorSoundType.Scared:
                {
                    audioMajor.clip = scared;
                    audioMajor.loop = true;
                    audioMajor.volume = 0.1f;
                    audioMajor.Play();
                    break;
                }
            case MajorSoundType.Happy:
                {
                    audioMajor.clip = happy;
                    audioMajor.loop = true;
                    audioMajor.volume = 1f;
                    audioMajor.Play();
                    break;
                }
            default: break;
        }
    }

    private IEnumerator PlayOtherSoundCoroutine(OtherSoundType type)
    {
        audioOther.Stop();
        switch (type)
        {
            case OtherSoundType.Talk:
                {
                    audioOther.clip = talk;
                    audioOther.loop = false;
                    audioOther.volume = 1f;
                    audioOther.Play();
                    break;
                }
            case OtherSoundType.Wave:
                {
                    audioOther.clip = wave;
                    audioOther.loop = true;
                    audioOther.volume = 1f;
                    audioOther.Play();
                    break;
                }
            case OtherSoundType.Point:
                {
                    audioOther.clip = pointing;
                    audioOther.loop = true;
                    audioOther.volume = 1f;
                    audioOther.Play();
                    break;
                }
            default:break;
        }
        yield return null;
    }

    private IEnumerator StartMoveSoundCoroutine()
    {
        audioMajor.clip = walkHead;
        audioMajor.loop = false;
        audioMajor.volume = 0.3f;
        audioMajor.Play();
        //Debug.Log("asdf");
        yield return new WaitForSeconds(walkHead.length);

        audioMajor.clip = walkBody;
        audioMajor.loop = true;
        audioMajor.volume = 0.3f;
        audioMajor.Play();
    }

    public enum MajorSoundType
    {
        None, Idle, Walk, Scared, Happy
    }

    public enum OtherSoundType
    {
        None, Talk, Wave, Point
    }
}
