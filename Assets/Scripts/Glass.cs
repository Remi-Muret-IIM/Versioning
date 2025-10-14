using UnityEngine;

public class Glass2D : MonoBehaviour
{
    public float fillLevel = 0f;
    public float maxFill = 1f;
    public SpriteRenderer fillVisual;

    public void Fill(float amount)
    {
        fillLevel = Mathf.Clamp(fillLevel + amount, 0f, maxFill);
        UpdateVisual();
        if (fillLevel >= maxFill)
        {
            Debug.Log("Verre rempli !");
        }
    }

    void UpdateVisual()
    {
        if (fillVisual != null)
            fillVisual.transform.localScale = new Vector3(1f, fillLevel / maxFill, 1f);
    }
}
