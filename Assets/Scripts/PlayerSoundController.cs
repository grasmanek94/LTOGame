using UnityEngine;
using System.Collections;

public class PlayerSoundController : MonoBehaviour
{

    public AudioSource jet_sound;
    private const float LowPitch = .1f;
    private const float HighPitch = 2.0f;
    private const float SpeedToRevs = .01f;
    private Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float forward_speed = transform.InverseTransformDirection(rigidbody.velocity).z;
        float engine_revs = Mathf.Abs(forward_speed) * SpeedToRevs;
        jet_sound.pitch = Mathf.Clamp(engine_revs, LowPitch, HighPitch);
    }

}