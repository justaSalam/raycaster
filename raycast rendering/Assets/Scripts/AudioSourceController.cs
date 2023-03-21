using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    public static AudioSourceController Instance;
    AudioSource audioSource;
    public AudioClip[] audioClips;
    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayAudio(string name)
    {
        for (int i = 0; i < audioClips.Length; i++)
        {
            if(audioClips[i].name == name)
            {
                audioSource.clip = audioClips[i];
                audioSource.Play();
            }
        }
    }
}

