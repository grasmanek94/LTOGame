using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointAble : MonoBehaviour {

    private GameObject last_know_checkpoint;
    private HoverEngine hover_engine;
    private HeadingRotator heading_rotator;
    private Rigidbody rigidbody;

    void Awake()
    {
        hover_engine = GetComponent<HoverEngine>();
        heading_rotator = GetComponent<HeadingRotator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(hover_engine.below != last_know_checkpoint)
        {
            ConnectionOffsets current = hover_engine.below.GetComponent<ConnectionOffsets>();
            if (current != null)
            {
                last_know_checkpoint = hover_engine.below;
            }
        }
    }

    public bool Reset()
    {
        if(last_know_checkpoint == null)
        {
            return false;
        }

        ConnectionOffsets offsets = last_know_checkpoint.GetComponent<ConnectionOffsets>();

        if (offsets.position_offsets.Length > 0)
        {
            Vector3 pos = last_know_checkpoint.transform.TransformPoint(offsets.position_offsets[0]);
            pos.y += hover_engine.hoverHeight;
            transform.position = pos;

            Vector3 rot = offsets.rotation_offsets[0];
            rot.y += last_know_checkpoint.transform.eulerAngles.y;

            transform.rotation = Quaternion.Euler(rot);
            heading_rotator.SetForceDirection(rot.y);
            transform.position += transform.forward * 2.5f;

            rigidbody.velocity = Vector3.zero;
        }

        return true;
    }
}
