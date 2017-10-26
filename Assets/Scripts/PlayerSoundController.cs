using UnityEngine;
using System.Collections;

public class PlayerSoundController : MonoBehaviour
{

    public AudioSource jet_sound;
    public AudioSource jump_sound;

    [System.Serializable]
    public class CollisionPair
    {
        public AudioSource[] sounds;
        public float minpower;
        public float maxpower;
    }

    public CollisionPair[] collisions;

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

    public void Jump(float power)
    {
        if (power > 0.33f)
        {
            jump_sound.pitch = 1.33f * power;
            jump_sound.Play();
        }
    }

    public void Impact(float power)
    {
        foreach(CollisionPair pair in collisions)
        {
            if(power >= pair.minpower && power <= pair.maxpower)
            {
                if (pair.sounds.Length > 0)
                {
                    AudioSource randsrc = pair.sounds[Random.Range(0, pair.sounds.Length)];
                    randsrc.Play();
                    break;
                }
            }
        }
    }
}