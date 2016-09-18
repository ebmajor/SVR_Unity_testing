using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

// The SelectionSlider functions as a bar that fills up 
// whilst the user looks at it and holds down the Fire1 button.
public class SelectionSlider : MonoBehaviour
{
	[SerializeField] bool m_GazeSelected;								// Whether the bar can be filled with gaze selection or if it requires input
	[SerializeField] float m_Duration = 2f;                     // The length of time it takes for the bar to fill.
    [SerializeField] AudioSource m_Audio;                       // Reference to the audio source that will play effects when the user looks at it and when it fills.
    [SerializeField] AudioClip m_OnOverClip;                    // The clip to play when the user looks at the bar.
    [SerializeField] AudioClip m_OnFilledClip;                  // The clip to play when the bar finishes filling.
    [SerializeField] Slider m_Slider;                           // Reference to the UI slider.
    [SerializeField] VRInteractiveItem m_InteractiveItem;       // Reference to the VRInteractiveItem to determine when to fill the bar.
	[SerializeField] Collider m_Collider;						// Reference to disable collider when selected
    [SerializeField] bool m_DisableOnBarFill;                   // Whether the bar should stop reacting once it's been filled (for single use bars).

	public UnityEvent OnBarFilled;                                   	// This UnityEvent is triggered when the bar finishes filling. UnityEvents appear graphically in the Inspector.

    bool m_GazeOver;                                            // Whether the user is currently looking at the bar.
    float m_Timer;                                              // Used to determine how much of the bar should be filled.
    Coroutine m_FillBarRoutine;                                 // Reference to the coroutine that controls the bar filling up, used to stop it if required.


	void Reset()
	{
		//Attempt tp get the Slider, InteractiveItem, and Collider attached to this object
		m_Slider = GetComponent<Slider>();
		m_InteractiveItem = GetComponent<VRInteractiveItem>();
		m_Collider = GetComponent<Collider> ();
	}

	//When object is enabled register for all of the needed input events
    void OnEnable ()
    {
		m_InteractiveItem.OnDown += HandleDown;
		m_InteractiveItem.OnUp += HandleUp;
        m_InteractiveItem.OnOver += HandleOver;
        m_InteractiveItem.OnOut += HandleOut;
    }


	//When object is disabled unregister for all input events
    void OnDisable ()
    {
		m_InteractiveItem.OnDown -= HandleDown;
		m_InteractiveItem.OnUp -= HandleUp;
        m_InteractiveItem.OnOver -= HandleOver;
        m_InteractiveItem.OnOut -= HandleOut;
    }


    IEnumerator FillBar ()
    {
        // When the bar starts to fill, reset the timer.
        m_Timer = 0f;

        // The amount of time it takes to fill is the duration set in the inspector
        float fillTime = m_Duration;

        // Until the timer is greater than the fill time...
        while (m_Timer < fillTime)
        {
            // ... add to the timer the difference between frames.
            m_Timer += Time.deltaTime;

            // Set the value of the slider or the UV based on the normalised time.
            SetSliderValue(m_Timer / fillTime);
            
            // Wait until next frame.
            yield return null;

            // If the user is still looking at the bar, go on to the next iteration of the loop.
            if (m_GazeOver)
                continue;

            // If the user is no longer looking at the bar, reset the timer and bar and leave the function.
            m_Timer = 0f;
            SetSliderValue (0f);
            yield break;
        }

        // If anything has subscribed to OnBarFilled call it now.
		if (OnBarFilled != null)
			OnBarFilled.Invoke ();

        // Play the clip for when the bar is filled.
        m_Audio.clip = m_OnFilledClip;
        m_Audio.Play();

        // If the bar should be disabled once it is filled, do so now.
		if (m_DisableOnBarFill) 
		{
			m_Collider.enabled = false;
			enabled = false;
		}
    }


    void SetSliderValue (float sliderValue)
    {
        // If there is a slider component set it's value to the given slider value.
		if (m_Slider)
			m_Slider.value = sliderValue;
    }


	//The user "clicked"
    void HandleDown ()
    {
        // If the user is looking at the bar start the FillBar coroutine and store a reference to it.
		if (m_GazeOver && !m_GazeSelected)
            m_FillBarRoutine = StartCoroutine(FillBar());
    }


	//The user stopped "clicking"
    void HandleUp ()
    {
		if (m_GazeSelected)
			return;
		
        // If the coroutine has been started (and thus we have a reference to it) stop it.
        if(m_FillBarRoutine != null)
            StopCoroutine (m_FillBarRoutine);

        // Reset the timer and bar values.
        m_Timer = 0f;
        SetSliderValue(0f);
    }


	//The user is looking at this
    void HandleOver ()
    {
        // The user is now looking at the bar.
        m_GazeOver = true;

		if(m_GazeSelected)
			m_FillBarRoutine = StartCoroutine(FillBar());

        // Play the clip appropriate for when the user starts looking at the bar.
        m_Audio.clip = m_OnOverClip;
        m_Audio.Play();
    }


	//The user stopped looking at this
    void HandleOut ()
    {
        // The user is no longer looking at the bar.
        m_GazeOver = false;

        // If the coroutine has been started (and thus we have a reference to it) stop it.
        if (m_FillBarRoutine != null)
            StopCoroutine(m_FillBarRoutine);

        // Reset the timer and bar values.
        m_Timer = 0f;
        SetSliderValue(0f);
    }
}
