using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadingRotator : MonoBehaviour {

    private Rigidbody rigidbody;
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

    // Use this for initialization
    void Awake ()
    {
        rigidbody = GetComponent<Rigidbody>();
        lockY = Mathf.Round(transform.eulerAngles.y / 90.0f) * 90.0f;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!rotating)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, lockY, transform.localRotation.eulerAngles.z);
        }
    }

    public void Left()
    {
        StartCoroutine(RotateMe(Vector3.up * -90.0f, 0.1f));
    }

    public void Right()
    {
        StartCoroutine(RotateMe(Vector3.up * 90.0f, 0.1f));
    }

    public void SetForceDirection(float direction)
    {
        lockY = direction;
    }
}
