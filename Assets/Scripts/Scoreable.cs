using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreable : MonoBehaviour
{
    public enum Action
    {
        NOTHING,
        DEACTIVATE
    }

    public bool Trigger = false;
    public bool Collision = false;

    public Action TriggerAction = Action.NOTHING;
    public Action CollisionAction = Action.NOTHING;

    public float TriggerScore = 0.0f;
    public float CollisionScore = 0.0f;

    private void PerformAction(Action which)
    {
        switch (which)
        {
            case Action.NOTHING:
                break;
            case Action.DEACTIVATE:
                transform.gameObject.SetActive(false);
                break;
        }
    }

    private void PerformEvent(Action input, GameObject other)
    {
        if (input != Action.NOTHING)
        {
            Scorer scorer = other.GetComponent<Scorer>();
            if (scorer != null)
            {
                PerformAction(input);
            }
        }
    }

    public void PerformTriggerAction()
    {
        PerformAction(TriggerAction);
    }

    public void PerformCollisionAction()
    {
        PerformAction(CollisionAction);
    }

    void OnTriggerEnter(Collider other)
    {
        PerformEvent(TriggerAction, other.gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        PerformEvent(CollisionAction, other.gameObject);
    }
}
