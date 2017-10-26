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
    private PlayerSoundController sound;

    // Use this for initialization
    void Awake ()
    {
        score = 0.0f;
        old_score = 0.0f;
        sound = GetComponent<PlayerSoundController>();
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
            float add_score = scoreable.TriggerScore * multiplier;
            score += add_score;
            sound.Coin(add_score);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        Scoreable scoreable = other.gameObject.GetComponent<Scoreable>();
        if (scoreable != null && scoreable.Collision)
        {
            float add_score = scoreable.CollisionScore * multiplier;
            score += add_score;
            sound.Coin(add_score);
        }
    }
}
