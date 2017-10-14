using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWorldGenerator : MonoBehaviour {
    
    public GameObject follow_player;

    private PlayerController follow_player_controller;

    private Dictionary<PrefabProperties.Prefab, List<GameObject>> instances;
    private Dictionary<PrefabProperties.Prefab, List<GameObject>> inactive;
    private Dictionary<PrefabProperties.Prefab, List<GameObject>> active;

    private int creation_tick;
    private System.Random random;

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
        ConnectionOffsets game_object_offsets = game_object.GetComponent<ConnectionOffsets>();
        game_object_offsets.ResetTaken();

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

    void DeactivateChainBackwards(GameObject from_exclusive)
    {
        if(from_exclusive == null)
        {
            return;
        }

        ConnectionOffsets game_object_offsets = from_exclusive.GetComponent<ConnectionOffsets>();
        if(game_object_offsets != null && game_object_offsets.taken.Length < 2)
        {
            return;
        }

        GameObject from_inclusive = game_object_offsets.taken[0];

        Dictionary<int, List<GameObject>> to_deactivate = new Dictionary<int, List<GameObject>>();

        int i = 0;
        to_deactivate.Add(i, new List<GameObject>());

        to_deactivate[i].Add(from_inclusive);

        int j = i++;
        to_deactivate.Add(i, new List<GameObject>());


        game_object_offsets = to_deactivate[j][0].GetComponent<ConnectionOffsets>();

        for (int m = 0; m < game_object_offsets.taken.Length; ++m)
        {
            if (game_object_offsets.taken[m] != null && m != 1) // 1 = forward
            {
                to_deactivate[i].Add(game_object_offsets.taken[m]);
            }
        }

        Deactivate(to_deactivate[j][0]);

        while (true)
        {
            j = i++;
            to_deactivate.Add(i, new List<GameObject>());

            int len = to_deactivate[j].Count;
            if(len == 0)
            {
                break;
            }

            for (int k = 0; k < len; ++k)
            {                
                game_object_offsets = to_deactivate[j][k].GetComponent<ConnectionOffsets>();
                
                for (int m = 0; m < game_object_offsets.taken.Length; ++m)
                {
                    if (game_object_offsets.taken[m] != null)
                    {
                        to_deactivate[i].Add(game_object_offsets.taken[m]);
                    }
                }
                Deactivate(to_deactivate[j][k]);
            }
        }
    }

    bool ConnectFromFirstAvailable(GameObject from, GameObject to)
    {
        ConnectionOffsets from_offsets = from.GetComponent<ConnectionOffsets>();

        int taken_idx = 0;
        for (int i = 0; i < from_offsets.taken.Length; ++i)
        {
            if(!from_offsets.taken[i])
            {
                taken_idx = i;
                break;
            }
        }

        if (from_offsets.taken[taken_idx])
        {
            return false;
        }

        ConnectionOffsets to_offsets = to.GetComponent<ConnectionOffsets>();

        if(to_offsets.taken[0] != null)
        {
            return false;
        }

        to_offsets.taken[0] = from;
        from_offsets.taken[taken_idx] = to;

        to.transform.position = from.transform.position + from_offsets.position_offsets[taken_idx] - to_offsets.position_offsets[0];
        to.transform.eulerAngles = from.transform.eulerAngles + from_offsets.rotation_offsets[taken_idx] - to_offsets.rotation_offsets[0];

        return true;
    }

    // Use this for initialization
    void Start ()
    {
        random = new System.Random(1337);
        
        instances = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();
        inactive = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();
        active = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();

        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Damage_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Simple_Straight_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Slope_Down_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Slope_Up_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_BusStop_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Cross_A_A_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Cross_A_B_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Crosswalk_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_A_Corrected", 1);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_B_Corrected", 1);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Intersection_A_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Streight_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Intersection_B_Corrected", 5);

        creation_tick = 0;

        GameObject start = Activate(PrefabProperties.Prefab.RoadEndA);
        start.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        GameObject next = Activate(PrefabProperties.Prefab.RoadStraight);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.RoadCrossRight);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.BridgeSlopeUp);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.RoadCrossLeft);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.BridgeSlopeDown);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.RoadStraight);
        ConnectFromFirstAvailable(start, next);

        follow_player_controller = follow_player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update ()
    {
        DeactivateChainBackwards(follow_player_controller.below);
	}
}
