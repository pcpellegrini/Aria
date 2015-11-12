using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour {

    public Slider energySlider;
    public Slider heatSlider;
    public Slider armorSlider;
    public Text specialAmmo;
    public Button menuBtn;

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
                specialAmmo.text = p_value.ToString();
                break;
            case "armor":
                armorSlider.value = p_value;
                break;
        }
        menuBtn.onClick.AddListener(delegate
        {
            Debug.Log("ok");
            Time.timeScale = 1;
            Application.LoadLevel(0);
        });
    }
}
