using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 90f;
    public float turnSpeed = 5f;
    private float powerInput;
    private float turnInput;
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
    }

    private void FixedUpdate()
    {
        rigidbody.AddRelativeForce(turnInput * turnSpeed, 0f, powerInput * speed);
    }
}