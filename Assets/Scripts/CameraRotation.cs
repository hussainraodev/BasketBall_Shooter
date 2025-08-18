//using UnityEngine;

//public class CameraRotation : MonoBehaviour
//{
//    private Camera camera;
//    public float minRotationX = -90f;
//    public float maxRotationX = 90f;
//    public float minRotationY = -45f;
//    public float maxRotationY = 45f;

//    private float accumulatedRotationX = 0f;
//    private float accumulatedRotationY = 0f;

//    public float rotationSpeed = 0.02f;
//    public float swipeThreshold = 50f;

//    private bool allowRotation = true;
//    BallLauncher ballLauncher;
//    void Start()
//    {
//        camera = GetComponentInChildren<Camera>();
//        ballLauncher = GetComponent<BallLauncher>();
//    }

//    void Update()
//    {
//        if (allowRotation && Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);

//            if (touch.phase == TouchPhase.Moved)
//            {
//                float swipeDeltaX = touch.deltaPosition.x;
//                float swipeDeltaY = touch.deltaPosition.y;

//                if (Mathf.Abs(swipeDeltaX) > Mathf.Abs(swipeDeltaY))
//                {
//                    float mouseX = swipeDeltaX * rotationSpeed;
//                    accumulatedRotationY += mouseX;
//                    accumulatedRotationY = Mathf.Clamp(accumulatedRotationY, minRotationY, maxRotationY);
//                    transform.localRotation = Quaternion.Euler(0, accumulatedRotationY, 0);
//                }
//                else
//                {
//                    float mouseY = swipeDeltaY * rotationSpeed;
//                    accumulatedRotationX -= mouseY;
//                    accumulatedRotationX = Mathf.Clamp(accumulatedRotationX, minRotationX, maxRotationX);
//                    camera.transform.localRotation = Quaternion.Euler(-accumulatedRotationX, 0, 0);

//                    if (swipeDeltaY > swipeThreshold)
//                    {
//                        Debug.Log("Swipe Up Detected!");
//                        // Disable rotation while swipe up is detected
//                        allowRotation = false;
//                        ballLauncher.Shoot();
//                    }
//                }
//            }
//        }
//        else if (!allowRotation && Input.touchCount == 0)
//        {
//            // Re-enable rotation when no touches are detected
//            allowRotation = true;
//        }
//    }
//}



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

    public float rotationSpeed = 0.2f;     // Slightly bigger for mouse
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
#if UNITY_ANDROID || UNITY_IOS   // ----- MOBILE TOUCH -----
        HandleTouchInput();
#else                            // ----- PC / WEB (MOUSE) -----
        HandleMouseInput();
#endif
    }

    private void HandleTouchInput()
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
                    float mouseX = swipeDeltaX * rotationSpeed * 0.1f; // scale down for touch
                    accumulatedRotationY += mouseX;
                    accumulatedRotationY = Mathf.Clamp(accumulatedRotationY, minRotationY, maxRotationY);
                    transform.localRotation = Quaternion.Euler(0, accumulatedRotationY, 0);
                }
                else
                {
                    float mouseY = swipeDeltaY * rotationSpeed * 0.1f;
                    accumulatedRotationX -= mouseY;
                    accumulatedRotationX = Mathf.Clamp(accumulatedRotationX, minRotationX, maxRotationX);
                    camera.transform.localRotation = Quaternion.Euler(-accumulatedRotationX, 0, 0);

                    if (swipeDeltaY > swipeThreshold)
                    {
                        Debug.Log("Swipe Up Detected!");
                        allowRotation = false;
                        ballLauncher.Shoot();
                    }
                }
            }
        }
        else if (!allowRotation && Input.touchCount == 0)
        {
            allowRotation = true;
        }
    }

    private void HandleMouseInput()
    {
        if (allowRotation)
        {
            // Mouse drag for rotation
            if (Input.GetMouseButton(0)) // Left-click & hold
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

                accumulatedRotationY += mouseX;
                accumulatedRotationY = Mathf.Clamp(accumulatedRotationY, minRotationY, maxRotationY);
                transform.localRotation = Quaternion.Euler(0, accumulatedRotationY, 0);

                accumulatedRotationX -= mouseY;
                accumulatedRotationX = Mathf.Clamp(accumulatedRotationX, minRotationX, maxRotationX);
                camera.transform.localRotation = Quaternion.Euler(-accumulatedRotationX, 0, 0);
            }

            // Shoot on right-click or spacebar
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Shoot Detected!");
                allowRotation = false;
                ballLauncher.Shoot();
            }
        }
        else if (!allowRotation && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            allowRotation = true;
        }
    }
}
