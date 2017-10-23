using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Lean.Touch;
using System;

public class PlayerController : MonoBehaviour
{
    public float speed = 300f;
    public float speed_limit = 750.0f;
    public float turnSpeed = 500f;
    public float seconds_stuck_lose_life = 0.50f;
    public int lives = 3;
    public float health = 1000.0f;
    public float max_health = 1000.0f;
    public float lose_life_per_second_stuck = 666.0f;
    public float speed_increase_per_minute = 50.0f;
    public float life_regen_factor = 75.0f;
    public float health_collision_factor = 0.75f;
    
    [SerializeField]
    private float actual_speed;

    private float powerInput;
    private float turnInput;
    private float speed_increase_per_delta_t;

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
    private float life_regen_factor_calculated;

    private bool is_touch_input;
    private float current_accelerometer_x;
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
        speed_increase_per_delta_t = speed_increase_per_minute / 60.0f;
        awoken_time = Time.time;
        awoken_complete = false;
        is_touch_input = false;
        current_accelerometer_x = Input.acceleration.x;
    }

    void UpdateLivesText()
    {
        if(old_lives != lives)
        {
            old_lives = lives;
            livesText.text = lives.ToString() + " Lives";
        }
    }

    void Jump()
    {
        hover_engine.jumpMultiplier = jump_recharge.UseCharge();
        hover_engine.Jump();
    }

    void Left()
    {
        heading_rotator.Left();
    }

    void Right()
    {
        heading_rotator.Right();
    }

    void Update()
    {
        //powerInput = Input.GetAxis("Vertical");
        ProcessControls();

        if (!awoken_complete && Time.time - awoken_time > 1.0f)
        {
            awoken_complete = true;
            powerInput = 1.0f;
        }

        if (actual_speed < speed_limit)
        {
            actual_speed += Time.deltaTime * speed_increase_per_delta_t;
            if(actual_speed > speed_limit)
            {
                actual_speed = speed_limit;
            }
        }

        speed_multiplier_calculated = actual_speed / speed;
        float speed_mult_sqrt = Mathf.Sqrt(speed_multiplier_calculated);
        hover_engine.hoverMultiplier = speed_mult_sqrt;
        scorer.multiplier = speed_mult_sqrt;
        life_regen_factor_calculated = Mathf.Sqrt(life_regen_factor * speed_mult_sqrt);

        if (health < max_health)
        {
            health += life_regen_factor_calculated * Time.deltaTime;
        }
        else if(health > max_health)
        {
            health = max_health;
        }

        if (health <= 0.0f)
        {
            awoken_complete = false;
            awoken_time = Time.time;
            powerInput = 0.0f;
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

    void FixedUpdate()
    {
        rigidbody.AddRelativeForce(turnInput * turnSpeed * speed_multiplier_calculated, 0f, powerInput * actual_speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        float mag = collision.impulse.magnitude;
        health -= mag * health_collision_factor;
        actual_speed -= Mathf.Sqrt(mag);
    }

    void ProcessControls()
    {
        if(!awoken_complete)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown("e"))
        {
            Right();
        }
        if (Input.GetKeyDown("q"))
        {
            Left();
        }

        float iax = Input.acceleration.x;
        if (current_accelerometer_x != iax)
        {
            is_touch_input = true;
        }

        if (!is_touch_input)
        {
            turnInput = Input.GetAxis("Horizontal");
        }
        else
        {
            turnInput = Input.acceleration.x * 1.3f;
        }
    }

    void OnFingerTap(LeanFinger finger)
    {
        if (!awoken_complete)
        {
            return;
        }

        Jump();
    }

    void OnEnable()
    {
        // Hook into the events we need
        LeanTouch.OnFingerSwipe += OnFingerSwipe;
        LeanTouch.OnFingerTap += OnFingerTap;
    }

    void OnDisable()
    {
        // Unhook the events
        LeanTouch.OnFingerSwipe -= OnFingerSwipe;
        LeanTouch.OnFingerTap -= OnFingerTap;
    }

    public void OnFingerSwipe(LeanFinger finger)
    {
        if (!awoken_complete)
        {
            return;
        }

        // Store the swipe delta in a temp variable
        var swipe = finger.SwipeScreenDelta;

        if (swipe.x < -Mathf.Abs(swipe.y))
        {
            Left();
        }

        if (swipe.x > Mathf.Abs(swipe.y))
        {
            Right();
        }

        if (swipe.y < -Mathf.Abs(swipe.x))
        {
            //InfoText.text = "You swiped down!";
        }

        if (swipe.y > Mathf.Abs(swipe.x))
        {
            //InfoText.text = "You swiped up!";
        }
    }

    public float GetActualSpeed()
    {
        return actual_speed;
    }

    public float GetSpeedPercentage()
    {
        return speed_multiplier_calculated;
    }
}