using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class Config : MonoBehaviour {
	
	public static string ipAddress = "128.2.237.66";
	public static string port = "7899";
	private float timeout = 2.0f;
	public static string filename = "config.ini";
	private float initialTime;
	void Start() {
		loadFile("config.ini");
		initialTime = Time.time;
		PSMoveInput.Connect(ipAddress, int.Parse(port));
		Debug.Log ("Connecting... Address: " + ipAddress + ", Port: " + port);
	}


	void loadFile (string filename) {
		if (!File.Exists (filename)) {
			File.CreateText(filename);
			return;
		}

		try {
			string line;  
			StreamReader sReader = new StreamReader(filename, Encoding.Default);
			do
			{
				line = sReader.ReadLine();
				if (line != null)
				{
					if (line.Contains("#")) {

					} else {
						string[] data = line.Split(':');
						if (data.Length == 2) {
							switch(data[0]) {
							case "Offset":
								string[] coords = data[1].Trim ().Split(',');
								HandleController.handlePositionOffset = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
								Debug.Log ("Offset is: " + HandleController.handlePositionOffset);
								break;
							case "IP Address":
								Debug.Log ("IP Address: " + data[1]);
								ipAddress = data[1].Trim();
								break;
							case "Port":
								Debug.Log ("Port: " + data[1]);
								port = data[1].Trim ();
								break;
							default:
								break;
							}
						}
					}
				}
			}
			while (line != null);
			sReader.Close();
			return;
		} catch (Exception e) {

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
		
	}

	void Update() {
		if (PSMoveInput.IsConnected) {
			//Application.LoadLevel("intro");
		} else {
			if (initialTime + timeout < Time.time) {
				Debug.Log ("Unsuccessful...reconnecting..");
				initialTime = Time.time;
				PSMoveInput.Connect(ipAddress, int.Parse(port));
			}
		}
	}

}
