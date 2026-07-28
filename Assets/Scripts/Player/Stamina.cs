using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 100f;

    [SerializeField] private float drainPerSecond = 20f;
    [SerializeField] private float regenPerSecond = 10f;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    public bool CanSprint()
    {
        return currentStamina > 0f;
    }

    public void Drain()
    {
        currentStamina -= drainPerSecond * Time.deltaTime;
        currentStamina = Mathf.Max(currentStamina, 0f);
    }

    public void Regenerate()
    {
        currentStamina += regenPerSecond * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    private void Update()
    {
        Debug.Log(currentStamina);
    }
}