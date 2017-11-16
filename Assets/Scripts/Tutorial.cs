using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

    private bool tilt;
    private bool jump;
    private bool swipe;
    private bool health;
    private bool done;
    private bool updating;

    private HoverEngine hv_engine;
    private HealthController health_controller;
    private PlayerController controller;

    public Text info_text;
    private TextFadeOut info_text_fadeout;

    void CheckDone()
    {
        if (tilt &&
            jump &&
            swipe &&
            health)
        {
            done = true;
        }
    }

    enum CurrentUpdate
    {
        None,
        Tilt,
        Jump,
        Swipe,
        Health
    }

    private CurrentUpdate current_update;

    PrefabProperties.Prefab GetCurrent()
    {
        if (hv_engine.below == null)
        {
            return PrefabProperties.Prefab.None;
        }

        PrefabProperties prefab = hv_engine.below.GetComponent<PrefabProperties>();
        if (prefab == null)
        {
            return PrefabProperties.Prefab.None;
        }

        return prefab.prefab;
    }

    PrefabProperties.Prefab GetInFront()
    {
        if (hv_engine.below == null)
        {
            return PrefabProperties.Prefab.None;
        }

        ConnectionOffsets offsets = hv_engine.below.GetComponent<ConnectionOffsets>();
        if (offsets == null)
        {
            return PrefabProperties.Prefab.None;
        }

        if(offsets.taken.Length == 0)
        {
            return PrefabProperties.Prefab.None;
        }

        int next_index = 1;
        if (offsets.taken.Length == 1)
        {
            next_index = 0;
        }

        GameObject next = offsets.taken[next_index];
        if(next == null)
        {
            return PrefabProperties.Prefab.None;
        }

        PrefabProperties prefab = next.GetComponent<PrefabProperties>();
        if(prefab == null)
        {
            return PrefabProperties.Prefab.None;
        }

        return prefab.prefab;
    }

	// Use this for initialization
	void Awake ()
    {
        tilt = false;
        jump = false;
        swipe = false;
        health = false;
        done = false;
        updating = false;
        current_update = CurrentUpdate.None;

        hv_engine = GetComponent<HoverEngine>();
        health_controller = GetComponent<HealthController>();
        controller = GetComponent<PlayerController>();
        info_text_fadeout = info_text.GetComponent<TextFadeOut>();
        info_text.gameObject.SetActive(false);
    }
	

	// Update is called once per frame
	void Update ()
    {
		if(done)
        {
            return;
        }

        if(!controller.awoken_complete)
        {
            return;
        }

        CheckDone();
        updating = current_update != CurrentUpdate.None;
     
        if (updating)
        {
            switch(current_update)
            {
                case CurrentUpdate.Swipe:
                    UpdateSwipe();
                    break;

                case CurrentUpdate.Jump:
                    UpdateJump();
                    break;

                case CurrentUpdate.Tilt:
                    UpdateTilt();
                    break;

                case CurrentUpdate.Health:
                    UpdateHealth();
                    break;
            }
            return;
        }

        if (!tilt)
        {
            UpdateTilt();
            return;
        }

        PrefabProperties.Prefab prefab = GetCurrent();

        switch (prefab)
        {
            case PrefabProperties.Prefab.RoadCrossA:
            case PrefabProperties.Prefab.RoadCrossB:
            case PrefabProperties.Prefab.RoadCrossLeft:
            case PrefabProperties.Prefab.RoadCrossRight:
                UpdateSwipe();
                return;

            case PrefabProperties.Prefab.BridgeDamage:
                UpdateJump();
                return;
        }

        prefab = GetInFront();
        switch (prefab)
        {
            case PrefabProperties.Prefab.RoadCrossA:
            case PrefabProperties.Prefab.RoadCrossB:
            case PrefabProperties.Prefab.RoadCrossLeft:
            case PrefabProperties.Prefab.RoadCrossRight:
                UpdateSwipe();
                return;

            case PrefabProperties.Prefab.BridgeDamage:
                UpdateJump();
                return;
        }

        if (health_controller.health < health_controller.max_health ||
            controller.GetActualSpeed() < controller.speed)
        {
            UpdateHealth();
            return;
        }

    }

    void UpdateHealth()
    {
        if (!health)
        {
            if (!updating)
            {
                info_text.text = "You lose health and speed when hitting obstacles or falling,\n these replenish over time";
                info_text.gameObject.SetActive(true);
                info_text_fadeout.fadeOutTime = 6;
                info_text_fadeout.FadeOut();
                current_update = CurrentUpdate.Health;
            }

            if (info_text_fadeout.faded)
            {
                health = true;
                info_text.gameObject.SetActive(false);
                current_update = CurrentUpdate.None;
            }
        }
    }

    void UpdateTilt()
    {
        if (!tilt)
        {
            if (!updating)
            {
                info_text.text = "Tilt your device to strafe across the plane \n to avoid obstacles or collect coins";
                info_text.gameObject.SetActive(true);
                info_text_fadeout.fadeOutTime = 7;
                info_text_fadeout.FadeOut();
                current_update = CurrentUpdate.Tilt;
            }

            if (info_text_fadeout.faded)
            {
                tilt = true;
                info_text.gameObject.SetActive(false);
                current_update = CurrentUpdate.None;
            }
        }
    }

    void UpdateSwipe()
    {
        if (!swipe)
        {
            if (!updating)
            {
                info_text.text = "Swipe left/right to turn your ship \n on intersections";
                info_text.gameObject.SetActive(true);
                info_text_fadeout.fadeOutTime = 4;
                info_text_fadeout.FadeOut();
                current_update = CurrentUpdate.Swipe;
            }

            if (info_text_fadeout.faded)
            {
                swipe = true;
                info_text.gameObject.SetActive(false);
                current_update = CurrentUpdate.None;
            }
        }
    }

    void UpdateJump()
    {
        if (!jump)
        {
            if (!updating)
            {
                info_text.text = "Tap to jump \n over obstacles or gaps";
                info_text.gameObject.SetActive(true);
                info_text_fadeout.fadeOutTime = 4;
                info_text_fadeout.FadeOut();
                current_update = CurrentUpdate.Jump;
            }

            if (info_text_fadeout.faded)
            {
                jump = true;
                info_text.gameObject.SetActive(false);
                current_update = CurrentUpdate.None;
            }
        }
    }
}
