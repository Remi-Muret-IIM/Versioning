using UnityEngine;

public class Bottle : MonoBehaviour
{
    public int childIndex = 0;
    public float targetZ = 100f;

    void Update()
    {
        HandleChildActivation();
    }

    void HandleChildActivation()
    {
        float zRotation = transform.eulerAngles.z;
        zRotation = (zRotation + 360f) % 360f;

        if (childIndex >= transform.childCount) return;
        Transform child = transform.GetChild(childIndex);

        if (zRotation >= targetZ)
            child.gameObject.SetActive(true);
        else
            child.gameObject.SetActive(false);
    }
}
