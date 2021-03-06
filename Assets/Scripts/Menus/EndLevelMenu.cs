using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelMenu : MonoBehaviour
{
    public string RetryMessage = "You Died";
    public string RetryPrompt = "Retry";
    public string NextLevelMessage = "Level Complete";
    public string NextLevelPrompt = "Continue";

    [SerializeField]
    private GameObject _onScreenControls = null;

    [SerializeField]
    private Text _messageTextField = null;

    [SerializeField]
    private Text _continueTextField = null;

    private bool _shouldRetry = false;

    private void Awake()
    {
        gameObject.SetActive(false);

        PlayerController.OnPlayerWin += PlayerWon;
        PlayerController.OnPlayerDeath += PlayerDead;
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerWin -= PlayerWon;
        PlayerController.OnPlayerDeath -= PlayerDead;
    }

    private void PlayerDead(PlayerController player) => OpenEndLevelScreen(true);
    private void PlayerWon(PlayerController player) => OpenEndLevelScreen(false);
    private void OpenEndLevelScreen(bool shouldRetry)
    {
        _shouldRetry = shouldRetry;
        _messageTextField.text = shouldRetry? RetryMessage : NextLevelMessage;
        _continueTextField.text = shouldRetry? RetryPrompt : NextLevelPrompt;

        _onScreenControls?.SetActive(false);
        gameObject.SetActive(true);
    }

    public void OnPressContinue() => AdsManager.PlayInterstitialAd(LoadScene);

    private void LoadScene()
    {
        if(_shouldRetry)
        {
            Utils.ReloadScene();
        }
        else
        {
            Utils.LoadNextScene();
        }
    }
}
