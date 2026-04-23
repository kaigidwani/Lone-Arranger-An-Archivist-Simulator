using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string TrackName;
    public AudioClip Clip;
}

public class MusicLibrary : MonoBehaviour
{
    // Properties

    public MusicTrack[] Tracks;

    /// <summary>
    /// Returns the audio clip associated with a track
    /// </summary>
    /// <param name="trackName">The name of the track to look for</param>
    /// <returns>If it exists, the AudioClip that is found</returns>
    public AudioClip GetClip(string trackName)
    {
        foreach (MusicTrack t in Tracks)
        {
            if (t.TrackName == trackName)
            {
                return t.Clip;
            }
        }

        return null;
    }
}
