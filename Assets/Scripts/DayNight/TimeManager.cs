using System;
using Skaillz.EditInline;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour {
    [SerializeField] Light sun;
    [SerializeField] Light moon;
    [SerializeField] AnimationCurve lightIntensityCurve;
    [SerializeField] float maxSunIntensity = 1;
    [SerializeField] float maxMoonIntensity = 0.5f;
    
    [SerializeField] Color dayAmbientLight;
    [SerializeField] Color nightAmbientLight;
    [SerializeField] Material skyboxMaterial;
    
    ColorAdjustments colorAdjustments;
    
    [EditInline, SerializeField] TimeSettings timeSettings;
    
    public event Action OnSunrise {
        add => service.OnSunrise += value;
        remove => service.OnSunrise -= value;
    }
    
    public event Action OnSunset {
        add => service.OnSunset += value;
        remove => service.OnSunset -= value;
    }
    
    public event Action OnHourChange {
        add => service.OnHourChange += value;
        remove => service.OnHourChange -= value;
    }    

    TimeService service;

    void Start() {
        service = new TimeService(timeSettings);
    }

    void Update() {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        UpdateSkyBlend();
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            timeSettings.timeMultiplier *= 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            timeSettings.timeMultiplier /= 2;
        }
    }

    void UpdateSkyBlend() {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);
        float blend = Mathf.Lerp(0, 1, lightIntensityCurve.Evaluate(dotProduct));
        skyboxMaterial.SetFloat("_Blend", blend);
    }
    
    void UpdateLightSettings() {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        float lightIntensity = lightIntensityCurve.Evaluate(dotProduct);
        
        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensity);
        moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightIntensity);
        
        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensity);
    }

    void RotateSun() {
        float rotation = service.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
    }

    void UpdateTimeOfDay() {
        service.UpdateTime(Time.deltaTime);
    }
}