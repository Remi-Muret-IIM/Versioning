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

    private readonly string[] bottleNames = { "Bottle1", "Bottle2", "Bottle3", "Bottle4" };

    private void Start()
    {
        gameManager.OnGlassSpawned += OnNewGlass;

        // Vérifications / configuration des sliders pour éviter les erreurs d'assignation
        int n = bottleNames.Length;

        if (fillSliders == null || targetSliders == null || percentageTexts == null)
            Debug.LogWarning("Fill_Bar: un ou plusieurs tableaux UI ne sont pas assignés dans l'inspector.");

        for (int i = 0; i < n; i++)
        {
            if (fillSliders != null && i < fillSliders.Length && fillSliders[i] != null)
            {
                fillSliders[i].minValue = 0f;
                fillSliders[i].maxValue = 100f;
                fillSliders[i].interactable = false;
                // masquer le handle si présent
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

                // couleur de la barre d'objectif
                if (targetSliders[i].fillRect != null)
                {
                    var img = targetSliders[i].fillRect.GetComponent<Image>();
                    if (img != null) img.color = targetColor;
                }

                // s'assurer que la target est derrière la fill (si les deux sont enfants du même container)
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
        }

        // Vérifier s'il y a des références dupliquées (erreur courante dans l'inspector)
        if (fillSliders != null && targetSliders != null)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i != j && fillSliders.Length > i && fillSliders.Length > j && fillSliders[i] == fillSliders[j])
                        Debug.LogWarning($"Fill_Bar: fillSliders[{i}] et fillSliders[{j}] pointent vers le même Slider (erreur d'assignation).");
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
        {
            UpdateFillBars();
        }
    }

    private void UpdateFillBars()
    {
        for (int i = 0; i < bottleNames.Length; i++)
        {
            float currentFill = Mathf.Clamp(GetBottleFill(bottleNames[i]), 0f, 100f);

            if (fillSliders != null && i < fillSliders.Length && fillSliders[i] != null)
                fillSliders[i].value = currentFill;

            float targetFill = 0f;
            if (gameManager != null && gameManager.targetQuantities != null && gameManager.targetQuantities.ContainsKey(bottleNames[i]))
                targetFill = gameManager.targetQuantities[bottleNames[i]];

            if (targetSliders != null && i < targetSliders.Length && targetSliders[i] != null)
                targetSliders[i].value = Mathf.Clamp(targetFill, 0f, 100f);

            if (percentageTexts != null && i < percentageTexts.Length && percentageTexts[i] != null)
                percentageTexts[i].text = $"{currentFill:F0}% / {targetFill:F0}%";
        }
    }

    private float GetBottleFill(string bottleName)
    {
        return bottleName switch
        {
            "Bottle1" => glass.fillBottle1,
            "Bottle2" => glass.fillBottle2,
            "Bottle3" => glass.fillBottle3,
            "Bottle4" => glass.fillBottle4,
            _ => 0f
        };
    }
}