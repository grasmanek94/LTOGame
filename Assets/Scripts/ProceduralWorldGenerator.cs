using System.Collections;
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

        return true;
    }

    // Use this for initialization
    void Start ()
    {
        instances = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();
        inactive = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();
        active = new Dictionary<PrefabProperties.Prefab, List<GameObject>>();

        InstantiatePrefabs("LowpolyStreetPack /Prefabs/Roads/Bridges/Elements/Bridge_Damage", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/Bridge_Simple_Straight", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Bridges/Elements/BridgeSlope_Simple", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_BusStop", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Cross_A_A", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Cross_A_B", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Crosswalk", 10);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_A", 1);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_End_B", 1);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Intersection_A", 5);
        InstantiatePrefabs("LowpolyStreetPack/Prefabs/Roads/Streets/Road_Streight", 10);

        creation_tick = 0;


    }

    // Update is called once per frame
    void Update ()
    {

	}
}
