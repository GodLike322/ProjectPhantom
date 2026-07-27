using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 2f;

    [SerializeField] private float mouseSensitivity = 3f;

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
}