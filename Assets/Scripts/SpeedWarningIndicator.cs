using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedWarningIndicator : MonoBehaviour {

    private PlayerController controller;
    public float low_speed_warning_threshold_percentage;
    public float low_speed_critical_threshold_percentage;
    public float low_speed_die_threshold_percentage;
    public float max_seconds_below_die_threshold;

    public Image flashing_warning_image;
    public Image flashing_critical_image;

    enum FlashState
    {
        NONE,
        WARNING,
        CRITICAL,
        DEAD,
        DONE
    }

    private FlashState state;
    private float time_below_die_threshold;
    // Use this for initialization
    void Awake ()
    {
        controller = GetComponent<PlayerController>();
        state = FlashState.NONE;
        EnableImagesByState();
    }
	
    void EnableImagesByState()
    {
        switch(state)
        {
            case FlashState.NONE:
            case FlashState.DONE:
                flashing_warning_image.gameObject.SetActive(false);
                flashing_critical_image.gameObject.SetActive(false);
                break;

            case FlashState.WARNING:
                flashing_warning_image.gameObject.SetActive(true);
                flashing_critical_image.gameObject.SetActive(false);
                break;

            case FlashState.CRITICAL:
            case FlashState.DEAD:
                flashing_warning_image.gameObject.SetActive(false);
                flashing_critical_image.gameObject.SetActive(true);
                break;
        }
    }

    void UpdateStateImages()
    {
        switch (state)
        {
            case FlashState.WARNING:
                flashing_warning_image.transform.localScale = new Vector3(Mathf.Sin(Time.time), Mathf.Sin(Time.time), 1.0f);
                break;

            case FlashState.CRITICAL:
            case FlashState.DEAD:
                flashing_critical_image.transform.localScale = new Vector3(Mathf.Sin(Time.time), Mathf.Sin(Time.time), 1.0f);
                break;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (controller.GetSpeedPercentage() >= 1.0f)
        {
            state = FlashState.NONE;
        }

        UpdateStateImages();

        if (state == FlashState.DONE)
        {
            return;
        }

		if(controller.GetSpeedPercentage() < low_speed_die_threshold_percentage)
        {
            if(state != FlashState.DEAD)
            {
                state = FlashState.DEAD;
                time_below_die_threshold = Time.time;
                EnableImagesByState();
            }

            if (Time.time - time_below_die_threshold > max_seconds_below_die_threshold)
            {
                controller.health = -controller.max_health;
                state = FlashState.DONE;
                EnableImagesByState();
            }
        }
        else if (controller.GetSpeedPercentage() < low_speed_critical_threshold_percentage)
        {
            if (state != FlashState.CRITICAL)
            {
                state = FlashState.CRITICAL;
                EnableImagesByState();
            }
        }
        else if (controller.GetSpeedPercentage() < low_speed_warning_threshold_percentage)
        {
            if (state != FlashState.WARNING)
            {
                state = FlashState.WARNING;
                EnableImagesByState();
            }
        }
        else if(state != FlashState.NONE)
        {
            state = FlashState.NONE;
            EnableImagesByState();
        }
    }
}
