using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaUI : MonoBehaviour
{
    public StaminaSystem staminaSystem;
    public Slider staminaSlider;
    public TextMeshProUGUI staminaText;
    
    void Start()
    {
        if (staminaSystem == null)
            staminaSystem = FindObjectOfType<StaminaSystem>();

        if (staminaSystem != null)
        {
            staminaSystem.OnStaminaChanged += OnStaminaChanged;
            UpdateUI(staminaSystem.currentStamina);
        }
    }

    void OnDisable()
    {
        if (staminaSystem != null)
            staminaSystem.OnStaminaChanged -= OnStaminaChanged;
    }

    void OnStaminaChanged(float currentStamina)
    {
        UpdateUI(currentStamina);
    }

    void UpdateUI(float currentStamina)
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = staminaSystem != null ? staminaSystem.maxStamina : staminaSlider.maxValue;
            staminaSlider.value = currentStamina;
        }

        if (staminaText != null && staminaSystem != null)
        {
            staminaText.text = $"Стамина: {currentStamina:F0}/{staminaSystem.maxStamina}";
        }
    }
}