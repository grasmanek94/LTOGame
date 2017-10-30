using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropManager : MonoBehaviour {

    public enum PropType
    {
        Bench_A,
        Bench_B,
        Bench_C,
        FuseBox,
        Hidrant,
        MailBox,
        NewsBoard,
        ParkLamp,
        Phone,
        PlantPot_A_V01,
        PlantPot_A_V02,
        PlantPot_A_V03,
        PlantPot_A_V04,
        PlantPot_A_V05,
        PlantPot_A_V06,
        PlantPot_B_V01,
        PlantPot_B_V02,
        PlantPot_B_V03,
        PlantPot_C_V01,
        PlantPot_C_V02,
        PlantPot_C_V03,
        PlantPot_D_V01,
        PlantPot_D_V02,
        PlantPot_D_V03
    }

    public enum BlockType
    {
        RoadBlock_A,
        RoadBlock_B,
        RoadBlock_C,
        RoadBlock_D
    }

    private class InstanceManager<T>
    {
        private Dictionary<T, GameObject> instances;
        private List<T> t_types;
        private List<GameObject> storage_instance;
        private HashSet<GameObject> storage_check;

        public InstanceManager()
        {
            storage_check = new HashSet<GameObject>();
            storage_instance = new List<GameObject>();
            instances = new Dictionary<T, GameObject>();
            t_types = new List<T>();
        }

        public void Instantiate(T type, string resource)
        {
            if(!t_types.Contains(type))
            {
                t_types.Add(type);
            }
            instances.Add(type, (GameObject)Resources.Load(resource));
        }

        public GameObject GetRandom()
        {
            GameObject game_object;

            if (storage_instance.Count < 1)
            {
                T type = t_types[Random.Range(0, t_types.Count)];
                game_object = Object.Instantiate(instances[type]);
                return game_object;
            }

            int where = Random.Range(0, storage_instance.Count);
            game_object = storage_instance[where];
            storage_instance.RemoveAt(where);
            storage_check.Remove(game_object);

            return game_object;
        }

        public void Return(GameObject game_object)
        {
            if (storage_check.Contains(game_object))
            {
                return;
            }

            storage_instance.Add(game_object);
        }
    }

    private static bool initialised = false;
    private static InstanceManager<PropType> props;
    private static InstanceManager<BlockType> blocks;

    public static GameObject GetRandomProp()
    {
        Init();
        return props.GetRandom();
    }

    public static void ReturnProp(GameObject game_object)
    {
        Init();
        props.Return(game_object);
        //game_object.transform.parent = instance.transform;
    }

    public static GameObject GetRandomBlock()
    {
        Init();
        return blocks.GetRandom();
    }

    public static void ReturnBlock(GameObject game_object)
    {
        Init();
        blocks.Return(game_object);
        //game_object.transform.parent = instance.transform;
    }

    private static void Init()
    {
        if (!initialised)
        {
            initialised = true;
            props = new InstanceManager<PropType>();

            props.Instantiate(PropType.Bench_A, "LowpolyStreetPack/Prefabs/StreetProps/Bench/Bench_A");
            props.Instantiate(PropType.Bench_B, "LowpolyStreetPack/Prefabs/StreetProps/Bench/Bench_B");
            props.Instantiate(PropType.Bench_C, "LowpolyStreetPack/Prefabs/StreetProps/Bench/Bench_C");
            props.Instantiate(PropType.FuseBox, "LowpolyStreetPack/Prefabs/StreetProps/FuseBox/FuseBox");
            props.Instantiate(PropType.Hidrant, "LowpolyStreetPack/Prefabs/StreetProps/Hidrant/Hidrant");
            props.Instantiate(PropType.MailBox, "LowpolyStreetPack/Prefabs/StreetProps/MailBox/MailBox");
            props.Instantiate(PropType.NewsBoard, "LowpolyStreetPack/Prefabs/StreetProps/NewsBoard/NewsBoard");
            props.Instantiate(PropType.ParkLamp, "LowpolyStreetPack/Prefabs/StreetProps/ParkLamp/ParkLamp");
            props.Instantiate(PropType.Phone, "LowpolyStreetPack/Prefabs/StreetProps/Phone/Phone");
            props.Instantiate(PropType.PlantPot_A_V01, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_A_V01");
            props.Instantiate(PropType.PlantPot_A_V02, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_A_V02");
            props.Instantiate(PropType.PlantPot_A_V03, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_A_V03");
            props.Instantiate(PropType.PlantPot_A_V04, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_A_V04");
            props.Instantiate(PropType.PlantPot_A_V05, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_A_V06");
            props.Instantiate(PropType.PlantPot_A_V06, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_A_V06");
            props.Instantiate(PropType.PlantPot_B_V01, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_B_V01");
            props.Instantiate(PropType.PlantPot_B_V02, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_B_V02");
            props.Instantiate(PropType.PlantPot_B_V03, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_B_V03");
            props.Instantiate(PropType.PlantPot_C_V01, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_C_V01");
            props.Instantiate(PropType.PlantPot_C_V02, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_C_V02");
            props.Instantiate(PropType.PlantPot_C_V03, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_C_V03");
            props.Instantiate(PropType.PlantPot_D_V01, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_D_V01");
            props.Instantiate(PropType.PlantPot_D_V02, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_D_V02");
            props.Instantiate(PropType.PlantPot_D_V03, "LowpolyStreetPack/Prefabs/StreetProps/PlantPots/QuickBuild/PlantPot_D_V03");

            blocks = new InstanceManager<BlockType>();

            blocks.Instantiate(BlockType.RoadBlock_A, "LowpolyStreetPack/Prefabs/StreetProps/RoadBlocks/RoadBlock_A");
            blocks.Instantiate(BlockType.RoadBlock_B, "LowpolyStreetPack/Prefabs/StreetProps/RoadBlocks/RoadBlock_B");
            blocks.Instantiate(BlockType.RoadBlock_C, "LowpolyStreetPack/Prefabs/StreetProps/RoadBlocks/RoadBlock_C");
            blocks.Instantiate(BlockType.RoadBlock_D, "LowpolyStreetPack/Prefabs/StreetProps/RoadBlocks/RoadBlock_D");
        }
    }

    private void Awake()
    {
        Init();
    }
}
