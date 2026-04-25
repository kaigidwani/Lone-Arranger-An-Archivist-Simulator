using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    // Fields

    [SerializeField] private MusicLibrary _musicLibrary;

    [SerializeField] private AudioSource _musicSource;

    // Properties

    public static MusicManager Instance;

    public float Volume = 0.95f; // will be used later for settings

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Plays a song if it exists in the music library
    /// </summary>
    /// <param name="trackName">The name of the track to play</param>
    /// <param name="fadeDuration">How long to fade out/in</param>
    public void PlayTrack(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(CrossfadeTrack(_musicLibrary.GetClip(trackName), fadeDuration));
    }

    private IEnumerator CrossfadeTrack(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;

        // Fade out current track
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            _musicSource.volume = Mathf.Lerp(Volume, 0, percent);
            yield return null;
        }

        _musicSource.Stop();

        if (nextTrack != null)
        {
            _musicSource.clip = nextTrack;
            _musicSource.Play();

            // Fade in new track
            percent = 0;
            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / fadeDuration;
                _musicSource.volume = Mathf.Lerp(0, Volume, percent);
                yield return null;
            }
        }
    }
}
