using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelTimer : MonoBehaviour
{
    public float maxLevelTime;
    public float levelTimer;
    public Text timeDisplay;
    public Gradient timeDisplayGradient;
    private bool gameOverFlag;
    // Use this for initialization
    private void Start ()
    {
        levelTimer = 0f;
        gameOverFlag = false;
    }
	
	// Update is called once per frame
	private void Update ()
    {
        if (gameOverFlag)
            return;

        levelTimer += Time.deltaTime;
        timeDisplay.text = (Mathf.FloorToInt(maxLevelTime - levelTimer)).ToString();
        timeDisplay.text += ".";
        timeDisplay.text += Mathf.FloorToInt(((maxLevelTime - levelTimer) - Mathf.Floor(maxLevelTime - levelTimer)) * 10f).ToString();
        timeDisplay.color = timeDisplayGradient.Evaluate(Mathf.Clamp01(levelTimer / maxLevelTime));
        if (levelTimer >= maxLevelTime)
        {
            //game over: lose
            //Time.timeScale = 0f;
            timeDisplay.text = "BYE";
            LevelController.Instance.fadeEffect.enabled = true;
            LevelController.Instance.fadeEffect.beginFadeOut(1f, () => { Application.LoadLevel("intro"); });
            gameOverFlag = true;
            //Debug.Log("lose");
            //Application.LoadLevel("intro");
        }
    }
}
