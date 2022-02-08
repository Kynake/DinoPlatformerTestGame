using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnClickStart()
    {
        ScoreController.ResetScore();
        ScoreController.StartTimer();
        Utils.LoadNextScene();
    }
}
