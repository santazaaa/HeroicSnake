using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : CharacterBase {

	public HealthBar healthBar;

	#region implemented abstract members of CharacterBase
	protected override void OnHitWall ()
	{
		
	}
	#endregion

	public override void TakeDamage(int damage) {
		base.TakeDamage (damage);

		// Update health bar
		healthBar.SetHealth(status.hp, status.maxHp);
	} 

}
