using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 90f;
    public float turnSpeed = 5f;
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    public float jumpForce = 20.0f;
    private float powerInput;
    private float turnInput;
    private Rigidbody rigidbody;

    public int score {
        get;
        private set;
    }

    private int old_score;
    public Text scoreText;

    public GameObject below
    {
        get;
        private set;
    }

    public GameObject hit_debug;

    private float lockY = 0.0f;
    private bool rotating = false;

    IEnumerator RotateMe(Vector3 byAngles, float inTime)
    {
        rotating = true;
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        var velocity = rigidbody.velocity.magnitude;
        lockY += byAngles.y;
        for (var t = 0f; t <= 2.0; t += Time.deltaTime / inTime)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            rigidbody.velocity = transform.forward * velocity;
            yield return null;
        }
        rotating = false;
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        below = null;
        score = 0;
        old_score = 0;
    }

    void Update()
    {
        powerInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown("e"))
        {
            StartCoroutine(RotateMe(Vector3.up * 90.0f, 0.1f));
        }
        if (Input.GetKeyDown("q"))
        {
            StartCoroutine(RotateMe(Vector3.up * -90.0f, 0.1f));
        }

        UpdateScoreText();
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

        if (Physics.Raycast(transform.position, -Vector3.up, out hit
            //, hoverHeight
            ))
        {
            float delta = Mathf.Clamp(hoverHeight - hit.distance, -hoverHeight, hoverHeight);

            float proportionalHeight = Mathf.Sign(delta) * Mathf.Pow(Mathf.Abs(delta), 2.0f) / hoverHeight;

            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);

            below = hit.collider.gameObject.transform.root.gameObject;
            if (hit_debug != null)
            {
                hit_debug.transform.position = hit.point;
                hit_debug.transform.LookAt(rigidbody.transform);
            }
        }
        else if (rigidbody.velocity.magnitude < 0.001)
        {
            Vector3 appliedHoverForce = Vector3.up * hoverForce * jumpForce;
            rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }

        rigidbody.AddRelativeForce(turnInput * turnSpeed, 0f, powerInput * speed);
    }

    void FixedUpdate()
    {
        Hover();
        MakeFlatToSurface();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 appliedHoverForce = Vector3.up * hoverForce * jumpForce;
            rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }

        if (!rotating)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, lockY, transform.localRotation.eulerAngles.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            score += 1;
        }
    }

    void UpdateScoreText()
    {
        if (old_score != score)
        {
            old_score = score;
            scoreText.text = "Score: " + score.ToString();
        }
    }
}