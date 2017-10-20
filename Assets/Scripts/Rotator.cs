using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
    public float x = 15.0f;
    public float y = 30.0f;
    public float z = 45.0f;

    void Update()
    {
        transform.Rotate(new Vector3(x, y, z) * Time.deltaTime);
    }
}