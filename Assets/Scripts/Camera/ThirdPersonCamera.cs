using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 2f;

    [SerializeField] private float mouseSensitivity = 3f;

    [SerializeField] private Camera playerCamera;

    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField] private float fovSpeed = 8f;

    [SerializeField] private float normalHeight = 3f;
    [SerializeField] private float sprintHeight = 2.7f;
    [SerializeField] private float heightSpeed = 5f;

    [SerializeField] private PlayerController playerController;

    private InputSystem_Actions inputActions;

    private float rotationX;
    private float rotationY;


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

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        rotationY += lookInput.x * mouseSensitivity;
        rotationX -= lookInput.y * mouseSensitivity;

        rotationX = Mathf.Clamp(rotationX, -20f, 60f);

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        Vector3 offset = rotation * new Vector3(0, height, -distance);

        transform.position = target.position + offset;

        transform.LookAt(target.position);
    }

    private void Update()
    {
        float targetFOV = playerController.IsSprinting
            ? sprintFOV
            : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            fovSpeed * Time.deltaTime
        );

        float targetHeight = playerController.IsSprinting
            ? sprintHeight
            : normalHeight;

        height = Mathf.Lerp(
            height,
            targetHeight,
            heightSpeed * Time.deltaTime
        );
    }
}