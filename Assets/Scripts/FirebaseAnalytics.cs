using UnityEngine;
using Firebase;
using System.Collections;
public class FirebaseAnalytics : MonoBehaviour
{
#if !UNITY_EDITOR
    // Start is called before the first frame update
    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
            Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        });
    }
#endif
    public static void Event(string SceneName, string Identifier, string value)
    {
#if !UNITY_EDITOR

       
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty(Identifier, value);
        Firebase.Analytics.FirebaseAnalytics.LogEvent(SceneName, Identifier, value);


#endif
    }

}
