using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

// Note: The Debug.Logs in this class are only compiled when in Dev builds,
// since they involve iterating through dicts, which is unnecessary
// in a Production build
public class AnalyticsManager : MonoBehaviour
{
    public const string LEVEL_WIN_EVENT_NAME = "LevelWin";
    public const string PLAYER_DEATH_EVENT_NAME = "PlayerDeath";
    public const string GAME_COMPLETE_EVENT_NAME = "GameComplete";

    private static AnalyticsManager _instance = null;

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;
    }

    private void OnDestroy()
    {
        if(_instance != this)
        {
            return;
        }

        _instance = null;
    }

    // Custom Events
    public static void SendLevelWinEvent(float successfulRunTime, float totalTimeInLevel, int totalDeathsInLevel)
    {
        var data = new Dictionary<string, object> {
            { "Level", Utils.GetCurrentSceneName() },
            { "Successfull_Run_Time", successfulRunTime },
            { "Total_Time_In_Level", totalTimeInLevel },
            { "Total_Deaths_In_Level", totalDeathsInLevel }
        };
        var result = Analytics.CustomEvent(LEVEL_WIN_EVENT_NAME, data);

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log($"[{nameof(SendLevelWinEvent)}]: Analytics Result: {result}. Data:\n{Utils.StringifyDictionary(data)}");
#endif
    }

    public static void SendPlayerDeathEvent(Vector3 deathLocation, PlayerDeathSources type, float failedAttemptTime, float totalTimeInLevel, int totalDeathsInLevel)
    {
        var data = new Dictionary<string, object> {
            { "Level", Utils.GetCurrentSceneName() },
            { "Death_Location", (Vector2) deathLocation },
            { "Death_Type", Enum.GetName(typeof(PlayerDeathSources), type) },
            { "Failed_Attempt_Time", failedAttemptTime },
            { "Total_Time_In_Level", totalTimeInLevel },
            { "Total_Deaths_In_Level", totalDeathsInLevel }
        };
        var result = Analytics.CustomEvent(PLAYER_DEATH_EVENT_NAME, data);

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log($"[{nameof(SendPlayerDeathEvent)}]: Analytics Result: {result}. Data:\n{Utils.StringifyDictionary(data)}");
#endif
    }

    public static void SendGameCompleteEvent(Dictionary<int, float> timePerLevelBreakdown, Dictionary<PlayerDeathSources, int> deathsBreakdown)
    {
        var mappedLevelBreakdown = timePerLevelBreakdown.ToDictionary(
            kvp => $"Time_Taken_{Utils.GetSceneNameFromBuildIndex(kvp.Key)}",
            kvp => kvp.Value
        );

        var mappedDeathsBreakdown = deathsBreakdown.ToDictionary(
            kvp => $"Deaths_By_{Enum.GetName(typeof(PlayerDeathSources), kvp.Key)}",
            kvp => kvp.Value
        );

        var data = new Dictionary<string, object> {
            { "Total_Time", timePerLevelBreakdown.Values.Sum() },
            { "Total_Deaths", deathsBreakdown.Values.Sum() },
        };

        // Flatten both breakdown dictionaries into final Analytics dictionary
        foreach(var kvp in mappedLevelBreakdown)
        {
            data[kvp.Key] = kvp.Value;
        }

        foreach(var kvp in mappedDeathsBreakdown)
        {
            data[kvp.Key] = kvp.Value;
        }

        var result = Analytics.CustomEvent(PLAYER_DEATH_EVENT_NAME, data);

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log($"[{nameof(SendGameCompleteEvent)}]: Analytics Result: {result}. Data:\n{Utils.StringifyDictionary(data)}");
#endif
    }
}
