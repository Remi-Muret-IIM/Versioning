using UnityEngine;

public class Glass : MonoBehaviour
{
    [Range(0f, 100f)]
    public float fillPercent = 0f;
    public float fillSpeed = 10f;

    private bool isFilling = false;

    void Update()
    {
        if (isFilling)
        {
            fillPercent += fillSpeed * Time.deltaTime;
            fillPercent = Mathf.Clamp(fillPercent, 0f, 100f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bottle1"))
        {
            isFilling = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bottle1"))
        {
            isFilling = false;
        }
    }
}
