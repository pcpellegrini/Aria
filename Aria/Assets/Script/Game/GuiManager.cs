using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour {

    public Slider energySlider;
    public Slider heatSlider;
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
        }
        menuBtn.onClick.AddListener(delegate
        {
            Time.timeScale = 1f;
            UnityEngine.Cursor.visible = true;
            Application.LoadLevel("Menu");
        });
    }
}
