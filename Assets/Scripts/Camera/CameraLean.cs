using UnityEngine;

public class CameraLean : MonoBehaviour
{
    // Скорость, с которой камера наклоняется
    [SerializeField] private float leanSpeed = 8f;

    // Максимальный угол наклона камеры
    [SerializeField] private float maxLeanAngle = 15f;

    private InputSystem_Actions inputActions;

    // Текущий угол наклона
    private float currentLean;

    // Базовый поворот камеры до наклона
    private Quaternion baseRotation;

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
        // Запоминаем нормальный поворот камеры
        baseRotation = transform.localRotation;
    }

    private void Update()
    {
        // Получаем ввод Q/E
        float leanInput = 0f;

        if (inputActions.Player.LeanLeft.IsPressed())
        {
            leanInput = 1f;
        }

        if (inputActions.Player.LeanRight.IsPressed())
        {
            leanInput = -1f;
        }

        // Целевой угол наклона
        float targetLean = leanInput * maxLeanAngle;

        // Плавное изменение угла
        currentLean = Mathf.Lerp(
            currentLean,
            targetLean,
            leanSpeed * Time.deltaTime
        );

        // Применяем наклон по Z
        transform.localRotation =
            baseRotation *
            Quaternion.Euler(
                0,
                0,
                currentLean
            );
    }
}