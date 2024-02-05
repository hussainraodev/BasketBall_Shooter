using UnityEngine;
using System.Collections;

public class DetectScoring : MonoBehaviour {
    public int scorePerHit = 1;
    ScoreKeeper scoreKeeper;

    void Start() {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
    }
    void Update() { }

    private void OnCollisionEnter(Collision collision)
    {
        scoreKeeper.IncrementScore(scorePerHit);
    }
}
