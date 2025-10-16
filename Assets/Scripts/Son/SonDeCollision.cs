using UnityEngine;

public class DetectCollision2D : MonoBehaviour
{
    // 🎵 Le son à jouer lors de la collision (à choisir dans l'inspecteur)
    public AudioClip collisionSound;

    // 🔊 La source audio utilisée pour jouer le son
    private AudioSource audioSource;

    void Start()
    {
        // Vérifie si un AudioSource existe déjà, sinon en crée un
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false; // Ne pas jouer automatiquement
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"💥 Collision détectée avec : {collision.gameObject.name}");

        // Si un son est assigné, on le joue
        if (collisionSound != null)
        {
            audioSource.clip = collisionSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("⚠️ Aucun son assigné dans l'inspecteur !");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log($"🚶‍♂️ Fin de la collision avec : {collision.gameObject.name}");
    }
}
