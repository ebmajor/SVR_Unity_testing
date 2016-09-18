using UnityEngine;
using System.Collections;

public class CameraAim : MonoBehaviour 
{
	[SerializeField] PlayerMovement playerMovement;

	void Update () 
	{
		//"Fire1" is left mouse click, left control key, or screen tap
		if (Input.GetButtonDown("Fire1"))
		{
			//Generate a world space (x, y, z) "Ray" from the camera based on where we clicked in screen space (x,y)
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayHit;

			//Does our Ray hit anything?
			if (Physics.Raycast(ray, out rayHit, 200f))
			{
				//If we hit the "floor" move the waypoint marker there and set it as the runner's destination
				if (rayHit.transform.tag == "Floor")
				{
					playerMovement.Move (rayHit.point);
				}
			}
		}
	}
}
