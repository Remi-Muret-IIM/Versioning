using UnityEngine;

public class RotationTrigger : MonoBehaviour
{
    public int childIndex = 0;
    public float targetZ = 105f;
    public float tolerance = 1f;

    void Update()
    {
        HandleChildActivation();
    }

    void HandleChildActivation()
    {
        float zRotation = transform.eulerAngles.z;
        zRotation = (zRotation + 360f) % 360f;
        Transform child = transform.GetChild(childIndex);

        if (Mathf.Abs(zRotation - targetZ) <= tolerance)
        {
            if (childIndex < transform.childCount)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            child.gameObject.SetActive(false);
        }
    }
}
