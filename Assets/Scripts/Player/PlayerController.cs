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
    [SerializeField] private Transform visualMeshTransform;

    private PlayerState playerState = PlayerState.Standing;
    private Vector3 velocity;
    private Stamina stamina;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();

        stamina = GetComponent<Stamina>();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        bool isSprinting = inputActions.Player.Sprint.IsPressed() && stamina.CanSprint();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();

        Vector3 movement = forward * moveInput.y + right * moveInput.x;

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        if (characterController.isGrounded &&
            playerState == PlayerState.Standing &&
            inputActions.Player.Jump.WasPressedThisFrame())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        float currentSpeed;

        switch (playerState)
        {
            case PlayerState.Standing:
                currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
                break;

            case PlayerState.Crouching:
                currentSpeed = isSprinting ? crouchSprintSpeed : crouchMoveSpeed;
                break;

            case PlayerState.Prone:
                currentSpeed = isSprinting ? proneSprintSpeed : proneMoveSpeed;
                break;

            default:
                currentSpeed = walkSpeed;
                break;
        }

        Vector3 finalMovement = (movement * currentSpeed) + velocity;
        characterController.Move(finalMovement * Time.deltaTime);

        if (isSprinting && movement.magnitude > 0.1f)
        {
            stamina.Drain();
        }
        else
        {
            stamina.Regenerate();
        }

        if (characterController.isGrounded && inputActions.Player.Crouch.WasPressedThisFrame())
        {
            playerState = playerState == PlayerState.Crouching
            ? PlayerState.Standing
            : PlayerState.Crouching;
        }

        if (characterController.isGrounded &&
            inputActions.Player.Prone.WasPressedThisFrame())
        {
            playerState = playerState == PlayerState.Prone
            ? PlayerState.Standing
            : PlayerState.Prone;
        }

        float targetHeight;

        if (playerState == PlayerState.Prone)
        {
            targetHeight = proneHeight;
        }
        else if (playerState == PlayerState.Crouching)
        {
            targetHeight = crouchHeight;
        }
        else
        {
            targetHeight = standingHeight;
        }

        float targetCenter;

        if (playerState == PlayerState.Prone)
        {
            targetCenter = proneCenter;
        }
        else if (playerState == PlayerState.Crouching)
        {
            targetCenter = crouchCenter;
        }
        else
        {
            targetCenter = standingCenter;
        }

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchSpeed * Time.deltaTime);

        Vector3 currentCenter = characterController.center;
        currentCenter.y = Mathf.Lerp(currentCenter.y, targetCenter, crouchSpeed * Time.deltaTime);
        characterController.center = currentCenter;

        if (visualMeshTransform != null)
        {
            float targetScaleY = characterController.height / standingHeight;

            Vector3 currentScale = visualMeshTransform.localScale;
            currentScale.y = Mathf.Lerp(currentScale.y, targetScaleY, crouchSpeed * Time.deltaTime);
            visualMeshTransform.localScale = currentScale;

            Vector3 currentMeshPos = visualMeshTransform.localPosition;
            currentMeshPos.y = Mathf.Lerp(currentMeshPos.y, targetCenter, crouchSpeed * Time.deltaTime);
            visualMeshTransform.localPosition = currentMeshPos;
        }
    }
}
