using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;

    [Header("Look Settings")]
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 90.0f; // Clamp vertical look

    private float rotationX = 0;

    void Start()
    {
        // Lock cursor and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMoveSpeed *= sprintSpeedMultiplier;
        }

        // Get input for movement (WASD)
        float horizontal = Input.GetAxis("Horizontal"); // A/D keys
        float vertical = Input.GetAxis("Vertical");     // W/S keys

        // Calculate movement direction relative to camera's forward/right
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Ensure movement is horizontal and not affected by camera pitch
        forward.y = 0; 
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        // Apply movement
        transform.position += moveDirection * currentMoveSpeed * Time.deltaTime;

        // Optional: Add vertical movement (Q/E or Space/Ctrl)
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * currentMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position += Vector3.down * currentMoveSpeed * Time.deltaTime;
        }
    }

    void HandleLook()
    {
        // Get mouse input for looking
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Re-lock cursor when application gains focus
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

