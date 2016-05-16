using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	// Singleton
	public static UIManager Instance { get { return _instance; } }
	private static UIManager _instance;

	// Panels
	public GameObject introPanel;
	public GameUIPanelController gamePanel;
	public ResultPanelController resultPanel;

	void Awake()
	{
		_instance = this;
	}

	void Start()
	{
		ShowIntroPanel ();
	}

	public void ShowIntroPanel()
	{
		introPanel.SetActive (true);
		gamePanel.gameObject.SetActive (false);
		resultPanel.gameObject.SetActive (false);
	}

	public void ShowGamePanel()
	{
		introPanel.SetActive (false);
		gamePanel.gameObject.SetActive (true);
		resultPanel.gameObject.SetActive (false);
	}

	public void ShowResultPanel()
	{
		introPanel.SetActive (false);
		gamePanel.gameObject.SetActive (false);
		resultPanel.gameObject.SetActive (true);
	}


}
