using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FMODUnity;

public class PlayerHealthManager : MonoBehaviour
{
    [Header ("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float lerpSpeed = 5f;

    [Header ("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private StudioEventEmitter damageSoundEmitter;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.minValue = 0;
        slider.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value != currentHealth) slider.value = Mathf.Lerp (slider.value, currentHealth, Time.deltaTime * lerpSpeed);

        Die();
    }

    private void Die ()
    {
        if (slider.value <= 0 )
        {
           Destroy (gameObject);
        }
    }

    public void TakeDamage (float value)
    {
        currentHealth -= value;
        Debug.Log("bomba is triggered");

        if (damageSoundEmitter != null)
        {
            damageSoundEmitter.Play();
        }
    }

    public void Heal (float value)
    {
        currentHealth += value;
    }
}
