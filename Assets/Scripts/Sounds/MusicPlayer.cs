using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] playlist;

    private int lastIndex = -1;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (playlist.Length == 0 || audioSource == null)
        {
            return;
        }

        PlayRandomMusic();
    }

    void Update()
    {
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
    }
}
