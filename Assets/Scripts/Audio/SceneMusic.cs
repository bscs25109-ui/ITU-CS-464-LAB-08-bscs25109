using UnityEngine;

// Put this on an empty object in a scene to choose which music that scene starts.
public class SceneMusic : MonoBehaviour
{
    [SerializeField] MusicTrack track = MusicTrack.Menu;
    void Start() => AudioManager.Music(track);
}