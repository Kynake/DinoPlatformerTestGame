using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMenu : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Utils.LoadNextScene();
    }
}
