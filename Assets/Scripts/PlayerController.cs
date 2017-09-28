using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;

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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddRelativeForce(0.0f, forceMultiplier * 100.0f, 0.0f);
        }

        if (!rotating)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, lockY, transform.localRotation.eulerAngles.z);
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
    }

    /*void LateUpdate()
    {
        rigidbody.AddTorque(rigidbody.velocity.magnitude / (speed * speed), 0.0f, 0.0f);
    }*/
}