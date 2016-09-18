using UnityEngine;
using System.Collections;

public class VRCameraRotation : MonoBehaviour 
{
	[SerializeField] VRInput m_InputScript;
	[SerializeField] VRInteractiveItem m_LeftArrow;
	[SerializeField] VRInteractiveItem m_RightArrow;
	[SerializeField] CameraRotationController m_RotationController;


	void Reset()
	{
		//Attempt to find the VRInput script in the scene
		m_InputScript = GameObject.FindObjectOfType<VRInput>();
		//Get a reference to the CameraRotationController
		m_RotationController = GetComponent<CameraRotationController>();
	}

	void OnEnable()
	{
		m_InputScript.OnSwipe += HandleSwipe;
		m_LeftArrow.OnClick += HandleLeftArrowClick;
		m_RightArrow.OnClick += HandleRightArrowClick;
	}


	void OnDisable()
	{
		m_InputScript.OnSwipe -= HandleSwipe;
		m_LeftArrow.OnClick -= HandleLeftArrowClick;
		m_RightArrow.OnClick -= HandleRightArrowClick;
	}

	//When the player swipes, rotation the camera rig
	void HandleSwipe(VRInput.SwipeDirection dir)
	{
		if (dir == VRInput.SwipeDirection.LEFT)
			m_RotationController.RotateCCW ();
		else if (dir == VRInput.SwipeDirection.RIGHT)
			m_RotationController.RotateCW ();
	}

	//Handle left button selection
	void HandleLeftArrowClick()
	{
		m_RotationController.RotateCCW ();
	}

	//Handle right button selection
	void HandleRightArrowClick()
	{
		m_RotationController.RotateCW ();
	}
}
