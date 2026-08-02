using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float distance = 7f;

    [SerializeField] private float mouseSensitivity = 3f;

    [SerializeField] private Camera playerCamera;

    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 75f;
    [SerializeField] private float fovSpeed = 8f;

    [SerializeField] private PlayerController playerController;

    [SerializeField] private float standingHeight = 3.5f;
    [SerializeField] private float crouchHeight = 2f;
    [SerializeField] private float proneHeight = 0.8f;

    [SerializeField] private float heightSmoothSpeed = 8f;

    [SerializeField] private float crouchFOV = 55f;
    [SerializeField] private float proneFOV = 50f;

    [SerializeField] private float crouchTilt = 3f;
    [SerializeField] private float proneTilt = 7f;
    [SerializeField] private float tiltSpeed = 5f;

    [SerializeField] private Transform cameraPivot;

    [SerializeField] private Transform cameraHolder;

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

        // 1. Считывание мыши и вращение пивота вокруг игрока
        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        rotationY += lookInput.x * mouseSensitivity;
        rotationX -= lookInput.y * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -20f, 60f); // Ограничение вертикального обзора

        // Принудительно держим пивот строго в точке CameraTarget игрока
        cameraPivot.position = target.position;
        cameraPivot.rotation = Quaternion.Euler(rotationX, rotationY, 0);

        // 2. Расчет и плавное изменение высоты камеры в зависимости от состояния
        float targetHeight = GetTargetHeight();
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, heightSmoothSpeed * Time.deltaTime);

        // 3. Смещение холдера камеры (двигаем назад на расстояние distance и вверх на currentHeight)
        // Сама Main Camera должна быть внутри CameraHolder в нулевых координатах!
        cameraHolder.localPosition = new Vector3(0, currentHeight, -distance);
    }


    private void Update()
    {
        float targetFOV;

        if (playerController.IsBoosting &&
            playerController.CurrentState == PlayerState.Standing)
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