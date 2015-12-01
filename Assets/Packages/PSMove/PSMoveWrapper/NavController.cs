using UnityEngine;
using System.Collections;
using System;
using PSMoveSharp;

public class NavController {
	public bool Connected {get; private set;}
	public int Num {get; private set;}
	
	public NavData Data {
		get {
			if(_dirty) {
				_data.Update(padData);
				_dirty = false;
			}
			return _data;
		}
	}
	
	private NavData _data;
	private bool _dirty;
	
	private PSMoveSharpNavPadData padData;
	
	public NavController(int num) {
		_data = new NavData();
		_dirty = true;
		padData = new PSMoveSharpNavPadData();
		Connected = false;
		Num = num;
	}
	
	public void Update(PSMoveSharpState state) {
		padData = state.padData[Num];
		
		Connected = ((state.navInfo.port_status[Num] & 0x1)==0x1);
		
		_dirty = true;
	}
	
}

public class NavData {
	
	#region nav data		
	
	public NavButton PrevButtons {get; private set;}
	public NavButton Buttons {get; private set;}
	
	/// <summary>
	/// range from -128 to 127
	/// </summary>
	public Vector2 ValueAnalog;
	
	/// <summary>
	/// range from 0 to 255
	/// </summary>
	public int ValueL2;
	
	public int thresholdL2 = 250;
	#endregion
	
	
	public void Update(PSMoveSharpNavPadData padData) {
		PrevButtons = Buttons;
		Buttons = NavButton.None;
		if((padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlUp)!=0)
			Buttons |= NavButton.Up;
		if((padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlDown)!=0)
			Buttons |= NavButton.Down;
		if((padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlLeft)!=0)
			Buttons |= NavButton.Left;
		if((padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlRight)!=0)
			Buttons |= NavButton.Right;
		if((padData.button[PSMoveSharpConstants.offsetDigital2] & PSMoveSharpConstants.ctrlCross)!=0)
			Buttons |= NavButton.Cross;
		if((padData.button[PSMoveSharpConstants.offsetDigital2] & PSMoveSharpConstants.ctrlCircle)!=0)
			Buttons |= NavButton.Circle;
		if((padData.button[PSMoveSharpConstants.offsetDigital2] & PSMoveSharpConstants.ctrlL1)!=0)
			Buttons |= NavButton.L1;
		if((padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlL3)!=0)
			Buttons |= NavButton.L3;
		
		ValueL2 = padData.button[PSMoveSharpConstants.offsetPressL2];
		
		if(ValueL2 >= thresholdL2) 
			Buttons |= NavButton.L2;
				
		ValueAnalog.x = padData.button[PSMoveSharpConstants.offsetAnalogLeftX] - 128;
		ValueAnalog.y = padData.button[PSMoveSharpConstants.offsetAnalogLeftY] - 128;
	}
	
	
	public bool GetButtons(NavButton requestButtons) {
		return GetButtons(Buttons, requestButtons);
	}
	
	public bool GetButtonsAny(NavButton requestButtons) {
		return GetButtons(Buttons, requestButtons);
	}
	
	public bool GetButtonsAny() {
		return GetButtonsAny(NavButton.All);
	}
	
	public bool GetButtonsUp(NavButton requestButtons) {
		return GetButtons(PrevButtons, requestButtons) && !GetButtons(Buttons, requestButtons);
	}
	
	public bool GetButtonsDown(NavButton requestButtons) {
		return !GetButtons(PrevButtons, requestButtons) && GetButtons(Buttons, requestButtons);
	}
	
	private bool GetButtons(NavButton state, NavButton requestButtons) {
		return (state & requestButtons) == requestButtons;
	}
	
	private bool GetButtonsAny(NavButton state, NavButton requestButtons) {
		return (state & requestButtons) > 0;
	}
}


internal enum NavButtonLineup {
	Left = 0,
	Up,
	Right,
	Down,
	Circle,
	Cross,
	L1,
	L2,
	L3,
	
	Total,
}

[Flags]
public enum NavButton : uint {
	None	= 0,
	
	Left 	= 1 << NavButtonLineup.Left,
	Up 		= 1 << NavButtonLineup.Up,
	Right 	= 1 << NavButtonLineup.Right,
	Down 	= 1 << NavButtonLineup.Down,
	Circle 	= 1 << NavButtonLineup.Circle,
	Cross 	= 1 << NavButtonLineup.Cross,
	L1	 	= 1 << NavButtonLineup.L1,
	L2	 	= 1 << NavButtonLineup.L2,
	L3	 	= 1 << NavButtonLineup.L3,
	
	All = ((1 << NavButtonLineup.Total) - 1),
}