using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundEffect : MonoBehaviour
{
    public AudioClip collisionSound;    // Assign the sound clip in the inspector
    public float minVelocity = 2.0f;    // Minimum velocity to play sound
    public float maxVolumeVelocity = 10.0f;    // Velocity at which sound is played at max volume
    public float minPitch = 0.8f;       // Minimum pitch variation
    public float maxPitch = 1.2f;       // Maximum pitch variation

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = collisionSound;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<CollisionSoundEffect>() != null)
        {
            float collisionVelocity = collision.relativeVelocity.magnitude;
            
            Debug.Log("Hit! " + collisionVelocity);

            if(collisionVelocity > minVelocity)
            {
                // Scale volume between 0 (minVelocity) and 1 (maxVolumeVelocity)
                float volume = Mathf.Clamp01((collisionVelocity - minVelocity) / (maxVolumeVelocity - minVelocity));

                // Randomize pitch
                float pitch = Random.Range(minPitch, maxPitch);

                audioSource.volume = volume;
                audioSource.pitch = pitch;
                audioSource.Play();
            }
        }
    }
}