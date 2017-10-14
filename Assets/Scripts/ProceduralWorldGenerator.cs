using System;
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
    private List<PrefabProperties.Prefab> random_pieces;

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
        if(game_object_offsets == null || game_object_offsets.taken.Length < 2)
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
            if (game_object_offsets.taken[m] != null && game_object_offsets.taken[m] != from_exclusive)
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

    void GenerateForward(GameObject from, int max_nesting_level)
    {
        GenerateForward(from, 0, max_nesting_level);
    }

    void GenerateForward(GameObject from, int nesting_level, int max_nesting_level)
    {
        if(from == null)
        {
            return;
        }

        ConnectionOffsets game_object_offsets = from.GetComponent<ConnectionOffsets>();

        if(game_object_offsets == null)
        {
            return;
        }

        
        for (int i = 1; i < game_object_offsets.taken.Length; ++i) // generate max 4 pieces forward
        {
            if(game_object_offsets.taken[i] == null)
            {
                PrefabProperties.Prefab piece = random_pieces[random.Next() % random_pieces.Count];
                ConnectFromFirstAvailable(from, Activate(piece));
            }

            if (nesting_level + 1 <= max_nesting_level)
            {
                GenerateForward(game_object_offsets.taken[i], nesting_level + 1, max_nesting_level);
            }
        }
    }

    bool ConnectFromFirstAvailable(GameObject from, GameObject to, int first_available = 0)
    {
        if(from == null || to == null)
        {
            return false;
        }

        ConnectionOffsets from_offsets = from.GetComponent<ConnectionOffsets>();

        if(from_offsets == null)
        {
            return false;
        }

        int taken_idx = first_available;
        for (int i = first_available; i < from_offsets.taken.Length; ++i)
        {
            if(!from_offsets.taken[i])
            {
                taken_idx = i;
                break;
            }
        }

        if (taken_idx < from_offsets.taken.Length && from_offsets.taken[taken_idx])
        {
            return false;
        }

        ConnectionOffsets to_offsets = to.GetComponent<ConnectionOffsets>();

        if(to_offsets == null || to_offsets.taken[0] != null)
        {
            return false;
        }

        to_offsets.taken[0] = from;
        from_offsets.taken[taken_idx] = to;

        Vector3 total_relative_pos = from_offsets.position_offsets[taken_idx] - to_offsets.position_offsets[0];
        Vector3 total_relative_rot = from_offsets.rotation_offsets[taken_idx] - to_offsets.rotation_offsets[0];

        to.transform.position = from.transform.position + total_relative_pos;
        to.transform.eulerAngles = from.transform.eulerAngles + total_relative_rot;

        /*to.transform.Translate(total_relative_pos, from.transform);*/
        
        return true;
    }

    // Use this for initialization
    void Start ()
    {
        random = new System.Random(1337);
        random_pieces = new List<PrefabProperties.Prefab>();

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
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_A_Corrected", 3);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_B_Corrected", 3);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Intersection_A_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Streight_Corrected", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Intersection_B_Corrected", 5);

        random_pieces.Add(PrefabProperties.Prefab.BridgeDamage);
        random_pieces.Add(PrefabProperties.Prefab.BridgeStraight);
        random_pieces.Add(PrefabProperties.Prefab.BridgeSlopeUp);
        random_pieces.Add(PrefabProperties.Prefab.BridgeSlopeDown);
        random_pieces.Add(PrefabProperties.Prefab.RoadBusStop);
        random_pieces.Add(PrefabProperties.Prefab.RoadCrossA);
        random_pieces.Add(PrefabProperties.Prefab.RoadCrossB);
        random_pieces.Add(PrefabProperties.Prefab.RoadCrossRight);
        random_pieces.Add(PrefabProperties.Prefab.RoadCrosswalk);
        random_pieces.Add(PrefabProperties.Prefab.RoadStraight);
        random_pieces.Add(PrefabProperties.Prefab.RoadCrossLeft);

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

        next = Activate(PrefabProperties.Prefab.BridgeSlopeUp);
        ConnectFromFirstAvailable(start, next);

        /*start = next;
        next = Activate(PrefabProperties.Prefab.RoadCrossLeft);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.BridgeSlopeDown);
        ConnectFromFirstAvailable(start, next);

        start = next;
        next = Activate(PrefabProperties.Prefab.RoadStraight);
        ConnectFromFirstAvailable(start, next);*/

        follow_player_controller = follow_player.GetComponent<PlayerController>();
    }


    // Update is called once per frame
    void Update ()
    {
        DeactivateChainBackwards(follow_player_controller.below);
        //GenerateForward(follow_player_controller.below, 1);
    }
}
