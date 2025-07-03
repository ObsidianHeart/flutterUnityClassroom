using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private Joystick joystick; // Reference to a UI Joystick for movement

    [Header("Look Settings")]
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 90.0f; // Clamp vertical look
    [SerializeField] private RectTransform lookTouchArea; // UI area for camera look input

    private float rotationX = 0;
    private Vector2 touchStartPos;
    private CharacterController characterController;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }

        if (lookTouchArea != null)
        {
            EventTrigger trigger = lookTouchArea.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = lookTouchArea.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
            entryPointerDown.eventID = EventTriggerType.PointerDown;
            entryPointerDown.callback.AddListener((data) => { OnPointerDownLookArea((PointerEventData)data); });
            trigger.triggers.Add(entryPointerDown);

            EventTrigger.Entry entryDrag = new EventTrigger.Entry();
            entryDrag.eventID = EventTriggerType.Drag;
            entryDrag.callback.AddListener((data) => { OnDragLookArea((PointerEventData)data); });
            trigger.triggers.Add(entryDrag);
        }
    }

    void Update()
    {
        HandleMovement();
        // Look is handled by touch events
    }

    void HandleMovement()
    {
        float currentMoveSpeed = moveSpeed;
        Vector3 moveDirection = Vector3.zero;

        if (joystick != null)
        {
            moveDirection = new Vector3(joystick.stick.x.ReadValue(), 0, joystick.stick.y.ReadValue());
        }
        else
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            moveDirection = new Vector3(horizontal, 0, vertical);
        }

        if (moveDirection.sqrMagnitude > 1)
            moveDirection.Normalize();

        // Move relative to the player's facing direction
        Vector3 move = transform.TransformDirection(moveDirection) * currentMoveSpeed;

        // Apply gravity
        move.y += Physics.gravity.y * Time.deltaTime;

        characterController.Move(move * Time.deltaTime);
    }

    public void OnPointerDownLookArea(PointerEventData eventData)
    {
        touchStartPos = eventData.position;
    }

    public void OnDragLookArea(PointerEventData eventData)
    {
        Vector2 touchDelta = eventData.delta;

        // Horizontal rotation (Yaw)
        transform.Rotate(0, touchDelta.x * lookSpeed * 0.1f, 0);

        // Vertical rotation (Pitch)
        rotationX += -touchDelta.y * lookSpeed * 0.1f;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        // Apply pitch to camera or child object (not the whole body)
        if (Camera.main != null)
        {
            Camera.main.transform.localEulerAngles = new Vector3(rotationX, 0, 0);
        }
    }
}


