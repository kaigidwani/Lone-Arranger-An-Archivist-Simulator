using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string TrackName;
    public AudioClip Clip;
}

public class MusicLibrary : MonoBehaviour
{

    // Fields

    // Properties

    public MusicTrack[] Tracks;

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
