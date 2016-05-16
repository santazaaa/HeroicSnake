using UnityEngine;
using System.Collections;

public enum MoveDirection {
	None = 0, Left = 1, Up = 2, Right = 3, Down = 4
}

public enum CharacterType
{
	Red, Green, Blue
}

public class CharacterStatus {

	public int maxHp;
	public int hp;
	public int attack;
	public int defense;
	public CharacterType type;
	
}

public abstract class CharacterBase : MonoBehaviour {

	// Inspector attributes
	public SpriteRenderer characterSprite;
	protected float speed = 1f;

	// Attributes
	protected CharacterStatus status;

	protected MoveDirection _currentDirection;
	protected Vector3 _currentVelocity;


	// Set character status when be spawned
	public void SetStatus(int hp, int attack, int defense, CharacterType type)
	{
		status = new CharacterStatus ();
		status.maxHp = hp;
		status.hp = hp;
		status.attack = attack;
		status.defense = defense;
		status.type = type;

		// Set sprite color based on type
		switch (type) {
		case CharacterType.Red:
			characterSprite.color = new Color (1, 0.5f, 0.5f);
			break;
		case CharacterType.Green:
			characterSprite.color = new Color (0.5f, 1, 0.5f);
			break;
		case CharacterType.Blue:
			characterSprite.color = new Color (0.5f, 0.5f, 1);
			break;
		}

		Debug.Log ("Set " + GetType().ToString() + " status: HP = " + hp + ", ATK = " + attack + ", DEF = " + defense + ", Type = " + type.ToString ());
	}

	public void SetSprite(Sprite sprite)
	{
		characterSprite.sprite = sprite;
	}

	// Set move direction
	public virtual void SetDirection(MoveDirection newDir)
	{
		_currentDirection = newDir;
		switch (newDir) {
		case MoveDirection.None:
			_currentVelocity = Vector3.zero;
			break;
		case MoveDirection.Left:
			_currentVelocity = Vector3.left;
			break;
		case MoveDirection.Right:
			_currentVelocity = Vector3.right;
			break;
		case MoveDirection.Up:
			_currentVelocity = Vector3.up;
			break;
		case MoveDirection.Down:
			_currentVelocity = Vector3.down;
			break;
		}

		_currentVelocity *= speed;
	}
		
	public virtual void Move()
	{
		// Check walls
		if (IsHitWall ()) {
			Debug.Log ("Hit wall!");
			OnHitWall ();
		} else { // If not hit wall, move to next position based on its velocity
			transform.Translate (_currentVelocity);
		}
	}

	public MoveDirection GetDirection()
	{
		return _currentDirection;
	}

	public MoveDirection GetOppositeDiretion() // Get opposite direction from current direction
	{
		if (_currentDirection == MoveDirection.None) {
			return MoveDirection.None;
		} else {
			int opDirIdx = (int)_currentDirection + 2;
			if (opDirIdx > 4)
				opDirIdx -= 4;
			return (MoveDirection)opDirIdx;
		}
	}

	public void SetPosition(Vector3 position) // A wrapper of transform.position
	{
		// Remove from old grid
		GameManager.Instance.FreeGrid(transform.position);

		// Add to new grid
		GameManager.Instance.AddToGrid(position);

		transform.position = position;
	}

	public Vector3 GetPosition(){
		return transform.position;
	}

	protected abstract void OnHitWall();

	protected bool IsHitWall()
	{
		if(_currentDirection != MoveDirection.None)
		{
			
			Vector2 dir = GetNextDirection ();

			RaycastHit2D hitInfo = Physics2D.Raycast (GetNextPosition(dir), dir, 0.1f, 1 << LayerMask.NameToLayer("Wall"));
			if(hitInfo.collider != null) {
				if (hitInfo.collider.CompareTag ("Wall")) {
					// Hit wall!
					return true;
				}
			}
		}
		return false;
	}

	protected Vector2 GetNextPosition() {
		return GetNextPosition (GetNextDirection ());
	}

	protected Vector2 GetNextPosition(Vector2 dir) {
		return new Vector2 (transform.position.x, transform.position.y) + dir;
	}

	protected Vector2 GetNextDirection() {
		Vector2 dir = Vector2.zero;
		switch (_currentDirection) {
		case MoveDirection.Left:
			dir = Vector2.left;
			break;
		case MoveDirection.Right:
			dir = Vector2.right;
			break;
		case MoveDirection.Up:
			dir = Vector2.up;
			break;
		case MoveDirection.Down:
			dir = Vector2.down;
			break;
		}
		return dir;
	}

	public CharacterStatus GetStatus()
	{
		return status;
	}

	// Call this when charater want to attack another character with attack amplifier (default is 1.0f)
	public void Attack(CharacterBase other, float amplifier)
	{
		CharacterStatus status = this.GetStatus ();
		int attack = (int)(status.attack * amplifier);
		if (this is Hero && other is Enemy && status.type == other.GetStatus ().type) {
			// Double its attack
			attack *= 2;
		}

		// Random damage in range
		attack = Random.Range((int)(attack * 0.8f), attack);

		int damage = attack - other.GetStatus().defense;

		if (damage > 0) { // Do damage to enemy
			other.TakeDamage (damage);
		} else { // Player cannot do damage to enemy!
			Debug.Log("Cannot do damage");
		}

		// Show attack effect
		Color slashColor = Color.cyan;
		if (this is Enemy)
			slashColor = Color.red;
		
		EffectManager.Instance.CreateSlashEffect(other.GetPosition(), slashColor);
		SoundManager.Instance.PlayOneShot (SoundManager.Instance.hitSFX);

	}

	// Call this when character takes damage from attacks
	public virtual void TakeDamage(int damage)
	{
		Color dmgColor = Color.red;
		if (this is Enemy)
			dmgColor = Color.white;
		

		// Create effect
		EffectManager.Instance.CreateDamageEffect(this.GetPosition(), dmgColor, damage.ToString());

		status.hp -= damage;
		if (status.hp < 0) {
			status.hp = 0;
		}
		Debug.Log (GetType ().ToString () + ": Take damage " + damage + " , " + status.hp + " left");
	}

	public virtual bool IsDead()
	{
		return status.hp <= 0;
	}

}
