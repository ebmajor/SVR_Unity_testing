using System;
using UnityEngine;

// In order to interact with objects in the scene
// this class casts a ray into the scene and if it finds
// a VRInteractiveItem it exposes it for other classes to use.
// This script should be generally be placed on the camera.
public class VREyeRaycaster : MonoBehaviour
{
    public event Action<RaycastHit> OnRaycasthit;           // This event is called every frame that the user's gaze is over a collider.

	[SerializeField] PlayerMovement m_PlayerMovement;		// A reference to the player's movement script
	[SerializeField] Transform m_RaycastOrigin;				// The transform that the raycast will come from
    [SerializeField] LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
    [SerializeField] Reticle m_Reticle;                     // The reticle, if applicable.
    [SerializeField] VRInput m_VrInput;                     // Used to call input based events on the current VRInteractiveItem.
    [SerializeField] bool m_ShowDebugRay;                   // Optionally show the debug ray.
    [SerializeField] float m_DebugRayLength = 5f;           // Debug ray length.
    [SerializeField] float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
    [SerializeField] float m_RayLength = 500f;              // How far into the scene the ray is cast.

    
    private VRInteractiveItem m_CurrentInteractible;                //The current interactive item
    private VRInteractiveItem m_LastInteractible;                   //The last interactive item
	private RaycastHit hit;											//The last thing hit 

    // Utility for other classes to get the current interactive item
    public VRInteractiveItem CurrentInteractible
    {
        get { return m_CurrentInteractible; }
    }


	void Reset()
	{
		//Attempt the find the PlayerMovement script in the scene
		m_PlayerMovement = GameObject.FindObjectOfType<PlayerMovement>();
		//Attempt to find the VRInput script on this object
		m_VrInput = GetComponent<VRInput>();
	}
    
	//When this object is enabled, register for the needed input events
    void OnEnable()
    {
        m_VrInput.OnClick += HandleClick;
        m_VrInput.OnDoubleClick += HandleDoubleClick;
        m_VrInput.OnUp += HandleUp;
        m_VrInput.OnDown += HandleDown;
    }


	//When this object is enabled, deregister from the input events
    void OnDisable ()
    {
        m_VrInput.OnClick -= HandleClick;
        m_VrInput.OnDoubleClick -= HandleDoubleClick;
        m_VrInput.OnUp -= HandleUp;
        m_VrInput.OnDown -= HandleDown;
    }


    void Update()
    {
        EyeRaycast();
    }

  
    void EyeRaycast()
    {
        // Show the debug ray if required
        if (m_ShowDebugRay)
        {
            Debug.DrawRay(m_RaycastOrigin.position, m_RaycastOrigin.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
        }

        // Create a ray that points forwards from the camera.
        Ray ray = new Ray(m_RaycastOrigin.position, m_RaycastOrigin.forward);
        
        // Do the raycast forwards to see if we hit an interactive item
		if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
        {
            VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
            m_CurrentInteractible = interactible;

            // If we hit an interactive item and it's not the same as the last interactive item, then tell it we are looking at it
            if (interactible && interactible != m_LastInteractible)
                interactible.Over(); 

            // If this is a new interactable item, tell the old one that we aren't looking at it anymore
            if (interactible != m_LastInteractible)
                DeactiveLastInteractible();

            m_LastInteractible = interactible;

            // Something was hit, set at the hit position.
            if (m_Reticle)
                m_Reticle.SetPosition(hit);

            if (OnRaycasthit != null)
                OnRaycasthit(hit);
        }
        else
        {
            // Nothing was hit, deactive the last interactive item.
            DeactiveLastInteractible();
            m_CurrentInteractible = null;

            // Position the reticle at default distance.
            if (m_Reticle)
                m_Reticle.SetPosition(ray);
        }
    }


    void DeactiveLastInteractible()
    {
        if (m_LastInteractible == null)
            return;

        m_LastInteractible.Out();
        m_LastInteractible = null;
    }


	//Player stopped holding down input
    void HandleUp()
    {
        if (m_CurrentInteractible != null)
            m_CurrentInteractible.Up();
    }


	//Player is holding down input
    void HandleDown()
    {
        if (m_CurrentInteractible != null)
            m_CurrentInteractible.Down();
    }


	//Player just pressed Input
    void HandleClick()
    {
        if (m_CurrentInteractible != null)
            m_CurrentInteractible.Click();
    }


	//Player pressed input twice quickly
    void HandleDoubleClick()
    {
		if (m_CurrentInteractible == null)
			return;

		//If we hit the "floor" move the waypoint marker there and set it as the runner's destination
		if (m_CurrentInteractible.tag == "Floor")
			m_PlayerMovement.Move (hit.point);
		else
            m_CurrentInteractible.DoubleClick();
    }
}