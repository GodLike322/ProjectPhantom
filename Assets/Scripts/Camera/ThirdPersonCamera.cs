using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float distance = 5f;
    //[SerializeField] private float height = 2f;

    [SerializeField] private float mouseSensitivity = 3f;

    [SerializeField] private Camera playerCamera;

    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField] private float fovSpeed = 8f;

    [SerializeField] private float normalHeight = 3f;
    [SerializeField] private float sprintHeight = 2.7f;
    [SerializeField] private float heightSpeed = 5f;

    [SerializeField] private PlayerController playerController;

    [SerializeField] private float standingHeight = 3f;
    [SerializeField] private float crouchHeight = 2f;
    [SerializeField] private float proneHeight = 0.8f;

    [SerializeField] private float heightSmoothSpeed = 8f;

    [SerializeField] private float crouchFOV = 55f;
    [SerializeField] private float proneFOV = 50f;

    [SerializeField] private float crouchTilt = 3f;
    [SerializeField] private float proneTilt = 7f;
    [SerializeField] private float tiltSpeed = 5f;

    private float currentTilt;

    private InputSystem_Actions inputActions;

    private float rotationX;
    private float rotationY;

    private float currentHeight;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        currentHeight = standingHeight;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        rotationY += lookInput.x * mouseSensitivity;
        rotationX -= lookInput.y * mouseSensitivity;

        rotationX = Mathf.Clamp(rotationX, -20f, 60f);

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        Vector3 offset = rotation * new Vector3(0, currentHeight, -distance);

        transform.position = target.position + offset;

        transform.LookAt(target.position);

        float targetTilt = 0f;

        switch (playerController.CurrentState)
        {
            case PlayerState.Crouching:
                targetTilt = crouchTilt;
                break;

            case PlayerState.Prone:
                targetTilt = proneTilt;
                break;
        }

        currentTilt = Mathf.Lerp(
            currentTilt,
            targetTilt,
            tiltSpeed * Time.deltaTime
        );

        transform.localRotation *= Quaternion.Euler(
            0,
            0,
            currentTilt
        );

        float targetHeight = GetTargetHeight();

        currentHeight = Mathf.Lerp(
            currentHeight,
            targetHeight,
            heightSmoothSpeed * Time.deltaTime
        );
    }

    private void Update()
    {
        float targetFOV;

        if (playerController.IsBoosting)
        {
            targetFOV = sprintFOV;
        }
        else
        {
            switch (playerController.CurrentState)
            {
                case PlayerState.Crouching:
                    targetFOV = crouchFOV;
                    break;

                case PlayerState.Prone:
                    targetFOV = proneFOV;
                    break;

                default:
                    targetFOV = normalFOV;
                    break;
            }
        }

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            fovSpeed * Time.deltaTime
        );
    }

    // Determines the target height of the camera based on the player's current state (standing, crouching, prone).
    private float GetTargetHeight()
    {
        switch (playerController.CurrentState)
        {
            case PlayerState.Crouching:
                return crouchHeight;

            case PlayerState.Prone:
                return proneHeight;

            default:
                return standingHeight;
        }
    }
}