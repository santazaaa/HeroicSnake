using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIPanelController : MonoBehaviour {

	public Text combatPointText;
	public Text bestText;
	public Text atkBonusText;

	public void SetCombatPoint(int point)
	{
		combatPointText.text = point.ToString ();
	}

	public void SetBestText(int point)
	{
		bestText.text = point.ToString ();
	}

	public void SetBonusATKText(int point)
	{
		atkBonusText.text = point.ToString () + "%";
	}

}
