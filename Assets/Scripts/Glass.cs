using UnityEngine;

public class Glass : MonoBehaviour
{
    public float fillSpeed = 10f;
    [Range(0f, 100f)] public float fillGlobal = 0f;
    [Range(0f, 100f)] public float fillBottle1 = 0f;
    [Range(0f, 100f)] public float fillBottle2 = 0f;
    [Range(0f, 100f)] public float fillBottle3 = 0f;
    [Range(0f, 100f)] public float fillBottle4 = 0f;

    private string activeBottleTag = null;

    void Update()
    {
        if (activeBottleTag != null && fillGlobal < 100)
        {
            IncreaseFill(activeBottleTag);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsBottleTag(other.tag))
        {
            activeBottleTag = other.tag;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsBottleTag(other.tag) && other.tag == activeBottleTag)
        {
            activeBottleTag = null;
        }
    }

    void IncreaseFill(string tag)
    {
        float delta = fillSpeed * Time.deltaTime;

        switch (tag)
        {
            case "Bottle1":
                fillBottle1 = Mathf.Clamp(fillBottle1 + delta, 0f, 100f);
                break;
            case "Bottle2":
                fillBottle2 = Mathf.Clamp(fillBottle2 + delta, 0f, 100f);
                break;
            case "Bottle3":
                fillBottle3 = Mathf.Clamp(fillBottle3 + delta, 0f, 100f);
                break;
            case "Bottle4":
                fillBottle4 = Mathf.Clamp(fillBottle4 + delta, 0f, 100f);
                break;
        }

        fillGlobal = Mathf.Clamp(fillGlobal + delta, 0f, 100f);
    }

    bool IsBottleTag(string tag)
    {
        return tag == "Bottle1" || tag == "Bottle2" || tag == "Bottle3" || tag == "Bottle4";
    }
}
