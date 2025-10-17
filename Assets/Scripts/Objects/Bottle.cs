using UnityEngine;

public class Bottle : MonoBehaviour
{
    [Header("Pour settings")]
    public float spawnOffset = 1.5f;
    public float targetZ = 90f;
    public float pourRate = 0.05f;

    [Header("drop settings")]
    public GameObject dropPrefab;
    public string dropTag;
    public float dropScale = 0.3f;
    public float dropForce = 0.1f;

    [Header("Colors")]
    public Color liquid1Color = Color.red;
    public Color liquid2Color = Color.green;
    public Color liquid3Color = Color.blue;
    public Color liquid4Color = Color.yellow;

    private float pourTimer;

    void Update()
    {
        HandlePouring();
    }

    void HandlePouring()
    {
        float zRotation = transform.eulerAngles.z;
        if (zRotation > 180f)
            zRotation -= 360f;

        bool isPouring = (zRotation >= targetZ || zRotation <= -targetZ);

        if (isPouring)
        {
            pourTimer += Time.deltaTime;
            if (pourTimer >= pourRate)
            {
                SpawnDrop();
                pourTimer = 0f;
            }
        }
        else
        {
            pourTimer = 0f;
        }
    }

    void SpawnDrop()
    {
        if (!dropPrefab) return;

        Vector3 spawnPos = transform.position + transform.up * spawnOffset;
        GameObject drop = Instantiate(dropPrefab, spawnPos, Quaternion.identity);
        drop.tag = dropTag;
        drop.transform.localScale = Vector3.one * dropScale;

        var sr = drop.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = GetColorForTag(dropTag);

        Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = transform.up;
            float randomForce = dropForce * Random.Range(1f, 1.2f);
            rb.AddForce(dir * randomForce, ForceMode2D.Impulse);
        }
    }

    Color GetColorForTag(string tag)
    {
        return tag switch
        {
            "Liquid1" => liquid1Color,
            "Liquid2" => liquid2Color,
            "Liquid3" => liquid3Color,
            "Liquid4" => liquid4Color,
            _ => Color.white,
        };
    }
}
