//This script allows the player to tap on the ground and get the runner to move to the chosen spot

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public GameObject navMarkerPrefab;	//Waypoint marker model
	public bool canMove = true;			//Can the player move?

	GameObject navMarker;				//Reference to an instantiated version of the waypoint marker model
	NavMeshAgent navAgent;				//Refernece to nav mesh agent component
	Animator anim;						//Reference to animator component


	void Start()
	{
		//Get our references
		navAgent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();		//We will uncomment this during the workshop

		//Instantiate a waypoint marker
		navMarker = (GameObject)Instantiate(navMarkerPrefab);
		navMarker.SetActive(false);
	}

	void Update()
	{
		//If the player is moving and it has an animator, update the animation
		if(navAgent.hasPath && anim != null)
			UpdateAnimation();
	}

	public void Move(Vector3 point)
	{
		if (!canMove)
			return;

		//Move the waypoint object and the navmesh agent to the desired point
		navMarker.transform.position = point;
		navMarker.SetActive(true);

		navAgent.SetDestination(point);
	}

	void UpdateAnimation()
	{
		//If we haven't reached the destination calculated the vector values to pass into the animator
		if (navAgent.remainingDistance > navAgent.stoppingDistance)
		{
			Vector3 move = navAgent.desiredVelocity;

			if (move.magnitude > 1f)
				move.Normalize();

			//Convert our normalized vector from world space to local space (not the world's "forward", but "my" "forward")
			move = transform.InverseTransformDirection(move);

			float m_TurnAmount = Mathf.Atan2(move.x, move.z);
			float m_ForwardAmount = move.z;

			anim.SetFloat("Speed", Mathf.Abs(m_ForwardAmount));
			anim.SetFloat("Turn", m_TurnAmount);	//This MAY be commented our during the workshop if there is time to set up an animation blend for turning

			//Debug.Log("Player Running...");	//Not necessarily to be uncommented, I just wanted you to see how to output to the console window
		}
		else
		{
			//When we reach our destination, hide the waypoint marker
			navMarker.SetActive(false);

			anim.SetFloat("Speed", 0f);
			anim.SetFloat("Turn", 0f);	//This MAY be commented our during the workshop if there is time to set up an animation blend for turning

			//Debug.Log("Player Stopping...");	//Not necessarily to be uncommented, I just wanted you to see how to output to the console window
		}
    }

	//Public method that sets the canMove variable
	public void Mobilize()
	{
		canMove = true;
	}

	//Public method that sets the canMove variable
	public void Immobilize()
	{
		canMove = false;
	}
}
