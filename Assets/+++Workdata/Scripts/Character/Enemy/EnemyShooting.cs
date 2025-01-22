using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyShooting : CharacterBase
{
	#region Variables

	[SerializeField] private Transform bulletSpawnPoint = null;
	
	[SerializeField] private float distance;

	private Coroutine bulletSpawn;

	private Transform playerTransform = null;

	private bool inRange = false;

	[SerializeField] private bool canAttack;

	private Animator anim;

	[FormerlySerializedAs("enemySounds")] [SerializeField] private AudioSource enemyIdleSounds;
	[Space]
	
	[Header("Effect")]
	[SerializeField] private ParticleSystem deathEffect;
	
	#endregion

	#region Methods

	private void Awake()
	{
		//Gets these components.
		playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

		anim = GetComponent<Animator>();
	}

	/// <summary>
	/// Damages the player
	/// </summary>
	/// <param name="other"></param>
	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<CharacterBase>().healthPoints--;
		}
	}

	/// <summary>
	/// Calculates the distance between the player amd this gameObject and calls a shoot method when the distance is small enough. 
	/// </summary>
	private void Update()
	{
		Vector3 target = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
		transform.LookAt(target);
		
		if (healthPoints <= 0)
		{
			UIManager.Instance.timer.time += 5;
			GameController.Instance.ChangeScore(500);
			UIManager.Instance.inGameScoreText.text = "Score: " + GameController.Instance.currentPoints;
			enemyIdleSounds.PlayOneShot(MusicManager.instance.enemyDeathSound);
			deathEffect.Play();
			gameObject.SetActive(false);
		}

		var dist = Vector3.Distance(transform.position, playerTransform.position);
		
		if(dist < distance && !inRange && canAttack)
		{
			inRange = true;
			
			anim.SetBool("InRange", inRange);
			if (playerTransform.gameObject.GetComponent<PlayerMovement>().states == PlayerMovement.PlayerStates.Dash)
			{
				//Think about changing this (doesnt really work) 
				Physics.IgnoreCollision(transform.gameObject.GetComponent<Collider>(), playerTransform.gameObject.GetComponent<MeshCollider>());
			}
		}
		else if (dist > distance)
		{
			inRange = false;
			anim.SetBool("InRange", inRange);
		}
	}

	/// <summary>
	/// Sets the spawned bullets values and shoots it. 
	/// </summary>
	private void InstantiateBullet()
	{
		GameObject bullet = ObjectPool.sharedInstance.GetPooledObject();
		if (bullet != null)
		{
			bullet.transform.position = bulletSpawnPoint.position;
			bullet.transform.rotation = bulletSpawnPoint.rotation;
			bullet.SetActive(true);
			bullet.GetComponent<Bullet>().StartBullet();	
		}
	}
	
	private void SpawnBullet()
	{
		//Calls ´the bullet and plays sound effect. 
		InstantiateBullet();
		
		enemyIdleSounds.PlayOneShot(MusicManager.instance.enemyShootSound);
	}
	#endregion
}