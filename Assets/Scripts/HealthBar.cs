using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public Image healthBar;

	private float maxWidth;

	public void SetHealth(float hp, float maxHP)
	{
		float ratio = hp / maxHP;
		healthBar.fillAmount = ratio;
	}

}
