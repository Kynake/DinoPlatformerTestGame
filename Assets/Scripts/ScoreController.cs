using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private static ScoreController _instance = null;

    private int _enemyDeaths = 0;
    private int _fallDeaths = 0;

    private int _jumps = 0;

    private float _timerStart = 0;

    public static int Deaths => _instance._enemyDeaths + _instance._fallDeaths;
    public static int EnemyDeaths
    {
        get => _instance._enemyDeaths;
        set => _instance._enemyDeaths = value;
    }

    public static int FallDeaths
    {
        get => _instance._fallDeaths;
        set => _instance._fallDeaths = value;
    }

    public static float TotalTime => Time.time - _instance._timerStart;

    public static int Jumps
    {
        get => _instance._jumps;
        set => _instance._jumps = value;
    }

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
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

    public static void StartTimer() => _instance._timerStart = Time.time;

    public static void ResetScore() => _instance.ResetInstanceStats();
    private void ResetInstanceStats()
    {
        _enemyDeaths = 0;
        _fallDeaths = 0;
        _jumps = 0;
        _timerStart = 0;
    }
}
