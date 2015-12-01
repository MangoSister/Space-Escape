using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PSMoveSharp;

/**
 * 
 * PlayStation Move Wrapper
 * Working with Move.me server on PS3.
 * 
 * Developed by Xun Zhang (lxjk001@gmail.com)
 * 2012.3.14
 * 
 **/

public class PSMoveForUnity : MonoBehaviour {
	
	
	#region system field
	
	public bool enableDefaultInGameCalibrate = true;
	/// <summary>
	/// This value will be set as initial value of PSMoveInput.onlineMode.
	/// Changing this value in runtime will NOT have effect.
	/// </summary>
	public bool onlineMode = true;
	#endregion
	
	void Awake() {
		
		PSMoveInput.onlineMode = onlineMode;
	}

	// Use this for initialization
	void Start () {
		//DontDestroyOnLoad(gameObject);
		
		
	}
	
	void Update() {		
		
		PSMoveInput.UpdateState();	
		
		if(enableDefaultInGameCalibrate) {
			if(PSMoveInput.IsConnected) {
				foreach(MoveController controller in PSMoveInput.MoveControllers) {
					if(controller.Data.GetButtonsDown(MoveButton.Move)) {
						if(controller.Data.SphereColor == new Color(0,0,0,1)) {
							controller.CalibrateAndTrack();
						}
						else {
							controller.CalibrateAndTrack(controller.Data.SphereColor);
						}
					}
					if(controller.Data.GetButtonsDown(MoveButton.Select)) {
						controller.Reset();
					}
				}
			}
		}
		
	}
	
	void OnApplicationQuit() {
		PSMoveInput.Disconnect();
	}
}
