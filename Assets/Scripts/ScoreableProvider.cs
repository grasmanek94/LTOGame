using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreableProvider : MonoBehaviour {

    System.Random random;
    public Vector3[] positions;

    private bool Do(double probability)
    {
        return random.NextDouble() < probability;
    }

    public enum Difficulty
    {
        RANDOM,
        EASY,
        MEDIUM,
        HARD,
    }

    void Awake()
    {
        random = new System.Random();
    }

    void OnEnable ()
    {
		
	}

    void OnDisable()
    {
        
    }
}
