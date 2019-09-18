using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CHANGELOG
// 11 October 2018 -- added BetterMouseLook
// - clamping vertical look, to prevent inverted controls / upside down controls
// - cursor lock, very important for WebGL FP games

// usage: put this on a object with main camera parented to it
// this lets you do mouse look
public class BetterMouseLook : MonoBehaviour
{

	private float vertical_look = 0f; // degrees for looking up/down... stored for later usage
	
	void Update () {

		
			// MOUSE LOOK!!!
		
			// getting mouse input
			// multiplying things by Time.deltaTime makes the operation framerate independent! IMPORTANT
			float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 100; // horizontal mouse movement
			float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * 100; // vertical mouse movement
		
			// rotate the camera based on mouse input
			// first, rotate body based on horizontal mouse movement
			transform.Rotate( 0f, mouseX, 0f); // yaw
		
			// BETTER MOUSE LOOK	
			// add mouse input to vertical_look, then clamp vertical_look
			vertical_look += -mouseY;
			vertical_look = Mathf.Clamp(vertical_look, -75f, 75f);
		
			// actually apply vertical_look to camera's rotation
			Camera.main.transform.localEulerAngles = new Vector3(vertical_look, 0, 0);
		
			// BETTER MOUSE LOOK
			// lock and hide the mouse cursor, if they click their mouse
			if (Input.GetMouseButtonDown(0)) // 0 = left-click
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		
	}

	
	
}
