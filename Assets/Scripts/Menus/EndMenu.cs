using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
    [SerializeField] private Text _totalTime = null;

    [SerializeField] private Text _totalDeaths = null;
    [SerializeField] private Text _enemyDeaths = null;
    [SerializeField] private Text _fallDeaths = null;

    [SerializeField] private Text _jumps = null;

    private void Start()
    {
        var finalTime = StatsController.TotalTime;
        _totalTime.text = $"{(int) finalTime / 60:00}:{finalTime % 60:00.00}";

        _totalDeaths.text = StatsController.TotalDeaths.ToString();
        _enemyDeaths.text = StatsController.EnemyDeaths.ToString();
        _fallDeaths.text = StatsController.FallDeaths.ToString();

        _jumps.text = StatsController.Jumps.ToString();

        // Send game complete analytics
        AnalyticsManager.SendGameCompleteEvent(StatsController.TimePerLevel, StatsController.DeathsByType);
    }

    public void ReturnToMainMenu()
    {
        AdsManager.PlayInterstitialAd(Utils.LoadMainMenu);
    }
}
