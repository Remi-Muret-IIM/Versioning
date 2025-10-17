using UnityEngine;
using System.Collections;

public class GlassExplode : MonoBehaviour
{
    public GameObject explosionEffectPrefab;
    public float resetDelay = 1.5f;
    public GameObject visualObject;
    public float fallAngleThreshold = 60f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isResetting = false;

    public AudioClip ExplosionClip;
    private AudioSource audioSource;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        float xAngle = Mathf.Abs(transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x);
        float zAngle = Mathf.Abs(transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);

        if (!isResetting && (xAngle > fallAngleThreshold || zAngle > fallAngleThreshold))
        {
            StartCoroutine(ExplodeAndReset());
        }
    }

    IEnumerator ExplodeAndReset()
    {
        isResetting = true;

        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            audioSource.clip = ExplosionClip;
            audioSource.Play();

        if (visualObject != null)
            visualObject.SetActive(false);

        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        if (GameManager.Instance != null)
            GameManager.Instance.OnRoundEnd(false);

        yield return new WaitForSeconds(resetDelay);

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (visualObject != null)
            visualObject.SetActive(true);

        foreach (var col in colliders)
            col.enabled = true;

        isResetting = false;
    }
}