using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Lean.Touch;
using System;

public class PlayerController : MonoBehaviour
{
    public float speed = 300f;
    public float speed_limit = 750.0f;
    public float turnSpeed = 500f;
    public float speed_increase_per_minute = 50.0f;
    
    [SerializeField]
    private float actual_speed;

    private float powerInput;
    private float turnInput;
    private float speed_increase_per_delta_t;

    public Rechargeable jump_recharge;
    public Image jump_charge_ui;
    private ProgressBar.ProgressRadialBehaviour jump_charge;

    private Rigidbody rigidbody;
    private HoverEngine hover_engine;
    private HeadingRotator heading_rotator;
    private Scorer scorer;
    private CheckpointAble checkpointable;
    private PlayerSoundController sound;

    private float speed_multiplier_calculated;
    private float awoken_time;
    private bool awoken_complete;

    private bool is_touch_input;
    private float current_accelerometer_x;
    private float speed_mult_sqrt;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        hover_engine = GetComponent<HoverEngine>();
        heading_rotator = GetComponent<HeadingRotator>();
        scorer = GetComponent<Scorer>();
        checkpointable = GetComponent<CheckpointAble>();
        sound = GetComponent<PlayerSoundController>();

        jump_charge = jump_charge_ui.GetComponent<ProgressBar.ProgressRadialBehaviour>();

        powerInput = 0.0f;
        actual_speed = speed;
        speed_increase_per_delta_t = speed_increase_per_minute / 60.0f;
        awoken_time = Time.time;
        awoken_complete = false;
        is_touch_input = false;
        current_accelerometer_x = Input.acceleration.x;
    }

    public void ResetLife()
    {
        awoken_complete = false;
        awoken_time = Time.time;
        powerInput = 0.0f;
        checkpointable.Reset();
        actual_speed = speed;
    }

    void Jump()
    {
        hover_engine.jumpMultiplier = jump_recharge.UseCharge();
        hover_engine.Jump();
        sound.Jump(hover_engine.jumpMultiplier);
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
        speed_mult_sqrt = Mathf.Sqrt(speed_multiplier_calculated);
        hover_engine.hoverMultiplier = speed_mult_sqrt;
        scorer.multiplier = speed_mult_sqrt;

        jump_charge.Value = jump_recharge.percentage * 100.0f;

    }

    public float GetSpeedMultiplierSqrt()
    {
        return speed_mult_sqrt;
    }

    void FixedUpdate()
    {
        rigidbody.AddRelativeForce(turnInput * turnSpeed * speed_multiplier_calculated, 0f, powerInput * actual_speed);
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

    void OnCollisionEnter(Collision collision)
    {
        if (!awoken_complete)
        {
            return;
        }

        float mag = collision.impulse.magnitude;
        sound.Impact(mag);
        actual_speed -= Mathf.Sqrt(mag);
    }
}