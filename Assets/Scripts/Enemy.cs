using UnityEngine;
using System.Collections;

public class Enemy : CharacterBase {

	public HealthBar healthBar;

	public override void TakeDamage(int damage) {
		base.TakeDamage (damage);

		// Update health bar
		healthBar.SetHealth(status.hp, status.maxHp);
	} 

	#region implemented abstract members of CharacterBase

	protected override void OnHitWall ()
	{
		
	}

	#endregion
}
