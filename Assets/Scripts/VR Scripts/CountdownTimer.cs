//This script counts down the time the player has to complete the maze

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour 
{
	[SerializeField] PlayerState m_PlayerState;		//A reference to the player state script
	[SerializeField] float m_TotalGameTime = 180f;	//The amount of time the player has to win
	[SerializeField] Color m_StartColor = Color.white ;				//Starting color of the bars
	[SerializeField] Color m_EndColor = Color.white;				//Final color of the bars
	[SerializeField] Slider[] m_Bars;					//An array of bars (sliders)


	Image[] m_BarImages;								//An array of the images on the sliders (for changing colors)
	float m_TimePerBar;								//How long each bar will be counting down
	float m_EllapsedTime;								//How much time the current bar has been counting down
	float m_TotalEllapsedTime;						//How much time all of the bars have been counting down
	int m_Index;										//The index of the current bar

	//The Reset() method is useful for automatically filling properties in the inspector
	void Reset()
	{
		//Find the PlayerState script on the player
		m_PlayerState = GameObject.FindObjectOfType<PlayerState> ();
		//Find all of the Sliders that are children of this object
		m_Bars = GetComponentsInChildren<Slider> ();
	}


	void Start () 
	{
		//Calculate how much time each bar gets
		m_TimePerBar = m_TotalGameTime / m_Bars.Length;
		m_EllapsedTime = 0f;
		m_TotalEllapsedTime = 0f;

		//Find all slider images
		m_BarImages = new Image[m_Bars.Length];
		for (int i = 0; i < m_Bars.Length; i++) 
		{
			m_BarImages [i] = m_Bars [i].GetComponentInChildren<Image> ();
			m_BarImages [i].color = m_StartColor;
		}
	}


	public void BeginCountdown()
	{
		StartCoroutine (CountDown ());
	}
		
	//This method counts down the sliders until the player is out of time
	IEnumerator CountDown()
	{
		//Get the first bar to count down (which is the last bar in the list)
		m_Index = m_Bars.Length - 1;

		//Loop while there are still bars left and the player hasn't won or lost
		while (m_Index >= 0 && m_PlayerState.isValidTarget) 
		{
			//Calculate ellapsed time
			m_EllapsedTime += Time.deltaTime;
			m_TotalEllapsedTime += Time.deltaTime;

			//Reduce the current slider
			m_Bars [m_Index].value = 1f - m_EllapsedTime / m_TimePerBar;

			//If this slider is out of time, move to the next slider
			if (m_EllapsedTime >= m_TimePerBar) 
			{
				m_EllapsedTime = 0f;
				m_Index--;
			}

			//Change the color of all of the sliders
			for (int i = 0; i < m_BarImages.Length; i++) 
				m_BarImages [i].color = Color.Lerp (m_StartColor, m_EndColor, m_TotalEllapsedTime / m_TotalGameTime);
			
			//Wait a frame before looping
			yield return null;
		}

		//If we get to this point then the countdown is finished and the player dies
		m_PlayerState.Died ();
	}
}
