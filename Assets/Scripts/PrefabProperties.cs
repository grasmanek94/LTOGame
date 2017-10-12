using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabProperties : MonoBehaviour {

    public enum Prefab
    {
        Bridge_Damage,
        Bridge_Simple_Straight,
        BridgeSlope_Simple,
        Road_BusStop,
        Road_Cross_A_A,
        Road_Cross_A_B,
        Road_Crosswalk,
        Road_End_A,
        Road_End_B,
        Road_Intersection_A,
        Road_Streight
    }

    public Prefab prefab;
}
