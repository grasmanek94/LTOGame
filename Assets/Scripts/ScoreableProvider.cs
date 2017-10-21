using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreableProvider : MonoBehaviour {

    [System.Serializable]
    public class ProbableOffset
    {
        public float probability;
        public Vector3[] offsets;
    }

    [System.Serializable]
    public class ScorePoint
    {
        public Scoreable.Difficulty difficulty;

        public ProbableOffset[] score_point_set;

        private float probability_sum = 0.0f;
        public int MaximumOffsets()
        {
            int max = 0;
            foreach (ProbableOffset probable_offset in score_point_set)
            {
                probability_sum += probable_offset.probability;
                if (probable_offset.offsets.Length > max)
                {
                    max = probable_offset.offsets.Length;
                }
            }

            if(probability_sum < 1.0f)
            {
                probability_sum = 1.0f;
            }

            return max;
        }

        public ProbableOffset GetByProbability()
        {
            float r = Random.Range(0.0f, 0.99999999f) * probability_sum;

            double sum = 0;
            foreach (ProbableOffset n in score_point_set)
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

    public ScorePoint[] score_points;

    private Dictionary<Scoreable.Difficulty, List<GameObject>> inactive;
    private HashSet<GameObject> active;

    void InstantiatePrefabs(string resource, Scoreable.Difficulty which, int count)
    {
        if (count < 1)
        {
            return;
        }

        GameObject original = (GameObject)Resources.Load(resource);

        if (!inactive.ContainsKey(which))
        {
            inactive.Add(which, new List<GameObject>());
        }

        while (count-- > 0)
        {
            GameObject game_object = Instantiate(original, transform);

            game_object.SetActive(false);

            inactive[which].Add(game_object);
        }
    }

    GameObject Activate(Scoreable.Difficulty which, Vector3 position)
    {
        if (inactive[which].Count < 1)
        {
            return null;
        }

        GameObject game_object = inactive[which][0];

        inactive[which].RemoveAt(0);
        active.Add(game_object);

        game_object.transform.localPosition = position;
        game_object.SetActive(true);

        return game_object;
    }

    bool Deactivate(GameObject game_object)
    {

        if (!active.Contains(game_object))
        {
            return false;
        }

        Scoreable.Difficulty which = game_object.GetComponent<Scoreable>().difficulty;

        active.Remove(game_object);
        inactive[which].Add(game_object);

        game_object.SetActive(false);

        return true;
    }

    void Awake()
    {
        inactive = new Dictionary<Scoreable.Difficulty, List<GameObject>>();
        active = new HashSet<GameObject>();

        Dictionary<Scoreable.Difficulty, int> counts = new Dictionary<Scoreable.Difficulty, int>();
        foreach (Scoreable.Difficulty which in System.Enum.GetValues(typeof(Scoreable.Difficulty)))
        {
            counts.Add(which, 0);
        }

        foreach (ScorePoint point in score_points)
        {
            counts[point.difficulty] += point.MaximumOffsets();
        }

        InstantiatePrefabs("Game/PickupQuestionMarkYellowBig", Scoreable.Difficulty.EASY, counts[Scoreable.Difficulty.EASY]);
        InstantiatePrefabs("Game/PickupQuestionMarkYellowSmall", Scoreable.Difficulty.MEDIUM, counts[Scoreable.Difficulty.MEDIUM]);
        InstantiatePrefabs("Game/PickupQuestionMarkRedBig", Scoreable.Difficulty.HARD, counts[Scoreable.Difficulty.HARD]);
        InstantiatePrefabs("Game/PickupQuestionMarkRedSmall", Scoreable.Difficulty.EXTREME, counts[Scoreable.Difficulty.EXTREME]);
    }

    void OnEnable ()
    {
        foreach (ScorePoint point in score_points)
        {
            ProbableOffset po = point.GetByProbability();
            if(po != null)
            {
                foreach (Vector3 offset in po.offsets)
                {
                    Activate(point.difficulty, offset);
                }
            }
        }
    }

    void OnDisable()
    {
        while(active.Count > 0)
        {
            Deactivate(active.First());
        }
    }
}
