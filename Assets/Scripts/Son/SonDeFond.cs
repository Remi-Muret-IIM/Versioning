using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("Liste des musiques disponibles")]
    public AudioClip[] playlist;

    [Header("Source audio principale")]
    public AudioSource audioSource;

    private int lastIndex = -1;

    void Start()
    {
        if (playlist.Length == 0 || audioSource == null)
        {
            Debug.LogWarning("⚠️ Aucun clip ou AudioSource assigné au MusicPlayer !");
            return;
        }

        PlayRandomMusic();
    }

    void Update()
    {
        // Si la musique s'arrête, on lance la suivante
        if (!audioSource.isPlaying)
        {
            PlayRandomMusic();
        }
    }

    void PlayRandomMusic()
    {
        if (playlist.Length == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, playlist.Length);
        }
        while (newIndex == lastIndex && playlist.Length > 1);

        lastIndex = newIndex;
        audioSource.clip = playlist[newIndex];
        audioSource.Play();

        Debug.Log("🎵 Lecture de : " + playlist[newIndex].name);
    }
}
