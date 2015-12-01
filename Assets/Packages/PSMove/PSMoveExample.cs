using UnityEngine;
using System.Collections;
using System;

public class PSMoveExample : MonoBehaviour {
	
	
	public string ipAddress = "128.2.237.237";
	public string port = "7899";
		
	public GameObject gem, handle;
	
	public bool isMirror = true;
	
	public float zOffset = 20;
	Quaternion temp = new Quaternion(0,0,0,0);
	
	
	#region GUI Variables
	string cameraStr = "Camera Switch On";
	string rStr = "0", gStr = "0", bStr = "0";
	string rumbleStr = "0";
	#endregion
	
	
	
	// Use this for initialization
	void Start () {
		
	}
		
	
	void Update() {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		if(PSMoveInput.IsConnected && PSMoveInput.MoveControllers[0].Connected) {
		
			Vector3 gemPos, handlePos;
			MoveData moveData = PSMoveInput.MoveControllers[0].Data;
			gemPos = moveData.Position;
			handlePos = moveData.HandlePosition;
			if(isMirror) {
				gem.transform.localPosition = gemPos;
				handle.transform.localPosition = handlePos;
				handle.transform.localRotation = Quaternion.Euler(moveData.Orientation);
			}
			else {
				gemPos.z = -gemPos.z + zOffset;
				handlePos.z = -handlePos.z + zOffset;
				gem.transform.localPosition = gemPos;
				handle.transform.localPosition = handlePos;
				handle.transform.localRotation = Quaternion.LookRotation(gemPos - handlePos);
				handle.transform.Rotate(new Vector3(0,0,moveData.Orientation.z));
	
			/* using quaternion rotation directly
			 * the rotations on the x and y axes are inverted - i.e. left shows up as right, and right shows up as left. This code fixes this in case 
			 * the object you are using is facing away from the screen. Comment out this code if you do want an inversion along these axes
			 * 
			 * Add by Karthik Krishnamurthy*/
				
				temp = moveData.QOrientation;
				temp.x = -moveData.QOrientation.x;
				temp.y = -moveData.QOrientation.y;
				handle.transform.localRotation = temp;
			
			}
		}
	}
	
	void OnGUI() {
		
		if(!PSMoveInput.IsConnected) {
			
			GUI.Label(new Rect(20, 45, 30, 35), "IP:");
			ipAddress = GUI.TextField(new Rect(60, 45, 120, 25), ipAddress);
			
			GUI.Label(new Rect(190, 45, 30, 35), "port:");
			port = GUI.TextField(new Rect(230, 45, 50, 25), port);
			
			if(GUI.Button(new Rect(300, 40, 100, 35), "Connect")) {
				PSMoveInput.Connect(ipAddress, int.Parse(port));
			}
			
		}
		else {
			
			
			if(GUI.Button(new Rect(20, 40, 100, 35), "Disconnect"))  {
				PSMoveInput.Disconnect();
				Reset();
			}
			
			
			GUI.Label(new Rect(10, 10, 150, 100),  "PS Move count : " + PSMoveInput.MoveCount);
			GUI.Label(new Rect(140, 10, 150, 100),  "PS Nav count : " + PSMoveInput.NavCount);
			
			//camera stream on/off
			if(GUI.Button(new Rect(5, 80, 130, 35), cameraStr)) {
				if(cameraStr == "Camera Switch On") {
					PSMoveInput.CameraFrameResume();
					cameraStr = "Camera Switch Off";
				}
				else {
					PSMoveInput.CameraFramePause();
					cameraStr = "Camera Switch On";
				}
			}
			
			//color and rumble for move number 0
			if(PSMoveInput.MoveControllers[0].Connected) {
				//Set Color and Track
				GUI.Label(new Rect(300, 50, 200,20), "R,G,B are floats that fall in 0 ~ 1");
				GUI.Label(new Rect(260, 20, 20, 20), "R");
				rStr = GUI.TextField(new Rect(280, 20, 60, 20), rStr);
				GUI.Label(new Rect(350, 20, 20, 20), "G");
				gStr = GUI.TextField(new Rect(370, 20, 60, 20), gStr);
				GUI.Label(new Rect(440, 20, 20, 20), "B");
				bStr = GUI.TextField(new Rect(460, 20, 60, 20), bStr);
				if(GUI.Button(new Rect(550, 30, 160, 35), "SetColorAndTrack")) {
					try {
						float r = float.Parse(rStr);
						float g = float.Parse(gStr);
						float b = float.Parse(bStr);
						PSMoveInput.MoveControllers[0].SetColorAndTrack(new Color(r,g,b));
					}
					catch(Exception e) {
						Debug.Log("input problem: " + e.Message);
					}
				}
				//Rumble
				rumbleStr = GUI.TextField(new Rect(805, 20, 40, 20), rumbleStr);
				GUI.Label(new Rect(800, 50, 200,20), "0 ~ 19");
				if(GUI.Button(new Rect(870, 30, 100, 35), "Rumble")) {
					try {
						int rumbleValue = int.Parse(rumbleStr);
						PSMoveInput.MoveControllers[0].SetRumble(rumbleValue);
					}
					catch(Exception e) {
						Debug.Log("input problem: " + e.Message);
					}
				}
			}
			
			//move controller information
			for(int i=0; i<PSMoveInput.MAX_MOVE_NUM; i++)
			{
				MoveController moveController = PSMoveInput.MoveControllers[i];
				if(moveController.Connected) {
					MoveData moveData = moveController.Data;
					string display = "PS Move #" + i + 
						"\nPosition:\t\t"+moveData.Position + 
						"\nVelocity:\t\t"+moveData.Velocity + 
						"\nAcceleration:\t\t"+moveData.Acceleration + 
						"\nOrientation:\t\t"+moveData.Orientation + 
						"\nAngular Velocity:\t\t"+moveData.AngularVelocity + 
						"\nAngular Acceleration:\t\t"+moveData.AngularAcceleration + 
						"\nHandle Position:\t\t"+moveData.HandlePosition + 
						"\nHandle Velocity:\t\t"+moveData.HandleVelocity + 
						"\nHandle Acceleration:\t\t"+moveData.HandleAcceleration +
						"\n" +
						"\nTrigger Value:\t\t" + moveData.ValueT +
						"\nButtons:\t\t" + moveData.Buttons +
						"\nSphere Color:\t\t" + moveData.SphereColor +
						"\nIs Tracking:\t\t" + moveData.IsTracking +
						"\nTracking Hue:\t\t" + moveData.TrackingHue;
					GUI.Label(new Rect( 10 + 650 * (i/2), 120+310*(i%2), 300, 400),   display);
				}
			}
			for(int j = 0; j < PSMoveInput.MAX_NAV_NUM; j++) {
				NavController navController = PSMoveInput.NavControllers[j];
				if(navController.Connected) {	
					NavData navData = navController.Data;
					string navDisplay = "PS Nav #" + j + 
						"\nAnalog :\t\t" + navData.ValueAnalog +
						"\nL2 Value:\t\t" + navData.ValueL2 +
						"\nButtons:\t\t" + navData.Buttons;
					GUI.Label(new Rect(400, 100 + 95 * j, 150, 95),   navDisplay);
				}
			}
		}
		
		
	}
	
	private void Reset() {
		cameraStr = "Camera Switch On";
		rStr = "0"; 
		gStr = "0"; 
		bStr = "0";
		rumbleStr = "0";
	}
}
