using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fill_Bar : MonoBehaviour
{
    [Header("References")]
    private Glass glass;
    public GameManager gameManager;

    [Header("UI Elements")]
    public Slider[] fillSliders;      // Remplace fillBars
    public Slider[] targetSliders;    // Remplace targetBars
    public TextMeshProUGUI[] percentageTexts;
    public Color targetColor = new Color(1f, 1f, 1f, 0.5f);

    [Header("Bottle keys (must match bottle.tag / GameManager keys)")]
    public string[] bottleKeys = new string[] { "Liquid1", "Liquid2", "Liquid3", "Liquid4" };

    private void Start()
    {
        if (gameManager != null)
            gameManager.OnGlassSpawned += OnNewGlass;
        else
            Debug.LogWarning("Fill_Bar: GameManager non assigné.");

        int n = bottleKeys.Length;

        if (fillSliders == null || targetSliders == null || percentageTexts == null)
            Debug.LogWarning("Fill_Bar: un ou plusieurs tableaux UI ne sont pas assignés dans l'inspector.");

        // configuration des sliders et vérifications
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
            else
            {
                Debug.LogWarning($"Fill_Bar: fillSliders[{i}] non assigné.");
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

                // essaye de mettre la target derrière la fill si possible
                if (fillSliders != null && i < fillSliders.Length && fillSliders[i] != null)
                {
                    int fillIndex = fillSliders[i].transform.GetSiblingIndex();
                    targetSliders[i].transform.SetSiblingIndex(Mathf.Max(0, fillIndex - 1));
                }
            }
            else
            {
                Debug.LogWarning($"Fill_Bar: targetSliders[{i}] non assigné.");
            }

            if (percentageTexts == null || i >= percentageTexts.Length || percentageTexts[i] == null)
                Debug.LogWarning($"Fill_Bar: percentageTexts[{i}] non assigné.");

            // log mapping slider <-> tag pour débug d'assignation
            string key = (i < bottleKeys.Length) ? bottleKeys[i] : "N/A";
            Debug.Log($"Fill_Bar mapping index {i} -> key '{key}'");
        }

        // vérifier doublons d'assignation de slider
        if (fillSliders != null)
        {
            for (int i = 0; i < fillSliders.Length; i++)
                for (int j = i + 1; j < fillSliders.Length; j++)
                    if (fillSliders[i] != null && fillSliders[j] != null && fillSliders[i] == fillSliders[j])
                        Debug.LogWarning($"Fill_Bar: fillSliders[{i}] et fillSliders[{j}] pointent vers le même Slider.");
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