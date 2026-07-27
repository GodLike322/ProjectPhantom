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
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float crouchMoveSpeed = 2f;
    [SerializeField] private Transform visualMeshTransform;

    private bool isCrouching;
    private Vector3 velocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

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

        float currentSpeed = isCrouching ? crouchMoveSpeed : walkSpeed;
        Vector3 finalMovement = (movement * currentSpeed) + velocity;
        characterController.Move(finalMovement * Time.deltaTime);

        if (inputActions.Player.Crouch.WasPressedThisFrame())
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        float targetCenter = isCrouching ? crouchCenter : standingCenter;

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
