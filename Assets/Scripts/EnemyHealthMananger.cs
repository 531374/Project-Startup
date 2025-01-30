using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FMODUnity;

public class EnemyHealthMananger : MonoBehaviour
{
    [Header ("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float currentHealth;
    [SerializeField] private float lerpSpeed = 5f;

    [Header ("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private StudioEventEmitter damageSoundEmitter;
    [SerializeField] private StudioEventEmitter deathSoundEmitter;

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

        Die ();
    }

    private void Die ()
    {
        if (slider.value <= 0 )
        {
            if (deathSoundEmitter != null)
            {
                deathSoundEmitter.Play();
            }
            Destroy (slider);
            Destroy (gameObject);


        }
    }
        
    

    public void TakeDamage (float value)
    {
        currentHealth -= value;

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
