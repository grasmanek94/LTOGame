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
        RoadCrossC,
        RoadCrosswalk,
        RoadEndA,
        RoadEndB,
        RoadStraight
    }

    public Prefab prefab;
}
