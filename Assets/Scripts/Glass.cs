using UnityEngine;
using System.Collections;

public class Glass : MonoBehaviour
{
    [Range(0f, 100f)]
    public float fillPercent = 0f;
    public float fillSpeed = 10f;

    private bool isFilling = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public float fallAngleThreshold = 60f;

    public GameObject explosionEffectPrefab;
    public float resetDelay = 1.5f;
    public GameObject visualObject;

    private bool isResetting = false;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (isFilling)
        {
            fillPercent += fillSpeed * Time.deltaTime;
            fillPercent = Mathf.Clamp(fillPercent, 0f, 100f);
        }
        float xAngle = Mathf.Abs(transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x);
        float zAngle = Mathf.Abs(transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);

        if (!isResetting && (xAngle > fallAngleThreshold || zAngle > fallAngleThreshold))
        {
            StartCoroutine(ExplodeAndReset());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bottle1"))
        {
            isFilling = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bottle1"))
        {
            isFilling = false;
        }
    }

    IEnumerator ExplodeAndReset()
    {
        isResetting = true;
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        if (visualObject != null) visualObject.SetActive(false);

        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        yield return new WaitForSeconds(resetDelay);
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        fillPercent = 0f;
        isFilling = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (visualObject != null) visualObject.SetActive(true);
        foreach (var col in colliders)
            col.enabled = true;

        isResetting = false;
    }
}
