using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionManager : MonoBehaviour
{
    public GameObject explosionPrefab;
    private LevelController levelController { get { return LevelController.Instance; } }
    //private 
    private List<Vector3> expLocations;

    public int expWaveNum;
    public Vector2 expWaveInterval;
    public Vector2 expLifeSpan;

    public List<AudioClip> expAudioClips;

    private float nextWaveInterval;
    private float nextWaveTimer;

    private void Start()
    {
        expLocations = new List<Vector3>();
        Debug.Assert(expWaveInterval.y > expWaveInterval.x);
        Debug.Assert(expLifeSpan.y > expLifeSpan.x);
        nextWaveInterval = Random.Range(expWaveInterval.x, expWaveInterval.y);
        nextWaveTimer = nextWaveInterval;
    }

    private void Update()
    {
        nextWaveTimer += Time.deltaTime;
        if (nextWaveTimer >= nextWaveInterval)
        {
            nextWaveTimer -= nextWaveInterval;
            nextWaveInterval = Random.Range(expWaveInterval.x, expWaveInterval.y);

            GenerateNewExpWave();
        }
    }

    private void GenerateNewExpWave()
    {
        StartCoroutine(GenNewExpWaveCoroutine());
    }

    private IEnumerator GenNewExpWaveCoroutine()
    {
        float frontierNrm = Mathf.InverseLerp
            (0, levelController.levelTimer.maxLevelTime, levelController.levelTimer.levelTimer);
        //frontierNrm = 0.5f;
        List<float> newExpNrm = new List<float>();
        while (newExpNrm.Count < expWaveNum)
        {
            float curr = Utility.GaussianRandom(frontierNrm, 0.16f);
            if (curr > frontierNrm + 0.2f || curr > 1.0f || curr < 0f)
                continue;
            else newExpNrm.Add(curr);
        }

        foreach (var percent in newExpNrm)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            if (Random.value > 0.5f)
            {
                var exp = (Instantiate(explosionPrefab,
                    new Vector3(levelController.gridSystem.transform.position.x,
                    0f, Mathf.Lerp(levelController.startZ, levelController.goalZ, percent)),
                    Quaternion.identity) as GameObject);
                var audio = exp.GetComponent<AudioSource>();
                audio.clip = expAudioClips[Random.Range(0, expAudioClips.Count)];
                audio.Play();
                //exp.lifeSpan = Random.Range(expLifeSpan.x, expLifeSpan.y);
            }
            else
            {
                var exp = (Instantiate(explosionPrefab,
                    new Vector3(levelController.gridSystem.transform.position.x + levelController.gridSystem.cellSize * levelController.gridSystem.gridSizeX,
                    0f, Mathf.Lerp(levelController.startZ, levelController.goalZ, percent)),
                    Quaternion.identity) as GameObject);
                var audio = exp.GetComponent<AudioSource>();
                audio.clip = expAudioClips[Random.Range(0, expAudioClips.Count)];
                audio.Play();
            }
        }

        yield return null;

    }

}
