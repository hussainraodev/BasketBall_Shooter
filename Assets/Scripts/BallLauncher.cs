using UnityEngine;
using System.Collections;

public class BallLauncher : MonoBehaviour
{
    public GameObject ballPreFab;
    public float ballSpeed = 5.0f;
    private bool isCameraDragging = false;
    void Start() {
        int currentLevel = PlayerPrefs.GetInt("Level", 1);
        FirebaseAnalytics.Event("level" + currentLevel + "_started", "level" + currentLevel + "_started", currentLevel.ToString());
    }
    void Update()
    {
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    GameObject instance = Instantiate(ballPreFab);
        //    instance.transform.position = transform.position;
        //    Rigidbody rb = instance.GetComponent<Rigidbody>();
        //    Camera camera = GetComponentInChildren<Camera>();
        //    rb.velocity = camera.transform.rotation * Vector3.forward * ballSpeed;
        //}
    }
    public void Shoot()
    {
        GameObject instance = Instantiate(ballPreFab);
        instance.transform.position = transform.position;
        Rigidbody rb = instance.GetComponent<Rigidbody>();
        Camera camera = GetComponentInChildren<Camera>();
        rb.velocity = camera.transform.rotation * Vector3.forward * ballSpeed;
    }
}
