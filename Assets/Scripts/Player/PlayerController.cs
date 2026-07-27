using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private InputSystem_Actions inputActions;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;

    private Vector3 velocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
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

    private void Update()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 movement = new Vector3(
            moveInput.x,
            0,
            moveInput.y
        );

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMovement = movement * moveSpeed + velocity;

        characterController.Move(finalMovement * Time.deltaTime);
    }
}