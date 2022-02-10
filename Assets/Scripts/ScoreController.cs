using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private static ScoreController _instance = null;

    public static Dictionary<int, float> TimePerLevel = new Dictionary<int, float>();
    private static float _attemptTimer = 0;

    public static Dictionary<int, int> FallDeathsPerLevel = new Dictionary<int, int>();
    public static Dictionary<int, int> EnemyDeathsPerLevel = new Dictionary<int, int>();

    public static int Jumps = 0;

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

    // Deaths
    public static int TotalDeaths => EnemyDeaths + FallDeaths;
    public static int EnemyDeaths
    {
        get => GetDeathsOfType(PlayerDeathSources.Enemy);
    }

    public static int FallDeaths
    {
        get => GetDeathsOfType(PlayerDeathSources.Fall);
    }

    public static Dictionary<PlayerDeathSources, int> DeathsByType => new Dictionary<PlayerDeathSources, int>
    {
        { PlayerDeathSources.Fall, FallDeaths },
        { PlayerDeathSources.Enemy, EnemyDeaths }
    };

    public static int TotalDeathsInLevel => GetTotalDeathsInLevel(Utils.GetCurrentSceneIndex());
    private static int GetTotalDeathsInLevel(int sceneIndex)
    {
        var deaths = 0;
        deaths += FallDeathsPerLevel.TryGetValue(sceneIndex, out var fall)? fall : 0;
        deaths += EnemyDeathsPerLevel.TryGetValue(sceneIndex, out var enemy)? enemy : 0;

        return deaths;
    }

    private static Dictionary<int, int> GetDeathSourceDict(PlayerDeathSources deathType)
    {
        Dictionary<int , int> dict;
        switch(deathType)
        {
            case PlayerDeathSources.Fall:
                dict = FallDeathsPerLevel;
                break;

            case PlayerDeathSources.Enemy:
                dict = EnemyDeathsPerLevel;
                break;

            default:
                dict = new Dictionary<int, int>();
                break;
        }

        return dict;
    }

    public static int GetDeathsOfType(PlayerDeathSources deathType) => GetDeathSourceDict(deathType).Values.Sum();

    public static void RegisterDeath(PlayerDeathSources deathType, Vector3 location)
    {
        var dict = GetDeathSourceDict(deathType);
        var sceneIndex = Utils.GetCurrentSceneIndex();
        var attemptTime = CurrentAttemptTimer;
        if(dict.ContainsKey(sceneIndex))
        {
            dict[sceneIndex]++;
        }
        else
        {
            dict[sceneIndex] = 1;
        }

        if(TimePerLevel.ContainsKey(sceneIndex))
        {
            TimePerLevel[sceneIndex] += attemptTime;
        }
        else
        {
            TimePerLevel[sceneIndex] = attemptTime;
        }

        // Register PlayerDeath analytics event
        AnalyticsManager.SendPlayerDeathEvent(location, deathType, attemptTime, TimePerLevel[sceneIndex], GetTotalDeathsInLevel(sceneIndex));
    }

    public static void RegisterFallDeath(Vector3 location) => RegisterDeath(PlayerDeathSources.Fall, location);
    public static void RegisterEnemyDeath(Vector3 location) => RegisterDeath(PlayerDeathSources.Enemy, location);

    // Time/Complete Level
    public static float TotalTime => TimePerLevel.Values.Sum();
    public static float TotalTimeInLevel => GetTotalTimeInLevel(Utils.GetCurrentSceneIndex());

    private static float GetTotalTimeInLevel(int sceneIndex)
    {
        if(TimePerLevel.TryGetValue(sceneIndex, out var time))
        {
            return time;
        }

        return 0;
    }

    public static void StartAttemptStopwatch() => _attemptTimer = Time.time;
    public static float CurrentAttemptTimer => Time.time - _attemptTimer;

    public static void RegisterLevelWin()
    {
        var sceneIndex = Utils.GetCurrentSceneIndex();
        var completedAttemptTime = CurrentAttemptTimer;

        if(TimePerLevel.ContainsKey(sceneIndex))
        {
            TimePerLevel[sceneIndex] += completedAttemptTime;
        }
        else
        {
            TimePerLevel[sceneIndex] = completedAttemptTime;
        }

        // Register LevelWin analytics event
        AnalyticsManager.SendLevelWinEvent(completedAttemptTime, TimePerLevel[sceneIndex], GetTotalDeathsInLevel(sceneIndex));
    }

    // Reset
    public static void ResetStats()
    {
        _attemptTimer = 0;
        TimePerLevel.Clear();
        FallDeathsPerLevel.Clear();
        EnemyDeathsPerLevel.Clear();
        Jumps = 0;
    }
}
