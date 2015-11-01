using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Info gameInfo;
    public GameObject mainPanel;
    public GameObject selectionPanel;
    public Button playBtn;
    public Button exitBtn;
    public Button backBtn;
    public Button startBtn;
    public Button[] aircraftBtn;

    void Start () {
        // Main Menu
        playBtn.onClick.AddListener(delegate
        {
            mainPanel.SetActive(false);
            selectionPanel.SetActive(true);
        });
        exitBtn.onClick.AddListener(delegate
        {
            Application.Quit();
        });
        // Selection Menu
        backBtn.onClick.AddListener(delegate
        {
            for (int j = 0; j < aircraftBtn.Length; j++)
            {
                aircraftBtn[j].image.color = Color.white;
            }
            mainPanel.SetActive(true);
            selectionPanel.SetActive(false);
        });

        for (int i = 0; i < aircraftBtn.Length; i++)
        {
            int __num = i;
            aircraftBtn[__num].onClick.AddListener(delegate
            {
                for (int j = 0; j < aircraftBtn.Length; j++)
                {
                    aircraftBtn[j].image.color = Color.white;
                }
                aircraftBtn[__num].image.color = Color.red;
                gameInfo.selectedAircraft = (AirCraft.type)__num;
            });
        }
        startBtn.onClick.AddListener(delegate
        {
            Application.LoadLevel("01");
        });
    }
}
