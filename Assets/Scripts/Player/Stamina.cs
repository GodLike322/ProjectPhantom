using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 100f;

    [SerializeField] private float sprintDrain = 20f;
    [SerializeField] private float crouchDrain = 10f;
    [SerializeField] private float proneDrain = 5f;
    [SerializeField] private float regenPerSecond = 10f;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public float SprintDrain => sprintDrain;
    public float CrouchDrain => crouchDrain;
    public float ProneDrain => proneDrain;

    private bool exhausted;

    public bool CanSprint()
    {
        return !exhausted && currentStamina > 0f;
    }

    public void Drain(float amount)
    {
        currentStamina -= amount * Time.deltaTime;

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            exhausted = true;
        }
    }

    public void Regenerate()
    {
        currentStamina += regenPerSecond * Time.deltaTime;

        currentStamina = Mathf.Min(
            currentStamina,
            maxStamina
        );

        if (currentStamina >= 20f)
        {
            exhausted = false;
        }
    }
}