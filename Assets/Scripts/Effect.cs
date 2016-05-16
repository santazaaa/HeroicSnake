using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {

	public SpriteRenderer spriteRenderer;

	public void SetSprite(Sprite sprite, Color color)
	{
		spriteRenderer.sprite = sprite;
		spriteRenderer.color = color;
	}

	// Called when animation ended
	public void SelfDestroy()
	{
		Destroy (gameObject);
	}
}
