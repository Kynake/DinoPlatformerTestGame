using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnClickStart()
    {
        ScoreController.ResetStats();
        Utils.LoadNextScene();

        print(Utils.GetSceneNameFromBuildIndex(1));
    }
}
