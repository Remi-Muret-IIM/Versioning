using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fill_Bar : MonoBehaviour
{
    [Header("References")]
    private Glass glass;
    public GameManager gameManager;

    [Header("UI Elements")]
    public Slider[] fillSliders;
    public Slider[] targetSliders;
    public TextMeshProUGUI[] percentageTexts;
    public Color targetColor = new Color(1f, 1f, 1f, 0.5f);

    private string[] bottleKeys = new string[] { "Liquid1", "Liquid2", "Liquid3", "Liquid4" };

    private void Start()
    {
        if (gameManager != null)
            gameManager.OnGlassSpawned += OnNewGlass;

        int n = bottleKeys.Length;

        for (int i = 0; i < n; i++)
        {
            if (fillSliders != null && i < fillSliders.Length && fillSliders[i] != null)
            {
                fillSliders[i].minValue = 0f;
                fillSliders[i].maxValue = 100f;
                fillSliders[i].interactable = false;
                if (fillSliders[i].handleRect != null)
                    fillSliders[i].handleRect.gameObject.SetActive(false);
            }

            if (targetSliders != null && i < targetSliders.Length && targetSliders[i] != null)
            {
                targetSliders[i].minValue = 0f;
                targetSliders[i].maxValue = 100f;
                targetSliders[i].interactable = false;
                if (targetSliders[i].handleRect != null)
                    targetSliders[i].handleRect.gameObject.SetActive(false);

                if (targetSliders[i].fillRect != null)
                {
                    var img = targetSliders[i].fillRect.GetComponent<Image>();
                    if (img != null) img.color = targetColor;
                }

                if (fillSliders != null && i < fillSliders.Length && fillSliders[i] != null)
                {
                    int fillIndex = fillSliders[i].transform.GetSiblingIndex();
                    targetSliders[i].transform.SetSiblingIndex(Mathf.Max(0, fillIndex - 1));
                }
            }

            string key = (i < bottleKeys.Length) ? bottleKeys[i] : "N/A";
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnGlassSpawned -= OnNewGlass;
    }

    private void OnNewGlass(Glass newGlass)
    {
        glass = newGlass;
    }

    private void Update()
    {
        if (glass != null)
            UpdateFillBars();
    }

    private void UpdateFillBars()
    {
        int n = bottleKeys.Length;
        for (int i = 0; i < n; i++)
        {
            string key = bottleKeys[i];
            float currentFill = Mathf.Clamp(GetBottleFill(key), 0f, 100f);

            if (fillSliders != null && i < fillSliders.Length && fillSliders[i] != null)
                fillSliders[i].value = currentFill;

            float targetFill = 0f;
            if (gameManager != null && gameManager.targetQuantities != null && gameManager.targetQuantities.ContainsKey(key))
                targetFill = gameManager.targetQuantities[key];

            if (targetSliders != null && i < targetSliders.Length && targetSliders[i] != null)
                targetSliders[i].value = Mathf.Clamp(targetFill, 0f, 100f);

            if (percentageTexts != null && i < percentageTexts.Length && percentageTexts[i] != null)
                percentageTexts[i].text = $"{currentFill:F0}% / {targetFill:F0}%";
        }
    }

    private float GetBottleFill(string bottleKey)
    {
        return bottleKey switch
        {
            "Liquid1" => glass.fillLiquid1,
            "Liquid2" => glass.fillLiquid2,
            "Liquid3" => glass.fillLiquid3,
            "Liquid4" => glass.fillLiquid4,
            _ => 0f
        };
    }
}