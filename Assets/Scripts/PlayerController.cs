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

    private float powerInput;
    private float turnInput;

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
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        hover_engine = GetComponent<HoverEngine>();
        heading_rotator = GetComponent<HeadingRotator>();
        scorer = GetComponent<Scorer>();
        checkpointable = GetComponent<CheckpointAble>();

        jump_charge = jump_charge_ui.GetComponent<ProgressBar.ProgressRadialBehaviour>();
        health_charge = health_charge_ui.GetComponent<ProgressBar.ProgressRadialBehaviour>();

        powerInput = 1.0f;
        old_lives = -1;
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
        powerInput = Input.GetAxis("Vertical");

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

        jump_charge.Value = 100.0f * jump_recharge.percentage;
        health_charge.Value = health / max_health * 100.0f;

        UpdateLivesText();

    }

    private void FixedUpdate()
    {
        rigidbody.AddRelativeForce(turnInput * turnSpeed, 0f, powerInput * speed);
    }

}