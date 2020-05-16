using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public bool isBillboard;
    public Transform cam;
    public Text agentNameText;
    public Color agentNameTextColor;
    public Text levelText;
    public Color levelTextColor;

    void Start()
    {
        /*
        if (agentNameText != null)
            agentNameText.color = agentNameTextColor;

        if (levelText != null)
            levelText.color = levelTextColor;
            */
    }
    void LateUpdate()
    {
        if (isBillboard)
            transform.LookAt(transform.position + cam.forward);
    }
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetAgentName(string name)
    {
        if(agentNameText != null)
        agentNameText.text = name;
    }

    public void SetLevel(string level)
    {
        if(levelText != null)
            levelText.text = level;
    }
}
