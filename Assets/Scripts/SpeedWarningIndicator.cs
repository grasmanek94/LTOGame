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
    private float dying_percentage;
    private bool enlarging;

    // Use this for initialization
    void Awake ()
    {
        controller = GetComponent<PlayerController>();
        state = FlashState.NONE;
        EnableImagesByState();
    }

    IEnumerator SmoothFullScale(GameObject image, float inTime)
    {
        float begin = image.transform.localScale.x;
        for (var t = 0f; t <= 2.0f; t += Time.deltaTime / inTime)
        {
            float scale_value = Mathf.Lerp(begin, 1.0f, Mathf.Min(t, 1.0f));
            image.transform.localScale = new Vector3(scale_value, scale_value, 1.0f);
            yield return null;
        }
        image.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        enlarging = false;
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
        float flashval = Mathf.Abs(Mathf.Sin(Time.time * 3.0f));
        switch (state)
        {
            case FlashState.WARNING:
                flashing_warning_image.transform.localScale = new Vector3(flashval, flashval, 1.0f);
                break;

            case FlashState.CRITICAL:
                flashing_critical_image.transform.localScale = new Vector3(flashval, flashval, 1.0f);
                flashing_critical_image.fillAmount = 1.0f;
                break;

            case FlashState.DEAD:
                if (!enlarging)
                {
                    enlarging = true;
                    StartCoroutine(SmoothFullScale(flashing_critical_image.gameObject, 0.25f));
                }
                flashing_critical_image.fillAmount = 1.0f - dying_percentage;
                break;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (controller.GetSpeedPercentage() >= 1.0f)
        {
            state = FlashState.NONE;
            EnableImagesByState();
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

            float time_dying = Time.time - time_below_die_threshold;
            dying_percentage = time_dying / max_seconds_below_die_threshold;
            if(dying_percentage > 1.0f)
            {
                dying_percentage = 1.0f;
            }

            if (time_dying > max_seconds_below_die_threshold)
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
