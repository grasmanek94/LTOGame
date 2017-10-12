﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWorldGenerator : MonoBehaviour {
    
    public GameObject follow;

    private Dictionary<PrefabProperties.Prefab, List<GameObject>> instances;
    private Dictionary<PrefabProperties.Prefab, List<GameObject>> inactive;
    private Dictionary<PrefabProperties.Prefab, List<GameObject>> active;

    private int creation_tick;

    void InstantiatePrefabs(string resource, int count)
    {
        if(count < 1)
        {
            return;
        }

        GameObject original = (GameObject)Resources.Load(resource);
        PrefabProperties.Prefab which = original.GetComponent<PrefabProperties>().prefab;

        if (!instances.ContainsKey(which))
        {
            instances.Add(which, new List<GameObject>());
            inactive.Add(which, new List<GameObject>());
            active.Add(which, new List<GameObject>());
        }

        --count;
        
        original.SetActive(false);
        instances[which].Add(original);
        inactive[which].Add(original);

        while (count --> 0)
        {
            GameObject game_object = Instantiate(original);

            game_object.SetActive(false);

            instances[which].Add(game_object);
            inactive[which].Add(game_object);
        }
    }

    GameObject Activate(PrefabProperties.Prefab which)
    {
        if(inactive[which].Count < 1)
        {
            return null;
        }

        GameObject game_object = inactive[which][0];

        inactive[which].RemoveAt(0);
        active[which].Add(game_object);

        game_object.SetActive(true);

        return game_object;
    }

    bool Deactivate(GameObject game_object)
    {
        PrefabProperties.Prefab which = game_object.GetComponent<PrefabProperties>().prefab;

        if(!active[which].Contains(game_object))
        {
            return false;
        }

        active[which].Remove(game_object);
        inactive[which].Add(game_object);

        game_object.SetActive(false);
        ConnectionOffsets game_object_offsets = game_object.GetComponent<ConnectionOffsets>();
        game_object_offsets.ResetTaken();

        return true;
    }

    bool ConnectFromFirstAvailable(GameObject from, GameObject to)
    {
        ConnectionOffsets from_offsets = from.GetComponent<ConnectionOffsets>();

        int i = 0;
        for (; i < from_offsets.taken.Length; ++i)
        {
            if(!from_offsets.taken[i])
            {
                break;
            }
        }

        if (from_offsets.taken[i])
        {
            return false;
        }

        ConnectionOffsets to_offsets = to.GetComponent<ConnectionOffsets>();

        if(to_offsets.taken[0] != null)
        {
            return false;
        }

        to_offsets.taken[0] = from;
        from_offsets.taken[i] = to;

        to.transform.position = from.transform.position + from_offsets.position_offsets[i] + to_offsets.position_offsets[0];
        to.transform.eulerAngles = from.transform.eulerAngles + from_offsets.rotation_offsets[i] + to_offsets.rotation_offsets[0];

        return true;
    }

    // Use this for initialization
    void Start ()
    {
        instances = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();
        inactive = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();
        active = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();

        InstantiatePrefabs("LowpolyStreetPack /Prefabs/Roads/Bridges/Elements/Bridge_Damage_Corrected", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Simple_Straight_Corrected", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Slope_Down_Corrected", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Slope_Up_Corrected", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_BusStop_Corrected", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Cross_A_A_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Cross_A_B_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Crosswalk_Corrected", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_A_Corrected", 1);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_B_Corrected", 1);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Intersection_A_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Streight_Corrected", 10);

        creation_tick = 0;

        GameObject start = Activate(PrefabProperties.Prefab.RoadEndA);
        start.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        GameObject next = Activate(PrefabProperties.Prefab.RoadStraight);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.BridgeSlopeUp);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.RoadCrossC);
        ConnectFromFirstAvailable(start, next);
    }

    // Update is called once per frame
    void Update ()
    {

	}
}
