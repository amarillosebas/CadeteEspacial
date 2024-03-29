﻿using UnityEngine;
using System.Collections;

public class FPSPlayerInput : MonoBehaviour {
	[Header("Dependencies")]
	public FPSPlayerDebug playerDebug;
	public Pure_FPP_Camera playerCamera;
	public Pure_FPP_Controller playerController;
	public PlayerTransformManager playerTransformManager;
	public PlayerReticle playerReticle;

	[Space(5f)]
	[Header("Variables")]
	public bool emulate = true;
	public bool trigger = false;

	Vector2 touchDirection;
	private Vector2 touchOrigin = -Vector2.one;
	
	void Start () {
		_directionTouch = int.MaxValue;
		_triggerTouch = int.MaxValue;
	}

	private int _directionTouch;
	private int _triggerTouch;

	private bool triggerState = false;
	private bool triggerBuffer = true;

	private void Update () {
		if (!TheGameManager.gameManager.vrActive) {
			if (Input.touchCount > 0) {
				foreach (Touch touch in Input.touches) {

					switch (touch.phase) {
						case TouchPhase.Began:
							if (touch.position.x < Screen.width / 2) {
								_directionTouch = touch.fingerId;
								touchOrigin = touch.position;
							} else if (touch.position.x > Screen.width / 2) {
								_triggerTouch = touch.fingerId;
								trigger = true;
							}
						break;
						case TouchPhase.Moved:
							if (touch.fingerId == _directionTouch) {
								touchDirection = touch.position - touchOrigin;
								touchDirection = new Vector2(Mathf.Clamp(touchDirection.x, -300f, 300f) / 300f, Mathf.Clamp(touchDirection.y, -300f, 300f) / 300f);
								playerCamera.SetLookVector(touchDirection);
							}
						break;
						case TouchPhase.Ended:
							if (touch.fingerId == _directionTouch) {
								playerCamera.StopRotation();
								touchOrigin.x = -1;
								touchDirection = Vector2.zero;
								_directionTouch = int.MaxValue;
							} else if (touch.fingerId == _triggerTouch) {
								trigger = false;
								_triggerTouch = int.MaxValue;
							}
						break;
					}
				}
			}
		}

		else {
			if (GvrControllerInput.ClickButton || Input.touchCount > 0) {
				trigger = true;
			} else trigger = false;
		}

		#if UNITY_EDITOR
		Emulation();
		#endif

		if (trigger != triggerBuffer) {
			triggerBuffer = trigger;

			if (trigger) {
				if (playerReticle) playerReticle.Trigger_Down();
				if (playerController) playerController.Trigger_Down();
			} else {
				if (playerReticle) playerReticle.Trigger_Up();
				if (playerController) playerController.Trigger_Up();
			}
		}
	}

	public void ToggleVR (bool b) {
		playerCamera.ToggleVR(b);
		playerController.ToggleVR(b);
	}

	void Emulation () {
		if (emulate) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				trigger = true;
			}
			if (Input.GetKeyUp(KeyCode.Space)) {
				trigger = false;
			}

			float ex = Input.GetAxis("Horizontal");
			float ey = Input.GetAxis("Vertical");
			Vector2 emulatedVector = new Vector2 (ex, ey);

			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)) playerCamera.SetLookVector(emulatedVector);

			if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)) playerCamera.StopRotation();
		}
	}
}