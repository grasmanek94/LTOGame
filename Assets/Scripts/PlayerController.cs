using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 90f;
    public float turnSpeed = 5f;
    public float seconds_stuck_lose_life = 1.0f;
    public int lives = 3;
    private float powerInput;
    private float turnInput;

    public Rechargeable jump_recharge;
    private Rigidbody rigidbody;
    private HoverEngine hover_engine;
    private HeadingRotator heading_rotator;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        hover_engine = GetComponent<HoverEngine>();
        heading_rotator = GetComponent<HeadingRotator>();

        powerInput = 1.0f;
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

        if(hover_engine.seconds_stuck >= seconds_stuck_lose_life)
        {
            // lose life

        }
    }

    private void FixedUpdate()
    {
        rigidbody.AddRelativeForce(turnInput * turnSpeed, 0f, powerInput * speed);
    }

}