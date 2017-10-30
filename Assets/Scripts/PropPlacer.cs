using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropPlacer : MonoBehaviour {

    [System.Serializable]
    public class SimpleTransform
    {
        public Vector3 position; // min position when area
        public Vector3 rotation; // min rotation when area
        public bool area;
        public Vector3 max_position;
        public Vector3 max_rotation;

        public Vector3 GetPos()
        {
            if (!area)
            {
                return position;
            }
            Vector3 toret = new Vector3();

            toret.x = Random.Range(position.x, max_position.x);
            toret.y = Random.Range(position.y, max_position.y);
            toret.z = Random.Range(position.z, max_position.z);

            return toret;
        }

        public Vector3 GetRot()
        {
            if (!area)
            {
                return rotation;
            }
            Vector3 toret = new Vector3();

            toret.x = Random.Range(rotation.x, max_rotation.x);
            toret.y = Random.Range(rotation.y, max_rotation.y);
            toret.z = Random.Range(rotation.z, max_rotation.z);

            return toret;
        }
    }
    
    [System.Serializable]
    public class ProbableOffset
    {
        public float probability;
        public SimpleTransform[] offsets;
    }

    [System.Serializable]
    public class PropPoint
    {
        public ProbableOffset[] probable_offsets;

        private float probability_sum = 0.0f;
        public void CalculateProbability()
        {
            foreach (ProbableOffset probable_offset in probable_offsets)
            {
                probability_sum += probable_offset.probability;
            }

            if (probability_sum < 1.0f)
            {
                probability_sum = 1.0f;
            }
        }

        public ProbableOffset GetByProbability()
        {
            float r = Random.Range(0.0f, 0.99999999f) * probability_sum;

            double sum = 0;
            foreach (ProbableOffset n in probable_offsets)
            {
                // loop until the random number is less than our cumulative probability
                if (r <= (sum = sum + n.probability))
                {
                    return n;
                }
            }

            return null;
        }
    }

    public PropPoint[] prop_points;
    public PropPoint[] block_points;

    private List<GameObject> attached_props;
    private List<GameObject> attached_blocks;

    private void Awake()
    {
        attached_props = new List<GameObject>();
        attached_blocks = new List<GameObject>();

        foreach (PropPoint point in prop_points)
        {
            point.CalculateProbability();
        }

        foreach (PropPoint point in block_points)
        {
            point.CalculateProbability();
        }
    }

    private void OnEnable()
    {
        foreach (PropPoint point in prop_points)
        {
            ProbableOffset po = point.GetByProbability();
            if (po != null)
            {
                foreach (SimpleTransform offset in po.offsets)
                {
                    GameObject game_object = PropManager.GetRandomProp();
                    SimpleTransform simple_transform = game_object.GetComponent<TransformOffset>().offset;

                    Vector3 offset_pos = offset.GetPos();
                    offset_pos += simple_transform.position;
                    Vector3 offset_rot = offset.GetRot();
                    offset_rot += simple_transform.rotation;

                    attached_props.Add(game_object);

                    game_object.transform.parent = transform;
                    game_object.transform.localPosition = offset_pos;
                    game_object.transform.localRotation = Quaternion.Euler(offset_rot);
                }
            }
        }

        foreach (PropPoint point in block_points)
        {
            ProbableOffset po = point.GetByProbability();
            if (po != null)
            {
                foreach (SimpleTransform offset in po.offsets)
                {
                    GameObject game_object = PropManager.GetRandomBlock();
                    SimpleTransform simple_transform = game_object.GetComponent<TransformOffset>().offset;
                    attached_blocks.Add(game_object);
                    game_object.transform.parent = transform;
                    game_object.transform.localPosition = offset.GetPos() + simple_transform.position;
                    game_object.transform.localRotation = Quaternion.Euler(offset.GetRot() + simple_transform.rotation);
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach(GameObject game_object in attached_props)
        {
            PropManager.ReturnProp(game_object);
        }
        attached_props.Clear();

        foreach (GameObject game_object in attached_blocks)
        {
            PropManager.ReturnBlock(game_object);
        }
        attached_blocks.Clear();
    }
}
