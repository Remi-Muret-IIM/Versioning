using UnityEngine;

public class CollisionSound: MonoBehaviour
{
    public AudioClip collisionSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionSound != null)
        {
            audioSource.clip = collisionSound;
            audioSource.Play();
        }
    }
}
