using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 300f;
    public float default_speed = 300f;
    public float turnSpeed = 500f;
    public float seconds_stuck_lose_life = 0.50f;
    public int lives = 3;
    public float health = 1000.0f;
    public float max_health = 1000.0f;
    public float lose_life_per_second_stuck = 666.0f;
    public float speed_increase_per_minute = 50.0f;

    private float actual_speed;
    private float powerInput;
    private float turnInput;
    private float speed_increade_per_delta_t;

    public Text livesText;

    public Rechargeable jump_recharge;
    public Image jump_charge_ui;
    private ProgressBar.ProgressRadialBehaviour jump_charge;

    public Image health_charge_ui;
    private ProgressBar.ProgressRadialBehaviour health_charge;

    private Rigidbody rigidbody;
    private HoverEngine hover_engine;
    private HeadingRotator heading_rotator;
    private Scorer scorer;
    private CheckpointAble checkpointable;

    private float seconds_stuck_last;
    private int old_lives = -1;
    private float speed_multiplier_calculated;
    private float awoken_time;
    private bool awoken_complete;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        hover_engine = GetComponent<HoverEngine>();
        heading_rotator = GetComponent<HeadingRotator>();
        scorer = GetComponent<Scorer>();
        checkpointable = GetComponent<CheckpointAble>();

        jump_charge = jump_charge_ui.GetComponent<ProgressBar.ProgressRadialBehaviour>();
        health_charge = health_charge_ui.GetComponent<ProgressBar.ProgressRadialBehaviour>();

        powerInput = 0.0f;
        old_lives = -1;
        actual_speed = speed;
        speed_increade_per_delta_t = speed_increase_per_minute / 60.0f;
        awoken_time = Time.time;
        awoken_complete = false;
    }

    void UpdateLivesText()
    {
        if(old_lives != lives)
        {
            old_lives = lives;
            livesText.text = lives.ToString() + " Lives";
        }
    }

    void Update()
    {
        if(!awoken_complete && Time.time - awoken_time > 1.0f)
        {
            awoken_complete = true;
            powerInput = 1.0f;
        }

        actual_speed += Time.deltaTime * speed_increade_per_delta_t;
        speed_multiplier_calculated = actual_speed / speed;
        hover_engine.hoverMultiplier = speed_multiplier_calculated;
        scorer.multiplier = Mathf.Sqrt(speed_multiplier_calculated);
        
        //powerInput = Input.GetAxis("Vertical");

        turnInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hover_engine.jumpMultiplier = jump_recharge.UseCharge();
            hover_engine.Jump();
        }
        if (Input.GetKeyDown("e"))
        {
            heading_rotator.Right();
        }
        if (Input.GetKeyDown("q"))
        {
            heading_rotator.Left();
        }

        if (health <= 0.0f)
        {
            checkpointable.Reset();
            health = max_health;
            lives -= 1;
            seconds_stuck_last = hover_engine.seconds_stuck - seconds_stuck_lose_life;
            actual_speed = speed;

            if(lives == 0)
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

        jump_charge.Value = jump_recharge.percentage * 100.0f;
        health_charge.Value = health / max_health * 100.0f;

        UpdateLivesText();

    }

    private void FixedUpdate()
    {
        rigidbody.AddRelativeForce(turnInput * turnSpeed * speed_multiplier_calculated, 0f, powerInput * actual_speed);
    }

}