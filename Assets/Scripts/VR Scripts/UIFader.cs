using System;
using UnityEngine;
using System.Collections;

// This class is used to fade in and out groups of UI
// elements.  It contains a variety of functions for
// fading in different ways.
[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    public event Action OnFadeInComplete;                   // This event is triggered when the UI elements have finished fading in.
    public event Action OnFadeOutComplete;                  // This event is triggered when the UI elements have finished fading out.

    [SerializeField] float m_FadeSpeed = 1f;        		// The amount the alpha of the UI elements changes per second.
    [SerializeField] CanvasGroup m_GroupToFade; 			// All the groups of UI elements that will fade in and out.
	[SerializeField] bool m_StartVisible;					// Should the UI elements be visible to start?
	[SerializeField] bool m_StartWithFade;					// Should the UI elements begin fading with they start up? Fading can either be in or out (opposite of their starting alpha)

    bool m_Fading;                                 			// Whether the UI elements are currently fading in or out.

    public bool Visible { get; private set; }               // Whether the UI elements are currently visible.


	void Reset()
	{
		//Attempt to grab the CanvasGroup on this object
		m_GroupToFade = GetComponent<CanvasGroup>();
	}


	void Start()
	{
		//If the object should start visible, set it to be visible. Otherwise, set it invisible
		if (m_StartVisible)
			SetVisible ();
		else
			SetInvisible ();

		//If there shouldn't be any initial fade, leave this method
		if (!m_StartWithFade)
			return;

		//If the object is currently visible, fade out. Otherwise fade in
		if (Visible)
			StartFadeOut ();
		else
			StartFadeIn ();
	}


	//Publicly accessible methods for fading in or fading out without needing to start a 
	//coroutine. These are needed in order for UI events (like buttons) to start a fade in
	//or out.
	public void StartFadeIn()
	{
		StartCoroutine (FadeIn ());
	}


	public void StartFadeOut()
	{
		StartCoroutine (FadeOut ());
	}


    public IEnumerator WaitForFadeIn()
    {
        // Keep coming back each frame whilst the groups are currently fading.
        while (m_Fading)
        {
            yield return null;
        }

        // Return once the FadeIn coroutine has finished.
        yield return StartCoroutine (FadeIn ());
    }


    public IEnumerator InteruptAndFadeIn ()
    {
        // Stop all fading that is currently happening.
        StopAllCoroutines ();

        // Return once the FadeIn coroutine has finished.
        yield return StartCoroutine(FadeIn());
    }


    public IEnumerator CheckAndFadeIn ()
    {
        // If not already fading return once the FadeIn coroutine has finished.
        if (!m_Fading)
            yield return StartCoroutine (FadeIn ());
    }


    public IEnumerator FadeIn()
    {
        // Fading has now started.
        m_Fading = true;

        // Fading needs to continue until the group is completely faded in
		while (m_GroupToFade.alpha < 1f) 
		{
			//Increase the alpha
			m_GroupToFade.alpha += m_FadeSpeed * Time.deltaTime;
			//Wait a frame
			yield return null;
		}

        // If there is anything subscribed to OnFadeInComplete, call it.
        if (OnFadeInComplete != null)
            OnFadeInComplete();

        // Fading has now finished.
        m_Fading = false;

        // Since everthing has faded in now, it is visible.
        Visible = true;
    }


    // The following functions are identical to the previous ones but fade the CanvasGroups out instead.
    public IEnumerator WaitForFadeOut ()
    {
        while (m_Fading)
        {
            yield return null;
        }

        yield return StartCoroutine (FadeOut ());
    }


    public IEnumerator InteruptAndFadeOut ()
    {
        StopAllCoroutines ();
        yield return StartCoroutine (FadeOut ());
    }


    public IEnumerator CheckAndFadeOut()
    {
        if (!m_Fading)
            yield return StartCoroutine(FadeOut());
    }


    public IEnumerator FadeOut ()
    {
        m_Fading = true;

		while (m_GroupToFade.alpha > 0f) 
		{
			m_GroupToFade.alpha -= m_FadeSpeed * Time.deltaTime;

			yield return null;
		}

        if (OnFadeOutComplete != null)
            OnFadeOutComplete();

        m_Fading = false;

        Visible = false;
    }


    // These functions are used if fades are required to be instant.
    public void SetVisible ()
    {
		m_GroupToFade.alpha = 1f;

        Visible = true;
    }


    public void SetInvisible ()
    {
		m_GroupToFade.alpha = 0f;

        Visible = false;
    }
}

