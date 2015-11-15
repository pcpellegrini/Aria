using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GuiManager : MonoBehaviour {

    public event Action goToMenu;

    public Slider energySlider;
    public Slider heatSlider;
    public Slider specialHeatSlider;
    public Slider armorSlider;
    public Slider monsterEnergy;
    public Button menuBtn;

    [HideInInspector]
    public bool inGame;

    public void ChangeValue(string p_slider, float p_value)
    {
        switch (p_slider)
        {
            case "energy":
                energySlider.value = p_value;
                break;
            case "heat":
                heatSlider.value = p_value;
                break;
            case "special":
                specialHeatSlider.value = p_value;
                break;
            case "armor":
                armorSlider.value = p_value;
                break;
            case "monsterEnergy":
                monsterEnergy.value = p_value;
                break;
        }
    }

    public void ChangeScene(string p_scene)
    {
        Time.timeScale = 1;
        if (goToMenu != null) goToMenu();
        Application.LoadLevel(p_scene);
    }
}
