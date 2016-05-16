using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour {

	// Singleton
	public static EffectManager Instance { get { return _instance; } }
	private static EffectManager _instance;

	// Prefabs
	public GameObject slashEffectPrefab;
	public GameObject damagePrefab;

	// Slash sprite
	public Sprite[] slashSprites;

	void Awake() {
		_instance = this;
	}

	public void CreateSlashEffect(Vector3 position, Color color) {
		GameObject slashObj = Instantiate (slashEffectPrefab, position, Quaternion.identity) as GameObject;
		Effect slashEffect = slashObj.GetComponent<Effect>();
		slashEffect.SetSprite (slashSprites[Random.Range (0, slashSprites.Length)], color); // Random slash effect from set of sprites
	}

	public void CreateDamageEffect(Vector3 position, Color color, string damage) {
		GameObject damageObj = Instantiate (damagePrefab, position, Quaternion.identity) as GameObject;
		Damage damageEffect = damageObj.GetComponent<Damage>();
		damageEffect.SetDamage (damage, color);
	}

}
