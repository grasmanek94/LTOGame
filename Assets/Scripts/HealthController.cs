using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthController : MonoBehaviour {

    public float seconds_stuck_lose_life = 0.50f;
    public int lives = 3;
    public float health = 1000.0f;
    public float max_health = 1000.0f;
    public float lose_life_per_second_stuck = 666.0f;
    public float life_regen_factor = 75.0f;
    public float health_collision_factor = 0.75f;

    public Text livesText;

    public Image health_charge_ui;
    private ProgressBar.ProgressRadialBehaviour health_charge;

    private float seconds_stuck_last;
    private int old_lives = -1;
    private float life_regen_factor_calculated;
    private HoverEngine hover_engine;
    private Scorer scorer;
    private PlayerController player_controller;

    // Use this for initialization
    void Start ()
    {
        hover_engine = GetComponent<HoverEngine>();
        scorer = GetComponent<Scorer>();
        player_controller = GetComponent<PlayerController>();

        health_charge = health_charge_ui.GetComponent<ProgressBar.ProgressRadialBehaviour>();
        old_lives = -1;
    }

    void UpdateLivesText()
    {
        if (old_lives != lives)
        {
            old_lives = lives;
            livesText.text = lives.ToString() + " Lives";
        }
    }

    // Update is called once per frame
    void Update ()
    {
        life_regen_factor_calculated = Mathf.Sqrt(life_regen_factor * player_controller.GetSpeedMultiplierSqrt());

        if (health < max_health)
        {
            health += life_regen_factor_calculated * Time.deltaTime;
        }
        else if (health > max_health)
        {
            health = max_health;
        }

        if (health <= 0.0f)
        {      
            health = max_health;
            lives -= 1;
            seconds_stuck_last = hover_engine.seconds_stuck - seconds_stuck_lose_life;
            player_controller.ResetLife();

            if (lives == 0)
            {
                SharedObject.Set("score", scorer.score);
                SceneManager.LoadScene(2); // game over scene
            }
        }
        else if (hover_engine.seconds_stuck >= seconds_stuck_lose_life)
        {
            // lose life
            float delta_s = hover_engine.seconds_stuck - seconds_stuck_lose_life;
            float delta_t = delta_s - seconds_stuck_last;
            seconds_stuck_last = delta_s;
            health -= delta_t * lose_life_per_second_stuck;
        }
        else
        {
            seconds_stuck_last = 0.0f;
        }
        health_charge.Value = health / max_health * 100.0f;

        UpdateLivesText();
    }

    void OnCollisionEnter(Collision collision)
    {
        float mag = collision.impulse.magnitude;
        health -= mag * health_collision_factor;
    }

}
