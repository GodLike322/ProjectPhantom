using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private InputSystem_Actions inputActions;

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

        Debug.Log($"Move: {moveInput}");
    }
}