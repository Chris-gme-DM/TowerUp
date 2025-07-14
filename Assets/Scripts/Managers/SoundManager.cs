using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance {get; private set;}

    [Header("AudioSources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxPrefabSource;
    [SerializeField] private List<AudioSource> sfxSources;

    [Header("AudioClips")]
    [SerializeField] private AudioClip[] backgroundMusic;
    [SerializeField] private AudioClip[] sfxSounds;
    [SerializeField] private AudioClip[] uiSounds;

    private Queue<AudioSource> sfxSourcePool = new Queue<AudioSource>();
    private int initialPoolSize = 20; // don#t know yet how many i may need

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            InitializeSFXPool();
        }
    }
    #region private Methods
    private void InitializeAudioSources()
    {
        // Basic setup if you create sources programmatically
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        // Configure musicSource for music
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        // ... more settings

        // If you're pooling, you might not have individual sfxSources as fields,
        // but rather generate them in the pool.
    }

    private void InitializeSFXPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            AudioSource newSource = Instantiate(sfxPrefabSource, transform); // Or new AudioSource()
            newSource.gameObject.SetActive(false); // Hide until needed
            sfxSourcePool.Enqueue(newSource);
        }
    }

    private AudioSource GetSFXSource()
    {
        if (sfxSourcePool.Count > 0)
        {
            AudioSource source = sfxSourcePool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }
        else
        {
            // Optionally create more if pool runs out
            Debug.LogWarning("SFX AudioSource pool exhausted, creating new one. Consider increasing pool size.");
            AudioSource newSource = Instantiate(sfxPrefabSource, transform);
            return newSource;
        }
    }

    private void ReturnSFXSource(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        sfxSourcePool.Enqueue(source);
    }
#endregion
    #region public Methods
    public void PlayBackgroundMusic()
    {
        int randomIndex = Random.Range(0, backgroundMusic.Length);
        AudioClip selectedClip = backgroundMusic[randomIndex];

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = selectedClip;
        musicSource.Play();
        Debug.Log($"Playing: {selectedClip.name}");
    }
    #endregion
}
