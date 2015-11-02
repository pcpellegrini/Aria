using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Info gameInfo;
	public SoundManager soundManager;
    public GameObject mainPanel;
    public GameObject selectionPanel;
	public GameObject optionsPanel;
    public GameObject creditsPanel;

    private GameObject _currentPanel;
	private GameObject _previousPanel;

    void Start () {
		_currentPanel = mainPanel;
    }

	public void ChangeMenu(string p_menu)
	{
		switch (p_menu)
		{
		case "main":
			_currentPanel.SetActive(false);
			_previousPanel.SetActive(true);
			_currentPanel = _previousPanel;
			_previousPanel = null;
			break;
		case "selection":
			_previousPanel = mainPanel;
			_currentPanel = selectionPanel;
			mainPanel.SetActive(false);
			selectionPanel.SetActive(true);
			break;
		case "options":
            _previousPanel = mainPanel;
			_currentPanel = optionsPanel;
			mainPanel.SetActive(false);
			optionsPanel.SetActive(true);
			break;
		case "credits":
            _previousPanel = mainPanel;
			_currentPanel = creditsPanel;
			mainPanel.SetActive(false);
			creditsPanel.SetActive(true);
			break;
		case "quit":
			Application.Quit();
			break;
		case "startGame":
			Application.LoadLevel("01");
			break;
		}
	}

	public void SelectAircraft(int p_type)
	{
		gameInfo.selectedAircraft = (AirCraft.type)p_type;
	}

	public void SaveOptions()
	{
		soundManager.SaveOptions ();
	}
}
