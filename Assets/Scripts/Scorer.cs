using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scorer : MonoBehaviour {

    public float score
    {
        get;
        private set;
    }

    public float multiplier = 1.0f;
    private float old_score;
    public Text scoreText;

    // Use this for initialization
    void Awake ()
    {
        score = 0.0f;
        old_score = 0.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        if (old_score != score)
        {
            old_score = score;
            if (scoreText != null)
            {
                scoreText.text = "Score: " + ((int)score).ToString();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Scoreable scoreable = other.gameObject.GetComponent<Scoreable>();
        if (scoreable != null && scoreable.Trigger)
        {
            score += (scoreable.TriggerScore * multiplier);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        Scoreable scoreable = other.gameObject.GetComponent<Scoreable>();
        if (scoreable != null && scoreable.Collision)
        {
            score += (scoreable.CollisionScore * multiplier);
        }
    }
}
