using UnityEngine;
/// This script handles player movement using on-screen buttons and rotation via screen swipes.
/// Attach this script to your player GameObject in Unity.

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The speed at which the player moves.")]
    public float moveSpeed = 5.0f;

    [Header("Rotation Settings")]
    [Tooltip("The sensitivity of screen swipes for player rotation.")]
    public float rotationSpeed = 0.5f; // Adjust this value to change swipe sensitivity

    // Internal flags to track which movement buttons are currently pressed.
    private bool moveForward;
    private bool moveBackward;
    private bool moveLeft;
    private bool moveRight;

    public void OnForwardButtonDown()
    {
        moveForward = true;
    }
    public void OnForwardButtonUp()
    {
        moveForward = false;
    }
    public void OnBackwardButtonDown()
    {
        moveBackward = true;
    }
    public void OnBackwardButtonUp()
    {
        moveBackward = false;
    }
    public void OnLeftButtonDown()
    {
        moveLeft = true;
    }
    public void OnLeftButtonUp()
    {
        moveLeft = false;
    }
    public void OnRightButtonDown()
    {
        moveRight = true;
    }
    public void OnRightButtonUp()
    {
        moveRight = false;
    }
    void Update()
    {
        // --- Handle Player Movement ---
        // Calculate the desired movement vector based on which buttons are currently pressed.
        Vector3 currentMovement = Vector3.zero;
        if (moveForward) currentMovement += Vector3.forward;
        if (moveBackward) currentMovement += Vector3.back;
        if (moveLeft) currentMovement += Vector3.left;
        if (moveRight) currentMovement += Vector3.right;
        // Normalize the vector if moving diagonally to prevent faster movement.
        // If the magnitude is greater than 1 (e.g., moving forward and right), normalize it.
        if (currentMovement.magnitude > 1f)
        {
            currentMovement.Normalize();
        }
        // Apply movement relative to the player's local space (its own forward, back, left, right).
        if (currentMovement != Vector3.zero)
        {
            transform.Translate(currentMovement * moveSpeed * Time.deltaTime, Space.Self);
        }
        // --- Handle Player Rotation (Swipe) ---
        // Check if there's at least one touch on the screen.
        if (Input.touchCount > 0)
        {
            // Get the first touch detected on the screen.
            Touch touch = Input.GetTouch(0);

            // Check if the touch is currently moving (i.e., a swipe).
            if (touch.phase == TouchPhase.Moved)
            {
                // Get the horizontal change in touch position since the last frame.
                // This value will be positive for right swipes and negative for left swipes.
                float swipeDeltaX = touch.deltaPosition.x;

                // Rotate the player around its local Y-axis (up vector) based on the swipe.
                // Multiplying by Time.deltaTime makes the rotation frame-rate independent.
                transform.Rotate(Vector3.up * swipeDeltaX * rotationSpeed, Space.Self);
            }
        }
    }
}
