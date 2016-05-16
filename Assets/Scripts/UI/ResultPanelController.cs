using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultPanelController : MonoBehaviour {

	public Text combatPointText;
	public Text bestText;

	public void SetCombatPoint(int point)
	{
		combatPointText.text = point.ToString ();
	}

	public void SetBestText(int point)
	{
		bestText.text = point.ToString ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
