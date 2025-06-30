
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float zoomSpeed = 10f;
    public float minZoomDistance = 5f;
    public float maxZoomDistance = 50f;

    private float currentZoomDistance;

    void Start()
    {
        currentZoomDistance = Vector3.Distance(transform.position, Vector3.zero); // Assuming camera looks at origin
    }

    void Update()
    {
        // Camera Rotation (Orbit around origin)
        if (Input.GetMouseButton(1)) // Right mouse button for rotation
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.RotateAround(Vector3.zero, Vector3.up, mouseX * rotationSpeed * Time.deltaTime);
            transform.RotateAround(Vector3.zero, transform.right, -mouseY * rotationSpeed * Time.deltaTime);
        }

        // Camera Zoom (Scroll wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentZoomDistance -= scroll * zoomSpeed;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);

            // Move camera along its forward vector
            transform.position = Vector3.zero - transform.forward * currentZoomDistance;
        }
    }
}


