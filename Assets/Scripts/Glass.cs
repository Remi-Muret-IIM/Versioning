using UnityEngine;
using System.Collections.Generic;

public class Glass : MonoBehaviour
{
    public float fillSpeed = 10f;
    public float margin = 10f;

    [Range(0f, 100f)] public float fillGlobal = 0f;
    [Range(0f, 100f)] public float fillBottle1 = 0f;
    [Range(0f, 100f)] public float fillBottle2 = 0f;
    [Range(0f, 100f)] public float fillBottle3 = 0f;
    [Range(0f, 100f)] public float fillBottle4 = 0f;

    private bool resultChecked = false;

    void Update()
    {
        if (fillGlobal >= 100f && !resultChecked)
            CheckGameConditions();
    }

    void OnParticleCollision(GameObject other)
    {
        IncreaseFill(other.tag);
    }

    void IncreaseFill(string tag)
    {
        float delta = fillSpeed * Time.deltaTime;

        switch (tag)
        {
            case "Bottle1": fillBottle1 = Mathf.Clamp(fillBottle1 + delta, 0f, 100f); break;
            case "Bottle2": fillBottle2 = Mathf.Clamp(fillBottle2 + delta, 0f, 100f); break;
            case "Bottle3": fillBottle3 = Mathf.Clamp(fillBottle3 + delta, 0f, 100f); break;
            case "Bottle4": fillBottle4 = Mathf.Clamp(fillBottle4 + delta, 0f, 100f); break;
        }

        fillGlobal = Mathf.Clamp(fillGlobal + delta, 0f, 100f);
    }

    void CheckGameConditions()
    {
        if (GameManager.Instance == null)
            return;

        resultChecked = true;
        bool win = true;

        var targets = GameManager.Instance.targetQuantities;
        List<string> allBottles = new List<string> { "Bottle1", "Bottle2", "Bottle3", "Bottle4" };

        foreach (string bottle in allBottles)
        {
            float current = GetFill(bottle);
            float target = targets.ContainsKey(bottle) ? targets[bottle] : 0f;

            if (Mathf.Abs(current - target) > margin)
                win = false;
        }

        GameManager.Instance.OnRoundEnd(win);
    }

    float GetFill(string tag)
    {
        return tag switch
        {
            "Bottle1" => fillBottle1,
            "Bottle2" => fillBottle2,
            "Bottle3" => fillBottle3,
            "Bottle4" => fillBottle4,
            _ => 0f
        };
    }
}
