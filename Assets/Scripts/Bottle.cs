using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
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

        if (zRotation >= targetZ || zRotation <= -targetZ)
            emission.rateOverTime = emissionRate;
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
