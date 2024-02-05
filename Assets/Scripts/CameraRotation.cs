using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    private Camera camera;
    public float minRotationX = -90f;
    public float maxRotationX = 90f;
    public float minRotationY = -45f;
    public float maxRotationY = 45f;

    private float accumulatedRotationX = 0f;
    private float accumulatedRotationY = 0f;

    public float rotationSpeed = 0.02f;
    public float swipeThreshold = 50f;

    private bool allowRotation = true;
    BallLauncher ballLauncher;
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        ballLauncher = GetComponent<BallLauncher>();
    }

    void Update()
    {
        if (allowRotation && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float swipeDeltaX = touch.deltaPosition.x;
                float swipeDeltaY = touch.deltaPosition.y;

                if (Mathf.Abs(swipeDeltaX) > Mathf.Abs(swipeDeltaY))
                {
                    float mouseX = swipeDeltaX * rotationSpeed;
                    accumulatedRotationY += mouseX;
                    accumulatedRotationY = Mathf.Clamp(accumulatedRotationY, minRotationY, maxRotationY);
                    transform.localRotation = Quaternion.Euler(0, accumulatedRotationY, 0);
                }
                else
                {
                    float mouseY = swipeDeltaY * rotationSpeed;
                    accumulatedRotationX -= mouseY;
                    accumulatedRotationX = Mathf.Clamp(accumulatedRotationX, minRotationX, maxRotationX);
                    camera.transform.localRotation = Quaternion.Euler(-accumulatedRotationX, 0, 0);

                    if (swipeDeltaY > swipeThreshold)
                    {
                        Debug.Log("Swipe Up Detected!");
                        // Disable rotation while swipe up is detected
                        allowRotation = false;
                        ballLauncher.Shoot();
                    }
                }
            }
        }
        else if (!allowRotation && Input.touchCount == 0)
        {
            // Re-enable rotation when no touches are detected
            allowRotation = true;
        }
    }
}

