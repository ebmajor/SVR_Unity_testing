using UnityEngine;
using UnityEngine.UI;

// The reticle is a small point at the centre of the screen.
// It is used as a visual aid for aiming. The position of the
// reticle is either at a default position in space or on the
// surface of a VRInteractiveItem as determined by the VREyeRaycaster.
public class Reticle : MonoBehaviour
{
    [SerializeField] float m_DefaultDistance = 5f;      // The default distance away from the camera the reticle is placed.
    [SerializeField] bool m_UseNormal;                  // Whether the reticle should be placed parallel to a surface.

    Vector3 m_OriginalScale;                            // Since the scale of the reticle changes, the original scale needs to be stored.
    Quaternion m_OriginalRotation;                      // Used to store the original rotation of the reticle.


    void Awake()
    {
        // Store the original scale and rotation.
		m_OriginalScale = transform.localScale;
		m_OriginalRotation = transform.localRotation;
    }


    // This overload of SetPosition is used when the the VREyeRaycaster hasn't hit anything.
    public void SetPosition (Ray ray)
    {
        // Set the position of the reticle to the default distance in front of the camera.
		transform.position = ray.GetPoint(m_DefaultDistance);

        // Set the scale based on the original and the distance from the camera.
		transform.localScale = m_OriginalScale * m_DefaultDistance;

        // The rotation should just be the default.
		transform.localRotation = m_OriginalRotation;
    }


    // This overload of SetPosition is used when the VREyeRaycaster has hit something.
    public void SetPosition (RaycastHit hit)
    {
		transform.position = hit.point;
		transform.localScale = m_OriginalScale * hit.distance;
        
        // If the reticle should use the normal of what has been hit...
        if (m_UseNormal)
            // ... set it's rotation based on it's forward vector facing along the normal.
			transform.rotation = Quaternion.FromToRotation (Vector3.forward, hit.normal);
        else
            // However if it isn't using the normal then it's local rotation should be as it was originally.
			transform.localRotation = m_OriginalRotation;
    }
}
