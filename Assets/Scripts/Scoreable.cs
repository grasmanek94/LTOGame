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

    public void PerformTriggerAction()
    {
        PerformAction(TriggerAction);
    }

    public void PerformCollisionAction()
    {
        PerformAction(CollisionAction);
    }
}
