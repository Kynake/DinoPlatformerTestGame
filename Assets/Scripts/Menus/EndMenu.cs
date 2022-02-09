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
        var finalTime = ScoreController.TotalTime;
        _totalTime.text = $"{(int) finalTime / 60:00}:{finalTime % 60:00.00}";

        _totalDeaths.text = ScoreController.Deaths.ToString();
        _enemyDeaths.text = ScoreController.EnemyDeaths.ToString();
        _fallDeaths.text = ScoreController.FallDeaths.ToString();

        _jumps.text = ScoreController.Jumps.ToString();
    }

    public void ReturnToMainMenu()
    {
        AdsManager.PlayInterstitialAd(Utils.LoadMainMenu);
    }
}
