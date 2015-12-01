using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PSMoveSharp;

public static class PSMoveInput {

	#region const num
	
	public const int MAX_MOVE_NUM = 4;
	public const int MAX_NAV_NUM = 7;
	
	public const uint PICK_FOR_ME = (4<<24);
    public const uint DONT_TRACK = (2<<24);
	
	#endregion
	
	/// <summary>
	/// If it is false, the Connect function will NOT connect to PS3.
	/// This prevents possible interference with other games testing on Move.me.
	/// You can also use this value to determine whether to activate your hot keys or simulator.
	/// </summary>
	public static bool onlineMode = true;
	
	
	public static MoveController[] MoveControllers {get; private set;}
	public static NavController[] NavControllers {get; private set;}
		
	/// <summary>
	/// It represents the complete state information of Move.me Server and all Move controllers. 
	/// Use it when you need additional information which is not provided by PSMoveWrapper.
	/// </summary>
	public static PSMoveSharpState State {get; private set;}
	public static PSMoveSharpCameraFrameState CameraFrameState {get; private set;}
	
	/// <summary>
	/// whether the program connected to Move.me on PS3.
	/// </summary>
	public static bool IsConnected {get; private set;}
	/// <summary>
	/// whether receiving camera image stream.
	/// </summary>
	public static bool IsCameraResume {get; private set;}
	
	public static int MoveCount {
		get {
			int count = 0;
			foreach(MoveController controller in MoveControllers) {
				if(controller.Connected) ++count;
			}
			return count;
		}
	}
	
	public static int NavCount {
		get {
			int count = 0;
			foreach(NavController controller in NavControllers) {
				if(controller.Connected) ++count;
			}
			return count;
		}
	}
	
	
	internal static PSMoveTimer MoveInternalTimer {
		get {
			if(_psMoveTimer == null) {
				_psMoveTimer = GameObject.FindObjectOfType(typeof(PSMoveTimer)) as PSMoveTimer;
				if(_psMoveTimer == null) {
					GameObject go = new GameObject("PSMoveTimer");
					_psMoveTimer = go.AddComponent<PSMoveTimer>();
				}
			}
			return _psMoveTimer;
		}
	}
	
	
		
	private static Texture2D imageTex;
	private static List<Color32> finalImage;
	
	private static PSMoveTimer _psMoveTimer = null;
	
	
	static PSMoveInput() {
		MoveControllers = new MoveController[MAX_MOVE_NUM];
		NavControllers = new NavController[MAX_NAV_NUM];
		
		for(int i = 0; i < MAX_MOVE_NUM; ++i) {
			MoveControllers[i] = new MoveController(i);
		}
		for(int i = 0; i < MAX_NAV_NUM; ++i) {
			NavControllers[i] = new NavController(i);
		}
		
		imageTex =  new Texture2D(0,0);
		finalImage = new List<Color32>();
	}	
	
	
	public static void Connect(string address, int port) {
		if(!onlineMode) {
			Debug.LogWarning("onlineMode == false, running in offline mode!");
			return;
		}
		IsConnected = PSMoveNetwork.client_connect(address, port);
		Debug.Log("Connect");		
	}
	
	/// <summary>
	/// Pause camera stream; Reset all move controllers; Disconnect;
	/// If you just want to disconnect, call <c>Disconnect(false)</c>
	/// </summary>
	public static void Disconnect() {
		Disconnect(true);
	}
	
	public static void Disconnect(bool isCleanUp) {
		if(isCleanUp) {
			CameraFramePause();
			ResetAll();
		}
		PSMoveNetwork.client_disconnect();
		IsConnected = false;
		Debug.Log("Disconnect");
	}
	
	/// <summary>
	/// Resume camera stream and set slice num to 8.
	/// Camera stream is initially paused.
	/// </summary>
	public static void CameraFrameResume() {
		CameraFrameResume(8);
	}
	
	/// <summary>
	/// Resume camera stream and set slice num.
	/// Camera stream is initially paused.
	/// </summary>
	/// <param name="sliceNum">
	/// A <see cref="System.Int32"/>, range from 1 to 8
	/// </param>
	public static void CameraFrameResume(int sliceNum) {
		if(!IsConnected) {
			return;
		}
		sliceNum = Mathf.Clamp(sliceNum, 1, 8);
		PSMoveNetwork.moveClient.CameraFrameResume();
		IsCameraResume = true;
		SetCameraFrameSlices(sliceNum);
	}
	
	/// <summary>
	/// Set slice number
	/// </summary>
	/// <param name="sliceNum">
	/// A <see cref="System.Int32"/>, range from 1 to 8
	/// </param>
	public static void SetCameraFrameSlices(int sliceNum) {
		if(!IsConnected) {
			return;
		}
		PSMoveNetwork.moveClient.CameraFrameSetNumSlices((uint)sliceNum);
		PSMoveNetwork.moveClient.SetNumImageSlices(sliceNum);
	}
	
	public static void CameraFramePause() {
		if(!IsConnected) {
			return;
		}
		PSMoveNetwork.moveClient.CameraFramePause();
		IsCameraResume = false;
	}
	
	/// <summary>
	/// Set all PS Moves with default color and track with corresponding hue. 
	/// Use this method when you do not care about the color of all PS Moves.
	/// </summary>
	public static void TrackAll() {
		if(!IsConnected) {
			return;
		}
		if (PSMoveNetwork.moveClient != null)
        {
            PSMoveNetwork.moveClient.TrackAllHues();
        }
	}
	
	public static int[] GetTrackingHues() {
		int[] hues = new int[MAX_MOVE_NUM];
		for(int i = 0; i < MAX_MOVE_NUM; ++i) {
			hues[i] = MoveControllers[i].Data.TrackingHue;
		}
		return hues;
	}
	
	public static void ResetAll () {
		foreach(MoveController controller in MoveControllers) {
			controller.Reset();
		}
	}
	
	/// <summary>
	/// Get camera image.
	/// The image should be 640x480, please check the length before using it.
	/// Also I know it is a weird issue, but after Move.me started, please calibrate at least once to make sure the image stream working smoothly. 
	/// Once you have done this, it will work fine.
	/// </summary>
	/// <returns>
	/// A <see cref="Color32[]"/>, null if not connected or camera stream is paused. 
	/// </returns>
	public static Color32[] GetCameraImage() {
		if(!IsConnected || !IsCameraResume) {
			return null;
		}
		PSMoveSharpState dummyState = new PSMoveSharpState();
		List<byte[]> image = CameraFrameState.GetCameraFrameAndState(ref dummyState);
		finalImage.Clear();
		
		foreach(byte[] slice in image) {
			imageTex.LoadImage(slice);
			finalImage.AddRange(imageTex.GetPixels32());
		}
		return finalImage.ToArray();
	}
	
	public static void UpdateState() {
						
		if (!IsConnected || PSMoveNetwork.moveClient == null)
        {
            return;
        }
		

        State = PSMoveNetwork.moveClient.GetLatestState();
		CameraFrameState = PSMoveNetwork.moveClient.GetLatestCameraFrameState();
		foreach(MoveController controller in MoveControllers) {
			controller.Update(State);
		}
		foreach(NavController controller in NavControllers) {
			controller.Update(State);
		}
	}	
	
}
