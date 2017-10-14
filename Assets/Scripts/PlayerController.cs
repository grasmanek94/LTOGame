using UnityEngine;
using System.Collections;

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

    public GameObject below
    {
        get;
        private set;
    }

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
    }

    void MakeFlatToSurface()
    {
        Vector3 euler = transform.localRotation.eulerAngles;

        float add_euler_z = Mathf.Sign(euler.z - 180.0f) * Mathf.Max(Mathf.Sqrt(180.0f - Mathf.Abs(euler.z - 180.0f)) / 5.0f, 15.0f);

        RaycastHit hit;
        
        Vector3 pos_front = transform.TransformPoint(0.0f, 0.0f, 14.0f);
        Vector3 pos_back = transform.TransformPoint(0.0f, 0.0f, -14.0f);

        float distance_front = 9.0f;
        float distance_back = 9.0f;

        bool successes = true;
        if (Physics.Raycast(pos_front, -transform.up, out hit, distance_front))
        {
            distance_front = hit.distance;
        }
        else
        {
            successes = false;
        }

        if (Physics.Raycast(pos_back, -transform.up, out hit, distance_back))
        {
            distance_back = hit.distance;
        }
        else
        {
            successes = false;
        }

        distance_front += 1.0f;
        distance_back += 1.0f;

        float delta = 45.0f - (Mathf.Rad2Deg * Mathf.Atan2(distance_back, distance_front));
        float add_euler_x = Mathf.Sign(delta) * Mathf.Max(Mathf.Abs(delta), 7.0f);

        if(!successes)
        {
            add_euler_x = Mathf.Sign(euler.x - 180.0f) * Mathf.Max(Mathf.Sqrt(180.0f - Mathf.Abs(euler.x - 180.0f)) / 5.0f, 15.0f);
        }

        rigidbody.AddRelativeTorque(add_euler_x, 0f, add_euler_z);
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float delta = hoverHeight - hit.distance;
            float proportionalHeight = Mathf.Pow(hoverHeight - hit.distance, 3.0f) / hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
            
        }
        else if(rigidbody.velocity.magnitude < 0.001)
        {
            Vector3 appliedHoverForce = Vector3.up * hoverForce * jumpForce;
            rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }

        if (Physics.Raycast(ray, out hit))
        {
            below = hit.collider.gameObject.transform.parent.gameObject;
        }

        rigidbody.AddRelativeForce(turnInput * turnSpeed, 0f, powerInput * speed);

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

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    /*public float speed;

    private Rigidbody rigidbody;
    private float height = 5.0f;
    private float forceMultiplier = 20.00f;
    private float lockY = 0.0f;
    private bool rotating = false;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.freezeRotation = true;

        rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }



    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            StartCoroutine(RotateMe(Vector3.up * 90.0f, 0.1f));
        }
        if (Input.GetKeyDown("q"))
        {
            StartCoroutine(RotateMe(Vector3.up * -90.0f, 0.1f));
        }


        //float corr_x = Mathf.Clamp(transform.rotation.eulerAngles.x, -60.0f, 60.0f);
        //float corr_z = Mathf.Clamp(transform.rotation.eulerAngles.z, -30.0f, 30.0f);
        //transform.rotation = Quaternion.Euler(corr_x, transform.rotation.eulerAngles.y, corr_z);

    }

    private void AddFloaterAtOffset(Vector3 offset)
    {
        Vector3 pos = transform.TransformPoint(offset);
        RaycastHit hit;

        float floating_distance = 0.4f;
        float min_floating_distance = 0.01f;
        float distance = floating_distance;

        if (Physics.Raycast(pos, -transform.up, out hit))
        {
            distance = hit.distance;
        }
        else
        {
            distance = min_floating_distance;
        }

        rigidbody.AddForceAtPosition(new Vector3(0.0f, Mathf.Clamp(forceMultiplier * (floating_distance / Mathf.Clamp(distance, min_floating_distance, floating_distance)), forceMultiplier / 10.0f, forceMultiplier * 2.0f), 0.0f), pos);
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        AddFloaterAtOffset(new Vector3(-6.5f, 0.0f, 14.0f));
        AddFloaterAtOffset(new Vector3(6.5f, 0.0f, 14.0f));
        AddFloaterAtOffset(new Vector3(-6.5f, 0.0f, -14.0f));
        AddFloaterAtOffset(new Vector3(6.5f, 0.0f, -14.0f));

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rigidbody.AddRelativeForce(movement * speed);
    }*/

    /*void LateUpdate()
    {
        rigidbody.AddTorque(rigidbody.velocity.magnitude / (speed * speed), 0.0f, 0.0f);
    }*/
}