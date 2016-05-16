using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : CharacterBase {

	public float timePerMove = 0.3f; // Wait time after each move in second
	public float minTimePerMove = 0.05f; // Min time per move (prevent too fast move after many enemies killed)

	private Hero head; // Head hero (the hero will fight with enemy)
	private List<Hero> tails; // Other heros in line

	private bool isCombating;
	private bool isDead;
	private bool canChangeDirection; // Used to prevent changing direction too frequently

	void Awake()
	{
		isCombating = false;
		tails = new List<Hero> ();
	}

	// Use this for initialization
	public void Init (Hero headHero) {
		isCombating = false;
		isDead = false;
		canChangeDirection = true;
		head = headHero;
		headHero.SetPosition (GetPosition ()); // Set head hero position to player position
		SetDirection (MoveDirection.None);
		StartCoroutine (MoveRoutine());
	}

	// Move to next position every x seconds
	IEnumerator MoveRoutine()
	{
		while (true) {
			Move ();
			yield return new WaitForSeconds (timePerMove);
		}
	}

	// Add move speed after killing enemy by reducing timePerMove
	public void IncreaseMoveSpeed()
	{
		timePerMove *= 0.95f;
		if (timePerMove < minTimePerMove) {
			timePerMove = minTimePerMove;
		}
	}

	// Move 1 slot
	public override void Move()
	{

		// Check walls
		if (IsHitWall ()) {
			Debug.Log ("Hit wall!");
			OnHitWall ();
		} else {
			CheckHitCharacter ();
		}

		canChangeDirection = true;

	}

	// Update each hero after each move or rotate hero in line
	void UpdateHeroPositions()
	{
		if (tails.Count > 0) {
			// Move each tails
			for(int i = tails.Count - 1; i >= 1; i--) {
				// Move from back to front
				Hero hero = tails[i];
				Hero frontHero = tails [i - 1]; // Hero in front of hero

				hero.SetPosition(frontHero.GetPosition());

			}

			// Move last one to last head position
			tails[0].SetPosition(head.GetPosition());
		}

		// Set head hero to current player position
		head.SetPosition(GetPosition());
	}

	bool CheckHitCharacter() { // Return true if hit, otherwise return false
		if(_currentDirection != MoveDirection.None)
		{
			Vector2 dir = GetNextDirection ();

			// Use 2D raycast to next position to check if player will hit something or not
			RaycastHit2D hitInfo = Physics2D.Raycast (GetNextPosition(dir), dir, 0.1f, 1 << LayerMask.NameToLayer("Character"));
			if(hitInfo.collider != null) {
				if (hitInfo.collider.CompareTag ("Hero")) {
					// Hit hero!
					Debug.Log ("Hit Hero: " + hitInfo.collider.gameObject.name);
					Hero hitHero = hitInfo.collider.gameObject.GetComponent<Hero> ();
					OnHitHero (hitHero);
				} else if (hitInfo.collider.CompareTag ("Enemy")) {
					// Hit enemy!
					Debug.Log ("Hit enemy!");
					Enemy hitEnemy = hitInfo.collider.gameObject.GetComponent<Enemy> ();
					OnHitEnemy (hitEnemy);
				} else if (hitInfo.collider.CompareTag ("Player")) {
					// Hit hero in line!
					Debug.Log("Hit player in line");

					// Destroy head hero
					RemoveHead();
				}
				return true;
			}
		}

		// Hit nothing, normal move player
		transform.Translate(_currentVelocity);

		// Update heroes position
		UpdateHeroPositions();

		return false;
	}

	void OnHitHero(Hero hitHero) {
		// Change tag to Player
		hitHero.gameObject.tag = "Player";

		// Add hero to tails
		AddHero(hitHero);

		// Update player position
		transform.Translate(_currentVelocity);

		// Update heroes position
		UpdateHeroPositions();
	}

	void OnHitEnemy(Enemy hitEnemy) {
		// Stop moving
		StopAllCoroutines();

		// Combat enemy
		StartCoroutine(CombatRoutine(hitEnemy));
	}

	IEnumerator CombatRoutine(Enemy enemy)
	{
		isCombating = true;
		Debug.Log ("Start combat with enemy");
		while (!isDead) {

			// Head hero attacks enemy
			head.Attack (enemy, GetBonusAttack());

			if (enemy.IsDead ()) {
				OnDefeatEnemy (enemy);

				// Enemy is dead, destroy enemy from scene
				SoundManager.Instance.PlayOneShot (SoundManager.Instance.dieSFX);
				GameManager.Instance.FreeGrid (enemy.transform.position);
				GameManager.Instance.RemoveEnemy (enemy);
				Destroy(enemy.gameObject);


				break;
			} else {
				
				// Wait 0.5 sec before next fight
				yield return new WaitForSeconds(0.5f);

				// Enemy is alive, enemy fights back!
				enemy.Attack(head, 1.0f);
			}
				
			if (head.IsDead ()) { // If head hero died, remove it from head and replace it with following hero
				RemoveHead ();
			}

			// Wait 0.5 sec before next fight
			yield return new WaitForSeconds(0.5f);

		}
		
		isCombating = false;

		if (!isDead) {
			// Continue moving
			StartCoroutine (MoveRoutine());
		}
	}

	public override void SetDirection(MoveDirection newDir) {
		if (isCombating) {
			Debug.Log ("Cannot change direction while fighting!");
			return;
		}

		// Check can change direction
		if (tails.Count > 0) {
			MoveDirection oppDir = GetOppositeDiretion();
			if (newDir == oppDir) {
				Debug.Log ("Cannot move to opposite direction");
				return;
			}
		}

		if (canChangeDirection) {
			base.SetDirection (newDir);
			canChangeDirection = false;
		} else {
			Debug.Log ("Wait until next move to change direction");
		}

	}
		
	void AddHero(Hero hero)
	{
		Debug.Log ("Add hero to tails");
		SoundManager.Instance.PlayOneShot (SoundManager.Instance.addSFX);
		tails.Add (hero);
		UIManager.Instance.gamePanel.SetBonusATKText (GetBonusAttackPercent ());
	}

	void RemoveHead()
	{
		SoundManager.Instance.PlayOneShot (SoundManager.Instance.dieSFX);
		GameManager.Instance.FreeGrid (head.transform.position);
		GameManager.Instance.RemoveHero (head);
		Destroy (head.gameObject);

		// Move next hero to head
		if (tails.Count > 0) {
			// There is hero available, move it to head
			head = tails [0];

			// Remove from tail
			tails.RemoveAt (0);

			// Update heroes position
			UpdateHeroPositions ();

			UIManager.Instance.gamePanel.SetBonusATKText (GetBonusAttackPercent ());

		} else {
			// No hero left, player lose and restart game
			isDead = true;
			Debug.Log ("Player is dead!");

			// Stop everything player doing
			StopAllCoroutines ();
		}
	}

	#region implemented abstract members of CharacterBase
	protected override void OnHitWall ()
	{
		// Remove head hero when hit wall
		RemoveHead();
	}
	#endregion

	public override bool IsDead()
	{
		return isDead;
	}

	// Bring head hero to back
	public void RotateLeftHero()
	{
		if (tails.Count > 0) {
			Vector3 lastHeroPos = tails [tails.Count - 1].GetPosition ();

			// Move all tails to next position
			UpdateHeroPositions ();

			// Move old head to back
			head.SetPosition(lastHeroPos);

			Hero oldHead = head;

			// Set first hero in tail as head
			head = tails[0];

			// Remove that hero from tail
			tails.RemoveAt(0);

			// Add old head to tail
			tails.Add(oldHead);

			// Play sound
			SoundManager.Instance.PlayOneShot(SoundManager.Instance.rotateSFX);

			Debug.Log ("Rotate to left");
		} else {
			Debug.Log ("Cannot rotate because no other hero left!");
		}
	}

	void OnDefeatEnemy(Enemy enemy)
	{
		// Increase move speed
		IncreaseMoveSpeed();

		// Grant points equal to enemy's hp
		GameManager.Instance.AddCombatPoint(enemy.GetStatus().maxHp);
	}

	// Bring back hero to head
	public void RotateRightHero()
	{
		if (tails.Count > 0) {
			Vector3 headPos = head.GetPosition ();


			// Move head to first tail position
			head.SetPosition (tails [0].GetPosition());

			// Move all tails back one step
			for(int i = 0; i < tails.Count - 1; i++) {
				Hero hero = tails[i];
				Hero nextHero = tails [i + 1];

				hero.SetPosition(nextHero.GetPosition());
			}

			Hero oldHead = head;

			// Set last hero in tail to head
			head = tails[tails.Count - 1];

			// Set head position
			head.SetPosition(headPos);

			// Remove last hero from tail
			tails.RemoveAt(tails.Count - 1);

			// Insert old head to front of tail
			tails.Insert(0, oldHead);

			// Play sound
			SoundManager.Instance.PlayOneShot(SoundManager.Instance.rotateSFX);

			Debug.Log ("Rotate to right");
		} else {
			Debug.Log ("Cannot rotate because no other hero left!");
		}
	}

	public float GetBonusAttack()
	{
		// Get bonus attack 5% per hero in tails
		return 1.0f + (0.05f * tails.Count);
	}

	public int GetBonusAttackPercent()
	{
		// Get bonus attack 5% per hero in tails
		return 5 * tails.Count;
	}

}
