using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{

#if UNITY_ANDROID
    public const string GameID = "4603985";
    public const string INTERSTITIAL_ID = "Interstitial_Android";
#elif UNITY_IOS
    public const string GameID = "4603984";
    public const string INTERSTITIAL_ID = "Interstitial_iOS";
#else
    public const string GameID = "";
    public const string INTERSTITIAL_ID = "";
#endif

    private static AdsManager _instance = null;

    private delegate void PlayAd(string id);
    private PlayAd OnPlayAd;

    private Action OnAdCompleteAction = null;

    [Tooltip("A time in seconds within which no ads will ever be shown upon starting the game")]
    public float MinimumFirstAdTime;

    [Tooltip("The minimum time interval, in seconds, between one ad and the next. This is to prevent player fatigue from being shown multiple ads too quickly")]
    public float MinimumAdInterval;
    private float _lastAdSeenTime;

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;

        Advertisement.Initialize(GameID);

        _lastAdSeenTime = MinimumFirstAdTime - MinimumAdInterval;
    }

    private void OnDestroy()
    {
        if(_instance != this)
        {
            return;
        }

        _instance = null;
    }

    // Static external calls
    public static void PlayInterstitialAd(Action onAdComplete)
    {
        if(!Advertisement.isInitialized)
        {
            onAdComplete?.Invoke();

            // Try to initialize ads again, if we haven't been able to
            Advertisement.Initialize(GameID);
            return;
        }

        if(Advertisement.isShowing || Time.realtimeSinceStartup - _instance._lastAdSeenTime < _instance.MinimumAdInterval)
        {
            onAdComplete?.Invoke();
            return;
        }

        _instance.OnPlayAd = _instance.ShowInsterstitialAd;
        _instance.OnAdCompleteAction = onAdComplete;

        Advertisement.Load(INTERSTITIAL_ID, _instance);
    }

    // Internal singleton methods
    private void ShowInsterstitialAd(string id)
    {
        OnPlayAd = null;
        Advertisement.Show(INTERSTITIAL_ID, this);
    }

    // Advertisement Events
    public void OnUnityAdsAdLoaded(string id) {
        OnPlayAd?.Invoke(id);
    }

    public void OnUnityAdsFailedToLoad(string unitID, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {unitID} - {error.ToString()} - {message}");

        // If we fail to load an ad for some reason, skip it for now so the game doesn't get stuck
        OnAdCompleteAction?.Invoke();
        OnAdCompleteAction = null;
    }

    public void OnUnityAdsShowFailure(string unitID, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {unitID}: {error.ToString()} - {message}");

        // If we fail to load an ad for some reason, skip it for now so the game doesn't get stuck
        OnAdCompleteAction?.Invoke();
        OnAdCompleteAction = null;
    }

    public void OnUnityAdsShowComplete(string unitID, UnityAdsShowCompletionState showCompletionState) {
        _lastAdSeenTime = Time.realtimeSinceStartup;
        OnAdCompleteAction?.Invoke();
        OnAdCompleteAction = null;
    }

    public void OnUnityAdsShowStart(string unitID) { }
    public void OnUnityAdsShowClick(string unitID) { }
}
