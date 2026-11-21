using UnityEngine;
using UnityEngine.SceneManagement;

// Every "go to the game" button calls SceneLoader.Load("Game") instead of loading the scene directly.
public static class SceneLoader
{
    public static string Target { get; private set; } = "Game";

    public static void Load(string sceneName)
    {
        Target = sceneName;
        Time.timeScale = 1f;                     // the clock survives scene loads, so reset it
        AudioListener.pause = false;
        SceneManager.LoadScene("Loading");       // the loading scene then loads Target asynchronously
    }
}