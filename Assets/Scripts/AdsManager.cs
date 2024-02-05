using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.Advertisements;
using System.Collections;

public enum AdType
{
    TestAds, LiveAds
}
public enum AdPriority
{
    Admob
}

public class AdsManager : MonoBehaviour//, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{

    public AdType adType;
    public AdPriority adPriority;
    public AdPosition bannerPosition;
    public string appID;

    [Header("Admob Ids Active")]
    public string admobBannerId;
    public string admobRectBannerId;
    public string admobInterstitalId;
    public string admobRewardedInterstitialId;
    public string admobRewardedId;
    public string admobAppOpenId;
    public string unityId;
    public string unityInterstitalId = "Interstitial_Android";
    public string unityRewardedId = "Rewarded_Android";

    //private static bool? _isInitialized;
    private BannerView _bannerView;
    private BannerView _rectBannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    private RewardedInterstitialAd _rewardedInterstitialAd;
    public static Action onRewardedVideoCompleteEvent, onRewardedInterstitialCompleteAction, onRewardedInterstitialSkipAction;

    public delegate void actionInterstitialAdClose();
    public event actionInterstitialAdClose onInterstitialAdCloseEvent;

    public static AdsManager instance;


    WaitForSecondsRealtime constantDelay_Zero_Two = new WaitForSecondsRealtime(.2f);
    public static AdsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AdsManager>();
                if (instance && instance.gameObject)
                {
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            //If I am the first instance, make me the Singleton
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        SetAdIds();
       
        // Use the AppStateEventNotifier to listen to application open/close events.
        // This is used to launch the loaded ad when we open the app.
    }
    IEnumerator Start()
    {
        yield return constantDelay_Zero_Two;
        InitializeGoogleMobileAds();
        yield return constantDelay_Zero_Two;
        DelayAdmobInterstitialLoad();
        yield return constantDelay_Zero_Two;
        DelayAdmobRewardedVideoLoad();
        yield return constantDelay_Zero_Two;
        LoadRewardedInterstitialAd();
        LoadBanner();
   
    }

    #region initialization events
    /// <summary>
    /// Initializes the Google Mobile Ads Unity plugin.
    /// </summary>

    private void InitializeGoogleMobileAds()
    {
        MobileAds.Initialize(initStatus => { Debug.Log("Initialized"); });
       
    }
   


    private void SetAdIds()
    {
        if (adType == AdType.TestAds)
        {
            appID = "ca-app-pub-3940256099942544~3347511713";
            admobInterstitalId = "ca-app-pub-3940256099942544/1033173712";
            admobBannerId = "ca-app-pub-3940256099942544/6300978111";
            admobRectBannerId = "ca-app-pub-3940256099942544/6300978111";
            admobRewardedId = "ca-app-pub-3940256099942544/5224354917";
            admobRewardedInterstitialId = "ca-app-pub-3940256099942544/5354046379";
            admobAppOpenId = "ca-app-pub-3940256099942544/9257395921";
            Debug.unityLogger.logEnabled = true;
        }
        
    }
    #endregion initialization events
    #region ad calling methods
   
    public void ShowRewardedVideo(Action rewardedFuction)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        Debug.Log(adPriority);
        onRewardedVideoCompleteEvent = null;
        onRewardedVideoCompleteEvent = rewardedFuction;
        if (adPriority == AdPriority.Admob)
        {
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                ShowAdmobRewardedAd();
            }
            else
            {

                DelayAdmobRewardedVideoLoad();
                Debug.LogError("Rewarded ad is not ready yet.");
            }
        }
    }
    public void ShowInterstitialAds()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        if (adPriority == AdPriority.Admob)
        {
            Debug.Log(1);
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log(1.1);
                ShowAdmobInterstitialAd();
            }
            else
            {
                Debug.Log(1.2);

                DelayAdmobInterstitialLoad();
            }
        }
       
    }
    public void ShowRewardedInterstitial(Action rewardFunction,Action skipFunction)
    {
        if (onRewardedInterstitialCompleteAction != null)
            onRewardedInterstitialCompleteAction = null;
        onRewardedInterstitialCompleteAction = rewardFunction;
        onRewardedInterstitialSkipAction = skipFunction;
        ShowRewardedInterstitialAd();
    }
    #endregion ad calling methods
    #region rewardedInterstitial
    public void DelayAdmobRewardedInterstitialLoad()
    {
        Invoke(nameof(LoadRewardedInterstitialAd), 0.5f);
    }
    public void LoadRewardedInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedInterstitialAd != null)
        {
            DestroyRewardedInterstitialAd();
        }

        Debug.Log("Loading rewarded interstitial ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedInterstitialAd.Load(admobRewardedInterstitialId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("Rewarded interstitial ad failed to load an ad with error : "
                                    + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpexted error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Rewarded interstitial load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Rewarded interstitial ad loaded with response : "
                + ad.GetResponseInfo());
                _rewardedInterstitialAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);

                // Inform the UI that the ad is ready.
                // AdLoadedStatus?.SetActive(true);
            });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public bool isRewardedInterstitialAdReady()
    {
        if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
        {
            return true;
        }
        else
            return false;
    }
    private void ShowRewardedInterstitialAd()
    {
        if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
        {
            _rewardedInterstitialAd.Show((Reward reward) =>
            {
                Debug.Log("Rewarded interstitial ad rewarded : " + reward.Amount);
            });
        }
        else
        {
            DelayAdmobRewardedInterstitialLoad();
            Debug.LogError("Rewarded interstitial ad is not ready yet.");
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus?.SetActive(false);
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyRewardedInterstitialAd()
    {
        if (_rewardedInterstitialAd != null)
        {
            Debug.Log("Destroying rewarded interstitial ad.");
            _rewardedInterstitialAd.Destroy();
            _rewardedInterstitialAd = null;
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus?.SetActive(false);
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (_rewardedInterstitialAd != null)
        {
            var responseInfo = _rewardedInterstitialAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    protected void RegisterEventHandlers(RewardedInterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
           
            Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            //if (onRewardedInterstitialSkipAction != null)
            //{
            //    StartCoroutine(onRewardedInterstitialSkippedEventDelayed(.2f));
            //}
            if (onRewardedInterstitialCompleteAction != null)
            {

                //isRewardedInterstitialGranted = true;
                StartCoroutine(onRewardedInterstitialCompleteEventDelayed(.1f));

            }
            DelayAdmobRewardedInterstitialLoad();
            Debug.Log("Rewarded interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open full screen content" +
                           " with error : " + error);
            StartCoroutine(onRewardedInterstitialSkippedEventDelayed(.2f));
            DelayAdmobRewardedInterstitialLoad();
        };
    }
    IEnumerator onRewardedInterstitialCompleteEventDelayed(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (onRewardedInterstitialCompleteAction != null)
        {
            onRewardedInterstitialCompleteAction.Invoke();
            onRewardedInterstitialCompleteAction = null;
            if (onRewardedInterstitialSkipAction != null)
                onRewardedInterstitialSkipAction = null;
        }

    }
    IEnumerator onRewardedInterstitialSkippedEventDelayed(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (onRewardedInterstitialSkipAction != null)
        {
            onRewardedInterstitialSkipAction.Invoke();
            onRewardedInterstitialSkipAction = null;
            if (onRewardedInterstitialCompleteAction != null)
                onRewardedInterstitialCompleteAction = null;

        }
        StartCoroutine(unloadRewardedInterstitialAction(0.2f));
        StartCoroutine(RewardedInterstitialDelayLoad(.6f));
    }
    void unloadingActionsOnSKip()
    {
        StartCoroutine(unloadRewardedInterstitialAction(0f));
    }

    IEnumerator unloadRewardedInterstitialAction(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (onRewardedInterstitialCompleteAction != null)
            onRewardedInterstitialCompleteAction = null;
        if (onRewardedInterstitialSkipAction != null)
            onRewardedInterstitialSkipAction = null;
    }
    IEnumerator RewardedInterstitialDelayLoad(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        DelayAdmobRewardedInterstitialLoad();
    }
    #endregion rewardedInterstitial
    #region admob banner ad
    /// <summary>
    /// Creates a 320x50 banner at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view.");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBanner();
        }

        // Create a 320x50 banner at top of the screen.
        _bannerView = new BannerView(admobBannerId, AdSize.Banner, bannerPosition);
        Debug.Log(admobBannerId);
        // Listen to events the banner may raise.
        ListenToAdEvents();

        Debug.Log("Banner view created.");
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBanner()
    {
        // Create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowBanner()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        if (_bannerView != null)
        {
            Debug.Log("Showing admob banner view.");
            _bannerView.Show();
        }
        else
        {
            LoadBanner();//else load ad
        }
    }

    /// <summary>
    /// Hides the ad.
    /// </summary>
    public void HideBanner()
    {
        if (_bannerView != null)
        {
            Debug.Log("Hiding banner view.");
            _bannerView.Hide();
        }
    }

    /// <summary>
    /// Destroys the ad.
    /// When you are finished with a BannerView, make sure to call
    /// the Destroy() method before dropping your reference to it.
    /// </summary>
    public void DestroyBanner()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    /// <summary>
    /// Listen to events the banner may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    #endregion end admob banner
    #region admob Rect banner ad
    /// <summary>
    /// Creates a 320x50 banner at top of the screen.
    /// </summary>
    public void CreateRectBannerView(AdPosition _adPosition)
    {
        Debug.Log("Creating rect banner view.");

        // If we already have a banner, destroy the old one.
        if (_rectBannerView != null)
        {
            DestroyRectBanner();
        }

        // Create a 320x50 banner at top of the screen.
        _rectBannerView = new BannerView(admobRectBannerId, AdSize.MediumRectangle, _adPosition);

        // Listen to events the banner may raise.
        ListenToAdEventsRectBanner();

        Debug.Log("rect Banner view created.");
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadRectBanner(AdPosition _adPosition)
    {
        // Create an instance of a banner view first.
        if (_rectBannerView == null)
        {
            CreateRectBannerView(_adPosition);
        }

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        Debug.Log("Loading rect banner ad.");
        _rectBannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowRectBanner(AdPosition _adPosition)
    {
        if (_rectBannerView != null)
        {
            Debug.Log("Showing rect banner view.");
            _rectBannerView.Show();
        }
        else
        {
            LoadRectBanner(_adPosition);//else load ad
        }
    }

    /// <summary>
    /// Hides the ad.
    /// </summary>
    public void HideRectBanner()
    {
        if (_rectBannerView != null)
        {
            Debug.Log("Hiding rect banner view.");
            _rectBannerView.Hide();
        }
    }

    /// <summary>
    /// Destroys the ad.
    /// When you are finished with a BannerView, make sure to call
    /// the Destroy() method before dropping your reference to it.
    /// </summary>
    public void DestroyRectBanner()
    {
        if (_rectBannerView != null)
        {
            Debug.Log("Destroying rect banner view.");
            _rectBannerView.Destroy();
            _rectBannerView = null;
        }
    }

    /// <summary>
    /// Listen to events the banner may raise.
    /// </summary>
    private void ListenToAdEventsRectBanner()
    {
        // Raised when an ad is loaded into the banner view.
        _rectBannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _rectBannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _rectBannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
        };
        // Raised when the ad is estimated to have earned money.
        _rectBannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _rectBannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _rectBannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _rectBannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _rectBannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    #endregion end admob Rect banner
    #region admob interstitial Ad
    public void DelayAdmobInterstitialLoad()
    {
        Invoke(nameof(LoadAdmobInterstitialAd),0.5f);
    }
    public void LoadAdmobInterstitialAd()
    {
        
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            DestroyAdmobInterstitialAd();
        }

        Debug.Log("Loading interstitial ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        InterstitialAd.Load(admobInterstitalId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
            _interstitialAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);
        });
        DelayAdmobRewardedInterstitialLoad();
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAdmobInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            DelayAdmobInterstitialLoad();
            Debug.LogError("Interstitial ad is not ready yet.");
        }

    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAdmobInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            Debug.Log("Destroying interstitial ad.");
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

    }


    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            DelayAdmobInterstitialLoad();

        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            DelayAdmobInterstitialLoad();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : "
                + error);
            DelayAdmobInterstitialLoad();
        };

    }
    #endregion end admob interstitial ad
    #region admob rewarded Ad
    public void DelayAdmobRewardedVideoLoad()
    {
        Invoke(nameof(LoadAdmobRewardedAd), 0.5f);
    }
    public void LoadAdmobRewardedAd()
    {
      
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            DestroyAdmobRewardedAd();
        }

        Debug.Log("Loading rewarded ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedAd.Load(admobRewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);


        });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAdmobRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad.");
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(String.Format("Rewarded ad granted a reward: {0} {1}",
                                        reward.Amount,
                                        reward.Type));
            });
        }
        else
        {
            DelayAdmobRewardedVideoLoad();
            Debug.LogError("Rewarded ad is not ready yet.");
        }
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAdmobRewardedAd()
    {
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }


    }
    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            if (onRewardedVideoCompleteEvent != null)
            {
                onRewardedVideoCompleteEvent.Invoke();
                onRewardedVideoCompleteEvent = null;
            }
            DelayAdmobRewardedVideoLoad();
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            if (onRewardedVideoCompleteEvent != null)
            {
                onRewardedVideoCompleteEvent.Invoke();
                onRewardedVideoCompleteEvent = null;
            }
            Debug.Log("Rewarded ad full screen content closed.");
            DelayAdmobRewardedVideoLoad();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : "
                + error);
            DelayAdmobRewardedVideoLoad();
        };

    }
    #endregion end admob rewarded ad
    //#region unity interstitial Ad
    //#endregion end unity interstitial ad
    #region admob open app Ad
    // App open ads can be preloaded for up to 4 hours.
    //private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
    //private DateTime _expireTime;
    //private AppOpenAd _appOpenAd;



    //private void OnDestroy()
    //{
    //    // Always unlisten to events when complete.
    //    AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    //}

    ///// <summary>
    ///// Loads the ad.
    ///// </summary>
    //public void LoadAppOpenAd()
    //{
    //    // Clean up the old ad before loading a new one.
    //    if (_appOpenAd != null)
    //    {
    //        DestroyAppOpenAd();
    //    }

    //    Debug.Log("Loading app open ad.");

    //    // Create our request used to load the ad.
    //    var adRequest = new AdRequest();

    //    // Send the request to load the ad.
    //    AppOpenAd.Load(admobAppOpenId, adRequest, (AppOpenAd ad, LoadAdError error) =>
    //    {
    //        // If the operation failed with a reason.
    //        if (error != null)
    //        {
    //            Debug.LogError("App open ad failed to load an ad with error : "
    //                            + error);
    //            return;
    //        }

    //        // If the operation failed for unknown reasons.
    //        // This is an unexpected error, please report this bug if it happens.
    //        if (ad == null)
    //        {
    //            Debug.LogError("Unexpected error: App open ad load event fired with " +
    //                           " null ad and null error.");
    //            return;
    //        }

    //        // The operation completed successfully.
    //        Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());
    //        _appOpenAd = ad;

    //        // App open ads can be preloaded for up to 4 hours.
    //        _expireTime = DateTime.Now + TIMEOUT;

    //        // Register to ad events to extend functionality.
    //        RegisterEventHandlers(ad);


    //    });
    //}

    ///// <summary>
    ///// Shows the ad.
    ///// </summary>
    //public void ShowAppOpenAd()
    //{
    //    // App open ads can be preloaded for up to 4 hours.
    //    if (_appOpenAd != null && _appOpenAd.CanShowAd())
    //    {
    //        Debug.Log("Showing app open ad.");
    //        _appOpenAd.Show();
    //    }
    //    else
    //    {
    //        Debug.LogError("App open ad is not ready yet.");
    //    }

    //}

    ///// <summary>
    ///// Destroys the ad.
    ///// </summary>
    //public void DestroyAppOpenAd()
    //{
    //    if (_appOpenAd != null)
    //    {
    //        Debug.Log("Destroying app open ad.");
    //        _appOpenAd.Destroy();
    //        _appOpenAd = null;
    //    }
    //}

    //private void OnAppStateChanged(AppState state)
    //{
    //    Debug.Log("App State changed to : " + state);

    //    // If the app is Foregrounded and the ad is available, show it.
    //    if (state == AppState.Foreground)
    //    {
    //        ShowAppOpenAd();
    //    }
    //}

    //private void RegisterEventHandlers(AppOpenAd ad)
    //{
    //    // Raised when the ad is estimated to have earned money.
    //    ad.OnAdPaid += (AdValue adValue) =>
    //    {
    //        Debug.Log(String.Format("App open ad paid {0} {1}.",
    //            adValue.Value,
    //            adValue.CurrencyCode));
    //    };
    //    // Raised when an impression is recorded for an ad.
    //    ad.OnAdImpressionRecorded += () =>
    //    {
    //        Debug.Log("App open ad recorded an impression.");
    //    };
    //    // Raised when a click is recorded for an ad.
    //    ad.OnAdClicked += () =>
    //    {
    //        Debug.Log("App open ad was clicked.");
    //    };
    //    // Raised when an ad opened full screen content.
    //    ad.OnAdFullScreenContentOpened += () =>
    //    {
    //        Debug.Log("App open ad full screen content opened.");

    //    };
    //    // Raised when the ad closed full screen content.
    //    ad.OnAdFullScreenContentClosed += () =>
    //    {
    //        Debug.Log("App open ad full screen content closed.");

    //        // It may be useful to load a new ad when the current one is complete.
    //        LoadAppOpenAd();
    //    };
    //    // Raised when the ad failed to open full screen content.
    //    ad.OnAdFullScreenContentFailed += (AdError error) =>
    //    {
    //        Debug.LogError("App open ad failed to open full screen content with error : "
    //                        + error);
    //    };
    //}
    #endregion end open app ad

}
