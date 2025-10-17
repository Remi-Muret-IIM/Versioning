using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingIncrement : MonoBehaviour
{
    [Header("Référence du Volume (ton prefab 'Bloom')")]
    public Volume bloomVolume;

    // Références internes aux overrides
    private Bloom bloom;
    private ChromaticAberration chromatic;
    private Vignette vignette;
    private FilmGrain grain;
    private LensDistortion distortion;

    [Header("Limites associées")]
    public float limitIntensity = 3f;
    public float limitChromatic = 1f;
    public float limitVignette = 0.4f;
    public float limitGrain = 1f;
    public float limitDistortion = 0.3f;

    [Header("Pas d’incrémentation")]
    public float stepIntensity = 0.15f;
    public float stepChromatic = 0.05f;
    public float stepVignette = 0.02f;
    public float stepGrain = 0.05f;
    public float stepDistortion = 0.015f;

    void Start()
    {
        if (bloomVolume == null)
        {
            Debug.LogError("Aucun Volume assigné !");
            return;
        }

        // Récupère les effets depuis le Volume Profile du prefab
        bloomVolume.profile.TryGet(out bloom);
        bloomVolume.profile.TryGet(out chromatic);
        bloomVolume.profile.TryGet(out vignette);
        bloomVolume.profile.TryGet(out grain);
        bloomVolume.profile.TryGet(out distortion);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Touche J détectée !");
            IncreaseEffects();
        }
    }

    void IncreaseEffects()
    {
        Debug.Log($"Bloom intensity = {bloom?.intensity.value}");

        if (bloom != null)
            bloom.intensity.value = Mathf.Min(bloom.intensity.value + stepIntensity, limitIntensity);

        if (chromatic != null)
            chromatic.intensity.value = Mathf.Min(chromatic.intensity.value + stepChromatic, limitChromatic);

        if (vignette != null)
            vignette.intensity.value = Mathf.Min(vignette.intensity.value + stepVignette, limitVignette);

        if (grain != null)
            grain.intensity.value = Mathf.Min(grain.intensity.value + stepGrain, limitGrain);

        if (distortion != null)
            distortion.intensity.value = Mathf.Min(distortion.intensity.value + stepDistortion, limitDistortion);
    }
}
