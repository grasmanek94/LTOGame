using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabProperties : MonoBehaviour {

    public enum Prefab
    {
        BridgeDamage,
        BridgeStraight,
        BridgeSlopeUp,
        BridgeSlopeDown,
        RoadBusStop,
        RoadCrossA,
        RoadCrossB,
        RoadCrossRight,
        RoadCrosswalk,
        RoadEndA,
        RoadEndB,
        RoadStraight,
        RoadCrossLeft
    }

    public Prefab prefab;
}
