using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	public GameObject newGameButton;
	public GameObject exitButton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startGame() {
			Application.LoadLevel ("main");
	} 

	public void exitGame () {
		Application.Quit ();
	}

}
