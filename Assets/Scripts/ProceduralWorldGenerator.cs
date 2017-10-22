using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWorldGenerator : MonoBehaviour {
    
    public GameObject follow_player;

    public int low_chance_piece;
    public int low_chance_piece_randomness;
    public int seed;

    private HoverEngine player_hover_engine;

    private Dictionary<PrefabProperties.Prefab, GameObject> originals;
    private Dictionary<PrefabProperties.Prefab, List<GameObject>> inactive;
    private Dictionary<PrefabProperties.Prefab, List<GameObject>> active;

    private int creation_tick;
    private int end_tick;

    private System.Random random;
    private List<PrefabProperties.Prefab> random_pieces_normal_chance;
    private List<PrefabProperties.Prefab> random_pieces_low_chance;

    private static Vector3 far_away_position = new Vector3(-200000000.0f, -200000000.0f, -200000000.0f);
    private static Quaternion zero_quat = Quaternion.Euler(0.0f, 0.0f, 0.0f);

    void InstantiatePrefabs(PrefabProperties.Prefab which, int count)
    {
        if (count < 1 || !originals.ContainsKey(which))
        {
            return;
        }

        GameObject original = originals[which];
    
        if (!inactive.ContainsKey(which))
        {
            inactive.Add(which, new List<GameObject>());
            active.Add(which, new List<GameObject>());
        }

        while (count-- > 0)
        {
            GameObject game_object = Instantiate(original, far_away_position, zero_quat);

            game_object.SetActive(false);
            ConnectionOffsets game_object_offsets = game_object.GetComponent<ConnectionOffsets>();
            game_object_offsets.ResetTaken();

            inactive[which].Add(game_object);
        }
    }

    void InstantiatePrefabs(string resource, int count = 5)
    {
        if(count < 1)
        {
            return;
        }

        GameObject original = (GameObject)Resources.Load(resource);
        PrefabProperties.Prefab which = original.GetComponent<PrefabProperties>().prefab;
        if (originals.ContainsKey(which))
        {
            original = originals[which];
        }
        else
        {
            originals.Add(which, original);
        }

        InstantiatePrefabs(which, count);
    }

    GameObject Activate(PrefabProperties.Prefab which)
    {
        if(inactive[which].Count < 1)
        {
            InstantiatePrefabs(which, 1);
        }

        GameObject game_object = inactive[which][0];

        inactive[which].RemoveAt(0);
        active[which].Add(game_object);

        game_object.SetActive(true);
        ConnectionOffsets game_object_offsets = game_object.GetComponent<ConnectionOffsets>();
        game_object.transform.SetPositionAndRotation(far_away_position, zero_quat);
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
        game_object.transform.SetPositionAndRotation(far_away_position, zero_quat);
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
        ++creation_tick;
        GenerateForward(from, 0, max_nesting_level);
    }

    void GenerateForward(GameObject from, int nesting_level, int max_nesting_level)
    {
        if(from == null)
        {
            return;
        }

        ConnectionOffsets game_object_offsets = from.GetComponent<ConnectionOffsets>();
        PrefabProperties game_object_prefabprops = from.GetComponent<PrefabProperties>();

        if (game_object_offsets == null)
        {
            return;
        }
        
        for (int i = 1; i < game_object_offsets.taken.Length; ++i) // generate max 4 pieces forward
        {
            if(game_object_offsets.taken[i] == null)
            {
                PrefabProperties.Prefab piece;
                if (!random_pieces_low_chance.Contains(game_object_prefabprops.prefab) && creation_tick > (low_chance_piece + (random.Next() % low_chance_piece_randomness)))
                {
                    creation_tick = 0;
                    piece = random_pieces_low_chance[random.Next() % random_pieces_low_chance.Count];
                }
                else
                {
                    piece = random_pieces_normal_chance[random.Next() % random_pieces_normal_chance.Count];
                }

                if (game_object_offsets.taken.Length >= 3 && 
                    (end_tick > ((low_chance_piece/2) + (random.Next() % (low_chance_piece_randomness / 2)))))
                {
                    if(end_tick % 2 == 0)
                    {
                        ConnectFromFirstAvailable(from, Activate(PrefabProperties.Prefab.RoadEndC));
                    }
                    else
                    {
                        ConnectFromFirstAvailable(from, Activate(PrefabProperties.Prefab.RoadEndD));
                    }
                    end_tick = 1;
                }
                else
                {
                    ++end_tick;
                    ConnectFromFirstAvailable(from, Activate(piece));
                }
            }

            if (nesting_level + 1 <= max_nesting_level)
            {
                GenerateForward(game_object_offsets.taken[i], nesting_level + 1, max_nesting_level);
            }
        }
    }

    private Vector2 RotatePointAroundPoint(float point_x, float point_y, float angle)
    {
        return RotatePointAroundPoint(new Vector2(point_x, point_y), angle);
    }

    private Vector2 RotatePointAroundPoint(Vector2 point, float angle)
    {
        float x1 = point.x;
        float y1 = point.y;

        angle *= Mathf.Deg2Rad;

        float x2 = x1 * Mathf.Cos(angle) - y1 * Mathf.Sin(angle);
        float y2 = x1 * Mathf.Sin(angle) + y1 * Mathf.Cos(angle);

        point.x = x2;
        point.y = y2;

        return point;
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

        Vector3 to_position_offsets = to_offsets.position_offsets[0];
        Vector3 to_rotation_offsets = to_offsets.rotation_offsets[0];
        Vector3 from_position_offsets = from_offsets.position_offsets[taken_idx];
        Vector3 from_rotation_offsets = from_offsets.rotation_offsets[taken_idx];

        Vector2 correction;

        // correct position
        correction = RotatePointAroundPoint(to_position_offsets.z, to_position_offsets.y, from_rotation_offsets.x);
        to_position_offsets.z = correction.x;
        to_position_offsets.y = correction.y;

        correction = RotatePointAroundPoint(to_position_offsets.z, to_position_offsets.x, from_rotation_offsets.y);
        to_position_offsets.z = correction.x;
        to_position_offsets.x = correction.y;

        correction = RotatePointAroundPoint(to_position_offsets.x, to_position_offsets.y, from_rotation_offsets.z);
        to_position_offsets.x = correction.x;
        to_position_offsets.y = correction.y;

        // apply transforms (and correct rotation)
        to.transform.rotation = Quaternion.Euler(from.transform.eulerAngles);
        to.transform.Rotate(from_rotation_offsets, Space.Self);
        to.transform.Rotate(to_rotation_offsets, Space.Self);
        to.transform.position = from.transform.TransformPoint(from_position_offsets - to_position_offsets); 

        return true;
    }

    // Use this for initialization
    void Start ()
    {
        random = new System.Random(seed);
        random_pieces_normal_chance = new List<PrefabProperties.Prefab>();
        random_pieces_low_chance = new List<PrefabProperties.Prefab>();

        inactive = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();
        active = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();
        originals = new Dictionary<PrefabProperties.Prefab, GameObject>();

        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Damage_Corrected_L");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Simple_Straight_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Slope_Down_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Slope_Up_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_BusStop_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_BusStop_Corrected_180");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Cross_A_A_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Cross_A_B_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Crosswalk_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_A_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_B_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_A_Corrected_180");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_B_Corrected_180");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Intersection_A_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Streight_Corrected");
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Intersection_B_Corrected");

        random_pieces_normal_chance.Add(PrefabProperties.Prefab.BridgeDamage);
        random_pieces_normal_chance.Add(PrefabProperties.Prefab.BridgeStraight);
        random_pieces_normal_chance.Add(PrefabProperties.Prefab.BridgeSlopeUp);
        random_pieces_normal_chance.Add(PrefabProperties.Prefab.BridgeSlopeDown);
        random_pieces_normal_chance.Add(PrefabProperties.Prefab.RoadBusStop);
        random_pieces_normal_chance.Add(PrefabProperties.Prefab.RoadBusStopB);
        random_pieces_normal_chance.Add(PrefabProperties.Prefab.RoadCrosswalk);
        random_pieces_normal_chance.Add(PrefabProperties.Prefab.RoadStraight);

        random_pieces_low_chance.Add(PrefabProperties.Prefab.RoadCrossA);
        random_pieces_low_chance.Add(PrefabProperties.Prefab.RoadCrossB);
        random_pieces_low_chance.Add(PrefabProperties.Prefab.RoadCrossRight);
        random_pieces_low_chance.Add(PrefabProperties.Prefab.RoadCrossLeft);

        creation_tick = 0;
        end_tick = 0;

        GameObject start = Activate(PrefabProperties.Prefab.RoadEndA);
        start.transform.position = new Vector3(0.0f, -1.33f, 2.0f);
        start.transform.rotation = Quaternion.Euler(0, 0, 0);

        GameObject next = Activate(PrefabProperties.Prefab.RoadStraight);
        ConnectFromFirstAvailable(start, next);

        player_hover_engine = follow_player.GetComponent<HoverEngine>();
    }

    // Update is called once per frame
    void Update ()
    {
        DeactivateChainBackwards(player_hover_engine.below);
        GenerateForward(player_hover_engine.below, 4);
    }
}
