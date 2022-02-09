using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils
{
    public const float CLOSE_ENOUGH_EPSILON = 0.001f;

    public static void LoadNextScene()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings, LoadSceneMode.Single);
    }

    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    // Like Mathf.Approximately, but more lenient
    public static bool CloseEnough(float x, float y) => ApproxDiff(x, y, CLOSE_ENOUGH_EPSILON);

    // Like Mathf.Approximately, but you can specify a value for epsilon
    public static bool ApproxDiff(float x, float y, float epsilon)
    {
        return Mathf.Approximately(x, y) || Mathf.Abs(x - y) < Mathf.Abs(epsilon);
    }
}
