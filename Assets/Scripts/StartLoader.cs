using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLoader
{
    public const string STARTER_PREFAB_NAME = "StartScripts";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnRuntimeLoaded()
    {
        var singletonObject = GameObject.Instantiate(Resources.Load<GameObject>(STARTER_PREFAB_NAME));
        GameObject.DontDestroyOnLoad(singletonObject);
    }
}
