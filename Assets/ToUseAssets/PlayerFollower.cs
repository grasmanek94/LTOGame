using UnityEngine;
using System.Collections;

public class PlayerFollower : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    void Start()
    {
        offset = (transform.position - player.transform.position) / (player.transform.localScale.magnitude / 1.75f);
    }

    void LateUpdate()
    {
        transform.position = player.transform.TransformPoint(offset);
        transform.LookAt(player.transform);
    }
}