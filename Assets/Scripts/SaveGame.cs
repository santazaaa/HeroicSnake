using UnityEngine;
using System.Collections;

public class SaveGame {

	public static readonly string SCORE_KEY = "BEST_SCORE";

	private static int bestScore;

	// Load best score from player prefs
	public static void LoadScore() {
		bestScore = PlayerPrefs.GetInt (SCORE_KEY, 0);
	}

	// Save player score if better than old one
	public static bool SaveScore(int score)
	{
		if (score > bestScore) {
			bestScore = score;
			PlayerPrefs.SetInt (SCORE_KEY, bestScore);
			PlayerPrefs.Save ();
			Debug.Log ("Save Game: score saved " + bestScore);
			return true; // Return true if saved
		}
		Debug.Log("Save Game: Old score is better, don't save");
		return false; // Otherwise return false
	}

	public static int GetBestScore()
	{
		return bestScore;
	}

}
