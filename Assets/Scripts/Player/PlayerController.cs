using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private InputSystem_Actions inputActions;

    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float standingCenter = 1f;
    [SerializeField] private float crouchCenter = 0.5f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchSpeed = 8f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float crouchMoveSpeed = 2f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float proneHeight = 0.5f;
    [SerializeField] private float proneMoveSpeed = 1f;
    [SerializeField] private float proneCenter = 0.25f;
    [SerializeField] private float crouchSprintSpeed = 3.5f;
    [SerializeField] private float proneSprintSpeed = 1.8f;
    [SerializeField] private float slowWalkSpeed = 2.5f;
    [SerializeField] private float slowCrouchSpeed = 1f;
    [SerializeField] private float slowProneSpeed = 0.5f;
    [SerializeField] private Transform visualMeshTransform;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 10f;

    private float currentSpeed;

    public bool IsBoosting { get; private set; }

    private PlayerState playerState = PlayerState.Standing;
    private Vector3 velocity;
    private Stamina stamina;
    public PlayerState CurrentState => playerState;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();

        stamina = GetComponent<Stamina>();
        currentSpeed = walkSpeed;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        bool sprintButton = inputActions.Player.Sprint.IsPressed();

        IsBoosting = sprintButton && stamina.CanSprint();

        Debug.Log(
            $"State: {playerState}, Shift: {sprintButton}, CanSprint: {stamina.CanSprint()}, Sprint: {IsBoosting}, Stamina: {stamina.CurrentStamina}"
        );

        bool isWalkingSlow = inputActions.Player.Walk.IsPressed();

        Vector3 movement = GetMovement();

        HandleRotation(movement);

        HandleGravity();

        HandleJump();

        float targetSpeed = GetCurrentSpeed(IsBoosting, isWalkingSlow);

        float speedChange = targetSpeed > currentSpeed ? acceleration : deceleration;

        currentSpeed = Mathf.Lerp(
            currentSpeed,
            targetSpeed,
            speedChange * Time.deltaTime
        );

        Vector3 finalMovement = (movement * currentSpeed) + velocity;

        characterController.Move(finalMovement * Time.deltaTime);

        HandleStamina(movement, IsBoosting);

        HandleStateChanges();

        UpdateCharacterController();

        UpdateVisual();
    }

    // Updates the visual representation of the player based on the current state (standing, crouching, prone).
    private void HandleRotation(Vector3 movement)
    {
        if (movement.magnitude <= 0.1f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(movement);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if (!characterController.isGrounded)
            return;

        if (playerState != PlayerState.Standing)
            return;

        if (!inputActions.Player.Jump.WasPressedThisFrame())
            return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // Get the current speed based on the player's state and input
    private Vector3 GetMovement()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * moveInput.y + right * moveInput.x;
    }

    // Returns the current speed based on the player's state and input
    private float GetCurrentSpeed(bool isSprinting, bool isWalkingSlow)
    {
        switch (playerState)
        {
            case PlayerState.Standing:

                if (isSprinting)
                    return sprintSpeed;

                if (isWalkingSlow)
                    return slowWalkSpeed;

                return walkSpeed;


            case PlayerState.Crouching:

                if (isSprinting)
                    return crouchSprintSpeed;

                if (isWalkingSlow)
                    return slowCrouchSpeed;

                return crouchMoveSpeed;


            case PlayerState.Prone:

                if (isSprinting)
                    return proneSprintSpeed;

                if (isWalkingSlow)
                    return slowProneSpeed;

                return proneMoveSpeed;


            default:
                return walkSpeed;
        }
    }

    // Handle stamina drain and regeneration based on movement and sprinting
    private void HandleStamina(Vector3 movement, bool isSprinting)
    {
        if (movement.magnitude <= 0.1f)
        {
            stamina.Regenerate();
            return;
        }


        if (!isSprinting)
        {
            stamina.Regenerate();
            return;
        }


        switch (playerState)
        {
            case PlayerState.Standing:
                stamina.Drain(stamina.SprintDrain);
                break;


            case PlayerState.Crouching:
                stamina.Drain(stamina.CrouchDrain);
                break;


            case PlayerState.Prone:
                stamina.Drain(stamina.ProneDrain);
                break;
        }
    }

    // Handle state changes based on input and current state
    private void HandleStateChanges()
    {
        if (!characterController.isGrounded)
            return;

        if (inputActions.Player.Crouch.WasPressedThisFrame())
        {
            switch (playerState)
            {
                case PlayerState.Standing:
                    playerState = PlayerState.Crouching;
                    break;

                case PlayerState.Crouching:
                    playerState = PlayerState.Standing;
                    break;

                case PlayerState.Prone:
                    playerState = PlayerState.Crouching;
                    break;
            }
        }

        if (inputActions.Player.Prone.WasPressedThisFrame())
        {
            switch (playerState)
            {
                case PlayerState.Standing:
                    playerState = PlayerState.Prone;
                    break;

                case PlayerState.Crouching:
                    playerState = PlayerState.Prone;
                    break;

                case PlayerState.Prone:
                    playerState = PlayerState.Standing;
                    break;
            }
        }
    }

    // Handle the player's death state
    private void UpdateCharacterController()
    {
        float targetHeight;

        switch (playerState)
        {
            case PlayerState.Prone:
                targetHeight = proneHeight;
                break;

            case PlayerState.Crouching:
                targetHeight = crouchHeight;
                break;

            default:
                targetHeight = standingHeight;
                break;
        }

        float targetCenter;

        switch (playerState)
        {
            case PlayerState.Prone:
                targetCenter = proneCenter;
                break;

            case PlayerState.Crouching:
                targetCenter = crouchCenter;
                break;

            default:
                targetCenter = standingCenter;
                break;
        }

        characterController.height = Mathf.Lerp(
            characterController.height,
            targetHeight,
            crouchSpeed * Time.deltaTime
        );

        Vector3 currentCenter = characterController.center;

        currentCenter.y = Mathf.Lerp(
            currentCenter.y,
            targetCenter,
            crouchSpeed * Time.deltaTime
        );

        characterController.center = currentCenter;
    }

    // Update the visual representation of the player based on the current state
    private void UpdateVisual()
    {
        if (visualMeshTransform == null)
            return;

        float targetScaleY = characterController.height / standingHeight;

        Vector3 currentScale = visualMeshTransform.localScale;

        currentScale.y = Mathf.Lerp(
            currentScale.y,
            targetScaleY,
            crouchSpeed * Time.deltaTime
        );

        visualMeshTransform.localScale = currentScale;

        float targetCenter;

        switch (playerState)
        {
            case PlayerState.Prone:
                targetCenter = proneCenter;
                break;

            case PlayerState.Crouching:
                targetCenter = crouchCenter;
                break;

            default:
                targetCenter = standingCenter;
                break;
        }

        Vector3 currentMeshPos = visualMeshTransform.localPosition;

        currentMeshPos.y = Mathf.Lerp(
            currentMeshPos.y,
            targetCenter,
            crouchSpeed * Time.deltaTime
        );

        visualMeshTransform.localPosition = currentMeshPos;
    }
}
