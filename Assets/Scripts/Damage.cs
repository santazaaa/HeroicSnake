using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Damage : MonoBehaviour {

	public Text damageText;

	public void SetDamage(string text, Color color)
	{
		damageText.text = text;
		damageText.color = color;
	}

	// Called when animation ended
	public void SelfDestroy()
	{
		Destroy (gameObject);
	}

}
