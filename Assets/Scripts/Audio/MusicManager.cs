using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    // Fields

    [SerializeField] private MusicLibrary _musicLibrary;

    [SerializeField] private AudioSource _musicSource;

    // Properties

    public static MusicManager Instance;

    public string CurrentTrack = "None";

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

    public void PlayTrack(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(CrossfadeTrack(_musicLibrary.GetClip(trackName), fadeDuration));
        CurrentTrack = trackName;
    }

    private IEnumerator CrossfadeTrack(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;

        // Fade out current track
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            _musicSource.volume = Mathf.Lerp(1f, 0, percent);
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
                _musicSource.volume = Mathf.Lerp(0, 1f, percent);
                yield return null;
            }
        }
    }
}
