using UnityEngine;
using System.Collections.Generic;

public class Glass : MonoBehaviour
{
    public float fillSpeed = 10f;
    public float margin = 10f;

    [Range(0f, 100f)] public float fillGlobal = 0f;
    [Range(0f, 100f)] public float fillLiquid1 = 0f;
    [Range(0f, 100f)] public float fillLiquid2 = 0f;
    [Range(0f, 100f)] public float fillLiquid3 = 0f;
    [Range(0f, 100f)] public float fillLiquid4 = 0f;

    private bool resultChecked = false;

    void Update()
    {
        if (fillGlobal >= 100f && !resultChecked)
            CheckGameConditions();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IncreaseFill(other.tag);
    }

    void IncreaseFill(string tag)
    {
        float delta = fillSpeed * Time.deltaTime;

        switch (tag)
        {
            case "Liquid1": fillLiquid1 = Mathf.Clamp(fillLiquid1 + delta, 0f, 100f); break;
            case "Liquid2": fillLiquid2 = Mathf.Clamp(fillLiquid2 + delta, 0f, 100f); break;
            case "Liquid3": fillLiquid3 = Mathf.Clamp(fillLiquid3 + delta, 0f, 100f); break;
            case "Liquid4": fillLiquid4 = Mathf.Clamp(fillLiquid4 + delta, 0f, 100f); break;
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
        List<string> allBottles = new List<string> { "Liquid1", "Liquid2", "Liquid3", "Liquid4" };

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
            "Liquid1" => fillLiquid1,
            "Liquid2" => fillLiquid2,
            "Liquid3" => fillLiquid3,
            "Liquid4" => fillLiquid4,
            _ => 0f
        };
    }
}
