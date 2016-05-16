using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	// Singleton
	public static InputController Instance { get { return _instance; } }
	private static InputController _instance;

	private Player player;

	void Awake()
	{
		_instance = this;
	}

	public void SetPlayer(Player player)
	{
		this.player = player;
	}
	
	// Update is called once per frame
	void Update () {
		switch (GameManager.Instance.GetCurrentState ()) {
		case GameState.PreGame:
			if (player != null) {
				CheckPressArrow ();
			}
			break;
		case GameState.Playing:
			if (player != null) {
				CheckPlayerInput ();
			}
			break;

		case GameState.Lose:
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.Quit ();
			} else if (Input.GetKeyDown(KeyCode.R)) {
				SoundManager.Instance.PlayOneShot (SoundManager.Instance.buttonClickSFX);
				GameManager.Instance.Restart ();
			}
			break;
		}

	}

	void CheckPressArrow()
	{
		bool isPressed = false;

		// Check move
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			Debug.Log ("Player: LEFT");
			player.SetDirection (MoveDirection.Left);
			isPressed = true;
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			Debug.Log ("Player: RIGHT");
			player.SetDirection (MoveDirection.Right);
			isPressed = true;
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Debug.Log ("Player: UP");
			player.SetDirection (MoveDirection.Up);
			isPressed = true;
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			Debug.Log ("Player: DOWN");
			player.SetDirection (MoveDirection.Down);
			isPressed = true;
		}
		if (isPressed) {
			SoundManager.Instance.PlayOneShot (SoundManager.Instance.buttonClickSFX);
			GameManager.Instance.StartGame ();
		}
	}

	void CheckPlayerInput()
	{
		// Check move
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			Debug.Log ("Player: LEFT");
			player.SetDirection (MoveDirection.Left);
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			Debug.Log ("Player: RIGHT");
			player.SetDirection (MoveDirection.Right);
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Debug.Log ("Player: UP");
			player.SetDirection (MoveDirection.Up);
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			Debug.Log ("Player: DOWN");
			player.SetDirection (MoveDirection.Down);
		}

		// Check rotate
		if (Input.GetKeyDown (KeyCode.Z)) {
			player.RotateLeftHero ();
		} else if (Input.GetKeyDown (KeyCode.X)) {
			player.RotateRightHero ();
		}
	}

}
