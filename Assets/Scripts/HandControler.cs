using UnityEngine;

public class HandControllerPouring2D : MonoBehaviour
{
    [Header("Hand Settings")]
    public float moveSpeed = 10f;
    [Range(0f, 1f)] public float chaosLevel = 0f;
    public float maxChaos = 1f;

    [Header("Bottle Handling")]
    public Transform heldBottle;
    public Transform pourVisualPrefab;  
    public Transform pourPoint;         
    public float pourThreshold = 30f;   
    public float pourRate = 0.5f;       

    private Transform currentPourVisual;

    void Update()
    {
        HandleMovement();
        HandleClick();
        HandlePouringVisual();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePos);

        // Rend la main bourré
        Vector3 chaosOffset = Vector3.zero;
        if (chaosLevel > 0f)
        {
            float time = Time.time * (2f + chaosLevel * 5f);
            chaosOffset = new Vector3(
                (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f,
                (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f,
                0f
            ) * chaosLevel * 3f;

            float shake = Mathf.Sin(Time.time * 50f * chaosLevel) * 0.05f * chaosLevel;
            transform.rotation = Quaternion.Euler(0, 0, shake * 100f);
        }

        float lagFactor = Mathf.Lerp(1f, 0.2f, chaosLevel);
        transform.position = Vector3.Lerp(transform.position, targetPosition + chaosOffset, Time.deltaTime * moveSpeed * lagFactor);
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldBottle == null)
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

                if (hit != null && (
                        hit.CompareTag("Bottle1") ||
                        hit.CompareTag("Bottle2") ||
                        hit.CompareTag("Bottle3") ||
                        hit.CompareTag("Bottle4")))
                {
                    heldBottle = hit.transform;
                    heldBottle.SetParent(transform);
                    heldBottle.localPosition = new Vector3(0, -0.5f, 0);
                    heldBottle.localRotation = Quaternion.identity;

                    Rigidbody2D rb = heldBottle.GetComponent<Rigidbody2D>();
                    if (rb != null)
                        rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
            else
            {
                Rigidbody2D rb = heldBottle.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.bodyType = RigidbodyType2D.Dynamic; 

                heldBottle.SetParent(null);
                heldBottle = null;

                if (currentPourVisual != null)
                {
                    Destroy(currentPourVisual.gameObject);
                    currentPourVisual = null;
                }
            }
        }
    }

    void HandleRotation()
    {
        if (heldBottle != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            { 
                float angle = heldBottle.localEulerAngles.z;
                angle = (angle > 180f) ? angle - 360f : angle;
                angle -= scroll * 50f;
                angle = Mathf.Clamp(angle, -90f, 90f);
                heldBottle.localEulerAngles = new Vector3(0, 0, angle);
            }
        }
    }

    void HandlePouringVisual()
    {
        if (heldBottle == null || pourPoint == null || pourVisualPrefab == null) return;

        float zAngle = heldBottle.localEulerAngles.z;
        zAngle = (zAngle > 180) ? zAngle - 360 : zAngle;

        if (Mathf.Abs(zAngle) >= pourThreshold)
        {
            if (currentPourVisual == null)
            {
                currentPourVisual = Instantiate(pourVisualPrefab, pourPoint.position, Quaternion.identity);
            }

            currentPourVisual.position = pourPoint.position;

            RaycastHit2D hit = Physics2D.Raycast(pourPoint.position, Vector2.down, 5f, LayerMask.GetMask("Glass"));
            if (hit.collider != null)
            {
                Glass2D glass = hit.collider.GetComponent<Glass2D>();
                if (glass != null)
                    glass.Fill(pourRate * Time.deltaTime);
            }
        }
        else
        {
            if (currentPourVisual != null)
            {
                Destroy(currentPourVisual.gameObject);
                currentPourVisual = null;
            }
        }
    }

    public void IncreaseChaos(float amount)
    {
        chaosLevel = Mathf.Clamp(chaosLevel + amount, 0f, maxChaos);
    }
}
