using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenRate = 5f; // Stamina regenerated per second
    [SerializeField] private float regenDelay = 3f; // Delay before regeneration starts
    [SerializeField] private float lerpSpeed = 5f; // Speed of fill lerp
    [SerializeField] private float fadeSpeed = 0.8f;
    [HideInInspector] public float currentStamina;
    private float lastStaminaChangeTime;
    private float targetStamina;

    [Header("UI Settings")]
    [SerializeField] private Image staminaBar; // Stamina bar image

    [Header ("")]
    private Coroutine fadeCoroutine;

    private void Start()
    {
        currentStamina = maxStamina;
        targetStamina = maxStamina;
        UpdateStaminaBarInstantly();
    }

    private void Update()
    {
        // Smoothly lerp the fill amount
        float currentFill = staminaBar.fillAmount;
        float targetFill = targetStamina / maxStamina;
        staminaBar.fillAmount = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * lerpSpeed);

        // Regenerate stamina if delay has passed
        if (Time.time - lastStaminaChangeTime >= regenDelay && currentStamina < maxStamina)
        {
            RegenerateStamina();
        }

        // Start fading if fully regenerated
        if (Mathf.Approximately(currentStamina, maxStamina) && fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeOutStaminaBar());
        }
    }

    public void TakeStamina(float amount)
    {
        // Stop any ongoing fade-out
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // Reset alpha to full
        SetStaminaBarAlpha(1f);

        // Update stamina values
        targetStamina = Mathf.Clamp(targetStamina - amount, 0, maxStamina);
        currentStamina = targetStamina;

        // Reset the last stamina change time
        lastStaminaChangeTime = Time.time;
    }

    private void RegenerateStamina()
    {
        targetStamina = Mathf.Clamp(targetStamina + regenRate * Time.deltaTime, 0, maxStamina);
        currentStamina = targetStamina;
    }

    private void UpdateStaminaBarInstantly()
    {
        staminaBar.fillAmount = currentStamina / maxStamina;
    }

    private IEnumerator FadeOutStaminaBar()
    {
        // Wait a short time before fading out
        yield return new WaitForSeconds(1f);

        // Gradually fade out the alpha
        Color originalColor = staminaBar.color;
        while (staminaBar.color.a > 0)
        {
            float newAlpha = Mathf.Clamp01(staminaBar.color.a - Time.deltaTime * fadeSpeed); // Adjust fade speed here
            SetStaminaBarAlpha(newAlpha);
            yield return null;
        }

        // Ensure fully invisible
        SetStaminaBarAlpha(0f);
        fadeCoroutine = null;
    }

    private void SetStaminaBarAlpha(float alpha)
    {
        Color newColor = staminaBar.color;
        newColor.a = alpha;
        staminaBar.color = newColor;
    }
}
