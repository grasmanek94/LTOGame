using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEngine : MonoBehaviour
{

    public float hoverForce = 40.0f;
    public float hoverHeight = 2.33f;
    public float jumpForce = 10.0f;
    public float seconds_stuck = 0.0f;

    private Rigidbody rigidbody;

    private float seconds_stuck_at;
    private bool is_stuck;
    
    public GameObject below
    {
        get;
        private set;
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        below = null;

        seconds_stuck_at = 0.0f;
        is_stuck = false;
    }

    public void Jump()
    {
        Vector3 appliedHoverForce = Vector3.up * hoverForce * jumpForce;
        rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
    }

    float CalculateAngleDifference(Vector3 pos_a, Vector3 pos_b, float angle)
    {
        RaycastHit hit;
        bool successes = true;
        float add_euler = 0.0f;
        float delta = 0.0f;
        float distance_a = hoverHeight * 4.0f;
        float distance_b = hoverHeight * 4.0f;

        if (Physics.Raycast(pos_a, -Vector3.up, out hit, distance_a))
        {
            distance_a = hit.distance;
        }
        else
        {
            successes = false;
        }

        if (Physics.Raycast(pos_b, -Vector3.up, out hit, distance_b))
        {
            distance_b = hit.distance;
        }
        else
        {
            successes = false;
        }

        distance_a += 1.0f;
        distance_b += 1.0f;

        delta = 45.0f - (Mathf.Rad2Deg * Mathf.Atan2(distance_b, distance_a));
        add_euler = Mathf.Sign(delta) * Mathf.Max(Mathf.Abs(delta), 7.0f);

        if (!successes)
        {
            add_euler = Mathf.Sign(angle - 180.0f) * Mathf.Max(Mathf.Sqrt(180.0f - Mathf.Abs(angle - 180.0f)) / 5.0f, 15.0f);
        }

        return add_euler;
    }

    void MakeFlatToSurface()
    {
        Vector3 euler = transform.localRotation.eulerAngles;

        Vector3 pos_front = transform.TransformPoint(0.0f, 0.0f, 14.0f);
        Vector3 pos_back = transform.TransformPoint(0.0f, 0.0f, -14.0f);
        Vector3 pos_left = transform.TransformPoint(-1.25f, 0.0f, 0.0f);
        Vector3 pos_right = transform.TransformPoint(1.25f, 0.0f, 0.0f);

        // apply calculated torque
        rigidbody.AddRelativeTorque(
            3.0f * CalculateAngleDifference(pos_front, pos_back, euler.x),
            0f,
            3.0f * CalculateAngleDifference(pos_left, pos_right, euler.z)
        );
    }

    void Hover()
    {
        RaycastHit hit;

        float delta = -0.33f;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            delta = Mathf.Clamp(hoverHeight - hit.distance, -hoverHeight, hoverHeight);
            below = hit.collider.gameObject.transform.root.gameObject;
            if (is_stuck)
            {
                is_stuck = false;
                seconds_stuck = 0.0f;
            }
        }
        else
        {
            if(!is_stuck)
            {
                is_stuck = true;
                seconds_stuck_at = Time.time;
            }
            seconds_stuck = Time.time - seconds_stuck_at;
            Debug.Log(seconds_stuck);
        }

        Vector3 appliedHoverForce = Vector3.up * delta * hoverForce;
        rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
    }

    void FixedUpdate()
    {
        Hover();
        MakeFlatToSurface();
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 appliedHoverForce = Vector3.up * hoverForce * jumpForce / 2.0f;
        rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
    }
}