using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState {
	Init,
	PreGame,
	Playing,
	Paused,
	Win,
	Lose
}

public class GameManager : MonoBehaviour {

	// Singleton
	public static GameManager Instance { get { return _instance; } }
	private static GameManager _instance;

	// Settings
	public float spawnTime = 2.0f;

	public int minHp = 50;
	public int maxHp = 100;
	public int minAttack = 20;
	public int maxAttack = 50;
	public int minDefense = 0;
	public int maxDefense = 30;

	// Walls
	public Transform leftWall;
	public Transform rightWall;
	public Transform topWall;
	public Transform bottomWall;

	// Prefabs
	public GameObject playerPrefab;
	public GameObject heroPrefab;
	public GameObject enemyPrefab;

	// Hero sprites
	public Sprite[] heroSprites;

	// Enemy sprites
	public Sprite[] enemySprites;

	// Attributes
	private List<Hero> heros;
	private List<Enemy> enemies;
	private int minX, maxX, minY, maxY, width, height;
	private Player player;
	private GameState currentState;
	private int combatPoint;
	private Grid[,] grids;
	private List<Grid> freeGrids;

	void Awake() {
		_instance = this;
		heros = new List<Hero> ();
		enemies = new List<Enemy> ();
		currentState = GameState.Init;
	}

	// Use this for initialization
	void Start () {
		// Get boundary from walls
		minX = (int)leftWall.position.x + 1;
		maxX = (int)rightWall.position.x - 1;
		minY = (int)bottomWall.position.y + 1;
		maxY = (int)topWall.position.y - 1;

		// Get width and height
		width = maxX - minX + 1;
		height = maxY - minY + 1;

		// Create grid array
		grids = new Grid[width, height];

		// Create grids to check if there is an object on it?
		for (int i = 0; i < width ; i++) {
			for (int j = 0; j < height ; j++) {
				grids[i, j] = new Grid (i + minX, j + minY);
			}
		}

		// Load best score from save
		SaveGame.LoadScore ();

		// Create player
		InitPlayer ();
	}

	void InitPlayer()
	{
		// Create first hero object to be used as a player's head hero
		Hero firstHero = SpawnHero ();
		firstHero.SetStatus (Random.Range (minHp, maxHp + 1), Random.Range (minAttack, maxAttack + 1), Random.Range (minDefense, maxDefense + 1), (CharacterType)Random.Range (0, 3));

		// Create player object
		player = (GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Player>();
		player.Init (firstHero);

		// Set input controller player
		InputController.Instance.SetPlayer(player);

		SetState (GameState.PreGame);
	}

	// Call this method to start the game
	public void StartGame()
	{
		Debug.Log ("Start game!");

		// Reset score
		combatPoint = 0;

		// Start spawning objects
		StartCoroutine (SpawnObject ());

		SetState (GameState.Playing);

		// Show game ui
		UIManager.Instance.ShowGamePanel ();

		// Setup uis
		UIManager.Instance.gamePanel.SetBestText (SaveGame.GetBestScore());
		UIManager.Instance.gamePanel.SetCombatPoint (combatPoint);
	}

	// Update is called once per frame
	void Update () {
		
		switch (currentState) {
		case GameState.Playing:
			
			// Check if player is dead
			if (player != null && player.IsDead ()) {
				Debug.Log ("Total combat point: " + combatPoint);
				Debug.Log ("Press anykey to restart");

				// Save score
				SaveGame.SaveScore(combatPoint);

				StopAllCoroutines (); // Stop spawning
				SetState (GameState.Lose);

				// Setup result panel ui (scores)
				UIManager.Instance.resultPanel.SetCombatPoint (combatPoint);
				UIManager.Instance.resultPanel.SetBestText (SaveGame.GetBestScore());

				UIManager.Instance.ShowResultPanel ();
			}
			break;
		}
	}



	IEnumerator SpawnObject()
	{
		while (true) {
			yield return new WaitForSeconds (2);

			// Random spawn a hero or an enemy every 2 secs
			float rand = Random.Range (0.0f, 1.0f);
			if (rand < 0.5f) { // Spawn enemy
				SpawnEnemy ();
			} else { // Spawn hero
				SpawnHero ();
			}
		}
	}

	Enemy SpawnEnemy()
	{
		Grid freeGrid = RandomFreeGrid ();

		if (freeGrid == null) {
			// No space to create enemy
			return null;
		}

		freeGrid.objectNumberOnGrid++;

		GameObject newObj = Instantiate(enemyPrefab, new Vector2(freeGrid.x, freeGrid.y), Quaternion.identity) as GameObject;
		Enemy newEnemy = newObj.GetComponent<Enemy> ();

		// Random hero status
		newEnemy.SetStatus(Random.Range(minHp, maxHp + 1), Random.Range(minAttack, maxAttack + 1), Random.Range(minDefense, maxDefense + 1), (CharacterType)Random.Range(0,3));

		if (heroSprites.Length > 0) {
			// Random enemy sprite
			newEnemy.SetSprite(enemySprites[Random.Range(0, heroSprites.Length)]);
		}

		// Add to list
		enemies.Add (newEnemy);

		return newEnemy;
	}

	Hero SpawnHero()
	{
		Grid freeGrid = RandomFreeGrid ();

		if (freeGrid == null) {
			// No space to create hero
			return null;
		}

		freeGrid.objectNumberOnGrid++;

		GameObject newObj = Instantiate(heroPrefab, new Vector2(freeGrid.x, freeGrid.y), Quaternion.identity) as GameObject;
		Hero newHero = newObj.GetComponent<Hero> ();

		if (heroSprites.Length > 0) {
			// Random hero sprite
			newHero.SetSprite(heroSprites[Random.Range(0, heroSprites.Length)]);
		}

		// Random hero status
		newHero.SetStatus(Random.Range(minHp, maxHp + 1), Random.Range(minAttack, maxAttack + 1), Random.Range(minDefense, maxDefense + 1), (CharacterType)Random.Range(0,3));

		// Add to list
		heros.Add (newHero);

		return newHero;
	}

	public void AddCombatPoint(int point)
	{
		Debug.Log ("Grant combat point: " + point);
		combatPoint += point;

		// Update ui
		UIManager.Instance.gamePanel.SetCombatPoint(combatPoint);
	}

	public void Restart()
	{
		Debug.Log ("Restart game");

		SetState (GameState.Init);

		// Clear all objects
		Destroy(player.gameObject);

		foreach (var hero in heros) {
			if (hero != null) 
				Destroy (hero.gameObject);
		}
		heros.Clear ();

		foreach (var enemy in enemies) {
			if(enemy != null)
				Destroy (enemy.gameObject);
		}
		enemies.Clear ();

		ResetGrids ();

		// Create new player
		InitPlayer();


		// Setup uis
		combatPoint = 0;
		UIManager.Instance.gamePanel.SetCombatPoint(combatPoint);
		UIManager.Instance.gamePanel.SetBestText (SaveGame.GetBestScore ());
		UIManager.Instance.gamePanel.SetBonusATKText (0);

		UIManager.Instance.ShowGamePanel ();
	}

	public Player GetPlayer()
	{
		return player;
	}


	public void SetState(GameState nextState)
	{
		Debug.Log ("GameManager: Change state from " + currentState.ToString () + " to " + nextState.ToString ());
		currentState = nextState;
	}

	public GameState GetCurrentState()
	{
		return currentState;
	}

	public void RemoveHero(Hero hero)
	{
		heros.Remove (hero);
	}

	public void RemoveEnemy(Enemy enemy)
	{
		enemies.Remove (enemy);
	}



	// Grids

	public Grid GetGrid(int x, int y)
	{
		return grids [x - minX, y - minY];
	}

	public void ResetGrids()
	{
		for (int i = 0; i < width ; i++) {
			for (int j = 0; j < height ; j++) {
				grids [i, j].objectNumberOnGrid = 0;
			}
		}
	}

	public List<Grid> GetFreeGrids()
	{
		List<Grid> result = new List<Grid> ();

		for (int i = 0; i < width ; i++) {
			for (int j = 0; j < height ; j++) {
				Grid grid = grids [i, j];
				if (!grid.isOccupied) {
					result.Add (grid);
				}
			}
		}

		Debug.Log ("Free grids = " + result.Count);

		return result;
	}

	public void AddToGrid(int x, int y)
	{
		GetGrid (x, y).objectNumberOnGrid++;
	}

	public void AddToGrid(Vector3 position)
	{
		AddToGrid ((int)position.x, (int)position.y);
	}

	public void FreeGrid(int x, int y)
	{
		GetGrid (x, y).objectNumberOnGrid--;
	}

	public void FreeGrid(Vector3 position)
	{
		FreeGrid ((int)position.x, (int)position.y);
	}

	public Grid RandomFreeGrid()
	{
		List<Grid> freeGrids = GetFreeGrids ();
		if (freeGrids.Count > 0) {
			return freeGrids[Random.Range(0, freeGrids.Count)];
		}
		return null;
	}

}
