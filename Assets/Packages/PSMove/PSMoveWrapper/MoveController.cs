using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using PSMoveSharp;

public class MoveController {
	public bool Connected {get; private set;}
	public int Num {get; private set;}
	
	
	#region rumble field
	public int RumbleLevel {get; private set;}
	#endregion
	
	public MoveData Data {
		get {
			if(_dirty) {
				_data.Update(gemState, sphereState, imageState);
				_dirty = false;
			}
			return _data;
		}
	}
	
	private MoveData _data;
	private bool _dirty;
	
	private PSMoveSharpGemState gemState;
	private PSMoveSharpSphereState sphereState;
	private PSMoveSharpImageState imageState;
	
	public MoveController(int num) {
		_data = new MoveData();
		_dirty = true;
		gemState = new PSMoveSharpGemState();
		sphereState = new PSMoveSharpSphereState();
		imageState = new PSMoveSharpImageState();
		Connected = false;
		Num = num;
	}
	
	public void Update(PSMoveSharpState state) {
		gemState = state.gemStates[Num];
		sphereState = state.sphereStates[Num];
		imageState = state.imageStates[Num];
		
		Connected = (state.gemStatus[Num].connected == 1);
		
		_dirty = true;
	}
	
	/// <summary>
	/// Calibrate the PS Move. The ball will NOT glow after calibration. 
	/// You need to call any one of the "SetColor" methods to make it glow, and the "Track" methods to track.
	/// </summary>
	public void Calibrate() {
		if(!PSMoveInput.IsConnected) {
			return;
		}
		if (PSMoveNetwork.moveClient != null)
		{
		    PSMoveNetwork.moveClient.CalibrateController(Num);
		}
	}
	
	/// <summary>
	/// Let PS3 pick color and track the selected PS Move. 
	/// Use this method when you do not care about the color of the ball.
	/// </summary>
	public void AutoTrack() {
		if(!PSMoveInput.IsConnected) {
			return;
		}
		int[] hues = PSMoveInput.GetTrackingHues();
		hues[Num] = Convert.ToInt32(PSMoveInput.PICK_FOR_ME);
		PSMoveNetwork.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestTrackHues, 
		                                     Convert.ToUInt32(hues[0]),
		                                     Convert.ToUInt32(hues[1]), 
		                                     Convert.ToUInt32(hues[2]), 
		                                     Convert.ToUInt32(hues[3]));
	}
	
	/// <summary>
	/// Set the color of PS Move's ball.
	/// The PS Eye camera will NOT automatically track after calling this method. 
	/// If you change the color of a tracking PS Move, the tracking will be lost. 
	/// You need to call any one of the "Track" methods to track. 
	/// The minimum step for color is 0.2f for any RGB value, the alpha value is not used.
	/// </summary>
	/// <param name="color">
	/// A <see cref="Color"/>
	/// </param>
	public void SetColor(Color color) {
		if(!PSMoveInput.IsConnected) {
			return;
		}
		float r = color.r;
		float g = color.g;
		float b = color.b;
		PSMoveNetwork.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestForceRGB, Convert.ToUInt32(Num), 
		                                     r, g, b);
	}
	
	/// <summary>
	/// Set the tracking hue of PS Move. 
	/// The hue should fit the ball's color to enable tracking.
	/// </summary>
	/// <param name="hue">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void SetTrackingHue(int hue) {
		if(!PSMoveInput.IsConnected) {
			return;
		}
		int[] hues = PSMoveInput.GetTrackingHues();
		hues[Num] = hue;
		PSMoveNetwork.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestTrackHues, 
		                                     Convert.ToUInt32(hues[0]),
		                                     Convert.ToUInt32(hues[1]), 
		                                     Convert.ToUInt32(hues[2]), 
		                                     Convert.ToUInt32(hues[3]));
	}
	
	
	public void SetColorAndTrack(Color color) {
		SetColor(color);
		SetTrackingHue(PSMoveUtil.GetHueFromColor(color));
	}
	
	/// <summary>
	/// the same as <c>CalibrateAndTrack(num, 0.8f)</c>
	/// </summary>
	public void CalibrateAndTrack() {
		CalibrateAndTrack(0.8f);
	}
	
	/// <summary>
	/// the same as <c>CalibrateAndTrack(num, color, 0.8f)</c>
	/// </summary>
	/// <param name="color">
	/// A <see cref="Color"/>
	/// </param>
	public void CalibrateAndTrack(Color color) {
		CalibrateAndTrack(color, 0.8f);
	}
	
	/// <summary>
	/// The combination of "Calibrate" and "AutoTrack". 
	/// Since calibration takes time, tracking should be delayed a certain amount of seconds. 
	/// 0.8f seems appropriate after some tests.
	/// </summary>
	/// <param name="time">
	/// A <see cref="System.Single"/>, delay time for tracking after calibration.
	/// </param>
	public void CalibrateAndTrack(float time) {
		Calibrate();
		PSMoveInput.MoveInternalTimer.AddTimer(time, DelayTrack, null);
	}	
	
	/// <summary>
	/// The combination of "Calibrate" and "SetColorAndTrack".
	/// </summary>
	/// <param name="color">
	/// A <see cref="Color"/>, the color to set and to track
	/// </param>
	/// <param name="time">
	/// A <see cref="System.Single"/>, delay time for tracking after calibration.
	/// </param>
	public void CalibrateAndTrack(Color color, float time) {
		Calibrate();
		PSMoveInput.MoveInternalTimer.AddTimer(time, DelayTrack, color);
	}
	
			
	private void DelayTrack(object param) {
		if(param == null) {
			AutoTrack();
		}
		else {
			SetColorAndTrack((Color)param);
		}
	}
		
	/// <summary>
	/// Reset move controller. 
	/// The ball will not glow and it need to re-calibrate.
	/// </summary>
	public void Reset() {
		if(!PSMoveInput.IsConnected) {
			return;
		}
		PSMoveNetwork.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestControllerReset, Convert.ToUInt32(Num));
	}
	
	/// <summary>
	/// Set rumble level. 0 -> not rumble. 19 -> max rumble.
	/// </summary>
	/// <param name="level">
	/// A <see cref="System.Int32"/>, rumble level, 0-19.
	/// </param>
	public void SetRumble(int level) {
		if(!PSMoveInput.IsConnected) {
			return;
		}
		level = Mathf.Clamp(level,0,19);
		if(RumbleLevel == level) {
			return;
		}
		RumbleLevel = level;
		//map to actual rumble scale. 0 -> 0; 1-19 -> 70-250.
		if(level!=0) {
			level = level*10 + 60;
		}
		Rumble(level);
	}
	
	/// <summary>
	/// send rumble request.
	/// The minimum step of the scale is 10. 
	/// According to the test, 0 is no rumble, 70 is minimum rumble and 250 is maximum rumble. 
	/// Scale 10-60 will not affect anything.
	/// </summary>
	/// <param name="rumbleValue">
	/// A <see cref="System.Int32"/>, 0-255
	/// </param>
	private void Rumble(int rumbleValue) {
		//rumbleValue = Mathf.Clamp(rumbleValue, 0, 255);
		PSMoveNetwork.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestSetRumble, Convert.ToUInt32(Num), Convert.ToUInt32(rumbleValue));
	}
	
}

public class MoveData {
	
	#region gem state field
	public Vector3 Position {get; private set;}
	public Vector3 Velocity {get; private set;}
	public Vector3 Acceleration {get; private set;}
	
	public Vector3 Orientation {get; private set;}
	public Quaternion QOrientation {get; private set;}
	public Vector3 AngularVelocity {get; private set;}
	public Vector3 AngularAcceleration {get; private set;}
	
	public Vector3 HandlePosition {get; private set;}
	public Vector3 HandleVelocity {get; private set;}
	public Vector3 HandleAcceleration {get; private set;}
	
	public MoveButton Buttons {get; private set;}
	public MoveButton PrevButtons {get; private set;}
	
	/// <summary>
	/// range from 0 to 255.
	/// </summary>
	public int ValueT {get; private set;}
	
	/// <summary>
	/// It is used for <c>WasPressed()</c> and <c>WasReleased()</c>.
	/// It represents whether the button TRIGGER can be treated as "pressed".
	/// The default value is 250. This field is modifiable.
	/// </summary>
	public int thresholdT = 250;
	#endregion
	
	#region sphere state field
	public Color SphereColor {get; private set;}
	public bool IsTracking {get; private set;}
	public int TrackingHue {get; private set;}
	#endregion
	
	#region image state field
	public Vector2 SpherePixelPosition {get; private set;}
	public float SpherePixelRadius {get; private set;}
	public Vector2 SphereProjectionPosition {get; private set;}
	public float SphereDistance {get; private set;}
	public bool SphereVisible {get; private set;}
	public bool SphereRadiusValid {get; private set;}
	#endregion
	
	public void Update(PSMoveSharpGemState gemState, 
		PSMoveSharpSphereState sphereState, 
		PSMoveSharpImageState imageState) {
		
		// gem state
		Position = PSMoveUtil.Float4ToVector3(gemState.pos)/100;
		Velocity = PSMoveUtil.Float4ToVector3(gemState.vel)/100;
		Acceleration = PSMoveUtil.Float4ToVector3(gemState.accel)/100;
		
		QOrientation = PSMoveUtil.Float4ToQuaternion(gemState.quat);
		Orientation = QOrientation.eulerAngles;
		AngularVelocity = PSMoveUtil.Float4ToVector3(gemState.angvel) * Mathf.Rad2Deg;
		AngularAcceleration = PSMoveUtil.Float4ToVector3(gemState.angaccel) * Mathf.Rad2Deg;
		
		HandlePosition = PSMoveUtil.Float4ToVector3(gemState.handle_pos)/100;
		HandleVelocity = PSMoveUtil.Float4ToVector3(gemState.handle_vel)/100;
		HandleAcceleration = PSMoveUtil.Float4ToVector3(gemState.handle_accel)/100;
		
		PrevButtons = Buttons;
		Buttons = MoveButton.None;
		if((gemState.pad.digitalbuttons & PSMoveSharpConstants.ctrlSquare) != 0)
			Buttons |= MoveButton.Square;
		if((gemState.pad.digitalbuttons & PSMoveSharpConstants.ctrlCross) != 0)
			Buttons |= MoveButton.Cross;
		if((gemState.pad.digitalbuttons & PSMoveSharpConstants.ctrlCircle) != 0)
			Buttons |= MoveButton.Circle;
		if((gemState.pad.digitalbuttons & PSMoveSharpConstants.ctrlTriangle) != 0)
			Buttons |= MoveButton.Triangle;
		if((gemState.pad.digitalbuttons & PSMoveSharpConstants.ctrlTick) != 0)
			Buttons |= MoveButton.Move;
		if((gemState.pad.digitalbuttons & PSMoveSharpConstants.ctrlStart) != 0)
			Buttons |= MoveButton.Start;
		if((gemState.pad.digitalbuttons & PSMoveSharpConstants.ctrlSelect) != 0)
			Buttons |= MoveButton.Select;
		
	    ValueT =  gemState.pad.analog_trigger;
		
		if(ValueT >= thresholdT)
			Buttons |= MoveButton.T;
		
		// sphere state			
		SphereColor = new Color(sphereState.r, sphereState.g, sphereState.b);
		IsTracking = (sphereState.tracking == 1);
		TrackingHue = (int)sphereState.tracking_hue;	
		
		// image state
		SpherePixelPosition = new Vector2(imageState.u, imageState.v);
		SpherePixelRadius = imageState.r;
		SphereProjectionPosition = new Vector2(imageState.projectionx, imageState.projectiony);
		SphereDistance = imageState.distance/100;
		SphereVisible = (imageState.visible == 1);
		SphereRadiusValid = (imageState.r_valid == 1);
		
		
	}
		
	public bool GetButtons(MoveButton requestButtons) {
		return GetButtons(Buttons, requestButtons);
	}
	
	public bool GetButtonsAny(MoveButton requestButtons) {
		return GetButtons(Buttons, requestButtons);
	}
	
	public bool GetButtonsAny() {
		return GetButtonsAny(MoveButton.All);
	}
	
	public bool GetButtonsUp(MoveButton requestButtons) {
		return GetButtons(PrevButtons, requestButtons) && !GetButtons(Buttons, requestButtons);
	}
	
	public bool GetButtonsDown(MoveButton requestButtons) {
		return !GetButtons(PrevButtons, requestButtons) && GetButtons(Buttons, requestButtons);
	}
	
	private bool GetButtons(MoveButton state, MoveButton requestButtons) {
		return (state & requestButtons) == requestButtons;
	}
	
	private bool GetButtonsAny(MoveButton state, MoveButton requestButtons) {
		return (state & requestButtons) > 0;
	}
	
}

internal enum MoveButtonLineup {
		
	Square = 0,
	Triangle,
	Circle,
	Cross,
	Move,
	Start,
	Select,
	T,
	
	Total,
}
	
[Flags]
public enum MoveButton : uint {
	None	= 0,
	
	Square 	= 1 << MoveButtonLineup.Square,
	Triangle = 1 << MoveButtonLineup.Triangle,
	Circle 	= 1 << MoveButtonLineup.Circle,
	Cross 	= 1 << MoveButtonLineup.Cross,
	Move	= 1 << MoveButtonLineup.Move,
	Start 	= 1 << MoveButtonLineup.Start,
	Select 	= 1 << MoveButtonLineup.Select,
	T		= 1 << MoveButtonLineup.T,
	
	All = ((1 << MoveButtonLineup.Total) - 1),
}