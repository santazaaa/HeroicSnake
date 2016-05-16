using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource bgmSource;
	public AudioSource sfxSource;

	// SFX
	public AudioClip hitSFX;
	public AudioClip dieSFX;
	public AudioClip addSFX;
	public AudioClip rotateSFX;
	public AudioClip buttonClickSFX;

	// Singleton
	public static SoundManager Instance { get { return _instance; } }
	private static SoundManager _instance;

	void Awake() {
		_instance = this;
	}

	public void PlayBGM() {
		// No need for now...
	}

	public void StopBGM() {
		// No need for now...
	}


	// Play sound effect one shot
	public void PlayOneShot(AudioClip clip)
	{
		sfxSource.PlayOneShot (clip);
	}


}
