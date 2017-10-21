using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;

public class Rechargeable : MonoBehaviour {

    public enum Refill
    {
        LINEAR,
        EXPONENTIAL
    }

    public string description;
    public float full_charge;
    public float min_charge;
    public float recharge_time;
    public float recharge_delay;
    public float initial_charge;
    public Refill refill_type;

    public float charge
    {
        get;
        private set;
    }

    public float percentage
    {
        get;
        private set;
    }

    private float start_refill;
    private float refill_delta;
    private float last_refill;

    private void Awake()
    {
        refill_delta = (full_charge - min_charge) / recharge_time;
    }

    private void OnEnable()
    {
        charge = initial_charge;
        percentage = initial_charge / (full_charge - min_charge);
        if (refill_type == Refill.EXPONENTIAL)
        {
            percentage *= percentage;
        }
    }

    void FixedUpdate ()
    {
		if(charge < full_charge && Time.time >= start_refill)
        {
            percentage = (Time.time - start_refill) / recharge_time;
            if(refill_type == Refill.EXPONENTIAL)
            {
                percentage *= percentage;
            }
            charge = percentage * refill_delta;
            if(charge > full_charge)
            {
                charge = full_charge;
                percentage = 1.0f;
            }
        }
	}

    public float UseCharge()
    {
        float used = charge - min_charge;
        charge = min_charge;
        start_refill = Time.time + recharge_delay;
        return used;
    }

    public override string ToString()
    {
        return base.ToString() + " (" + description + ")";
    }
}
