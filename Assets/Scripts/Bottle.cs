using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Bottle : MonoBehaviour
{
    public AudioClip CoulSound;
    private AudioSource audioSource;

    public float targetZ = 100f;
    public float emissionRate = 50f;

    private GameObject child;
    private new ParticleSystem particleSystem;
    private ParticleSystem.EmissionModule emission;
    private Glass glass;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        particleSystem = child.GetComponent<ParticleSystem>();
        emission = particleSystem.emission;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        HandleEmissionRate();
    }

    void HandleEmissionRate()
    {
        float zRotation = transform.eulerAngles.z;

        if (zRotation > 180f)
            zRotation -= 360f;

        if (zRotation >= targetZ || zRotation <= -targetZ) { 
            emission.rateOverTime = emissionRate;
            Debug.Log("glouglou");}
        else
            emission.rateOverTime = 0f;
    }

    public void RegisterGlass(Glass g)
    {
        glass = g;
        var trigger = particleSystem.trigger;
        trigger.enabled = true;
        trigger.SetCollider(0, glass.GetComponent<Collider2D>());
    }
}
