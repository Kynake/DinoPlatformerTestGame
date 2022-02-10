using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public static int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static string GetSceneNameFromBuildIndex(int buildIndex)
    {
        if(buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            return string.Empty;
        }

        // Workaround for finding the name of a scene that's not loaded
        // https://stackoverflow.com/questions/40898310/get-name-of-next-scene-in-build-settings-in-unity3d
        var filepath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        var sceneName = System.IO.Path.GetFileNameWithoutExtension(filepath);

        return sceneName;
    }

    // Like Mathf.Approximately, but more lenient
    public static bool CloseEnough(float x, float y) => ApproxDiff(x, y, CLOSE_ENOUGH_EPSILON);

    // Like Mathf.Approximately, but you can specify a value for epsilon
    public static bool ApproxDiff(float x, float y, float epsilon)
    {
        return Mathf.Approximately(x, y) || Mathf.Abs(x - y) < Mathf.Abs(epsilon);
    }

    public static string StringifyDictionary<K, V>(Dictionary<K, V> dict)
    {
        var sb = new StringBuilder();

        foreach(var kvp in dict)
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");
        }

        return sb.ToString();
    }
}
