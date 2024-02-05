using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ScoreKeeper : MonoBehaviour
{
    public int score = 0;
    public GameObject completePanel;
    public GameObject player;
    public Text levelText;
    private void Start()
    {
        levelText.text = PlayerPrefs.GetInt("Level", 1).ToString(); ;
    }
    public void IncrementScore(int amount)
    {
        score += amount;

        ShowComplete();
    }
    public void ShowComplete()
    {
        player.SetActive(false);
        int currentLevel = PlayerPrefs.GetInt("Level",1);
        PlayerPrefs.SetInt("Level",currentLevel+1);
        AdsManager.instance.ShowInterstitialAds();
        FirebaseAnalytics.Event("level" + currentLevel + "_completed", "level" + currentLevel + "__completed", currentLevel.ToString());
        completePanel.SetActive(true);
    }
    public void Next()
    {
        player.SetActive(true);
        score = 0;
        int currentLevel = PlayerPrefs.GetInt("Level", 1);
        completePanel.SetActive(false);
        FirebaseAnalytics.Event("level" + currentLevel + "_started", "level" + currentLevel + "_started", currentLevel.ToString());
    }
}
