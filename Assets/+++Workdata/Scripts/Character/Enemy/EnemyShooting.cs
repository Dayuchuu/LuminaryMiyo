using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyShooting : CharacterBase
{
	#region Variables

	[SerializeField] private Transform bulletSpawnPoint = null;

	[SerializeField] private float bulletSpawnCooldown = 0f;
	
	[SerializeField] private float distance;

	private Coroutine bulletSpawn;

	private Transform playerTransform = null;

	private bool inRange = false;

	private Animator anim;

	[FormerlySerializedAs("enemySounds")] [SerializeField] private AudioSource enemyIdleSounds;
	[SerializeField] private AudioSource enemySounds;
	[Space]
	
	[Header("Effect")]
	[SerializeField] private ParticleSystem deathEffect;
	
	#endregion

	#region Methods

	private void Awake()
	{
		playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

		anim = GetComponent<Animator>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<CharacterBase>().healthPoints--;
		}
	}

	private void Update()
	{
		Vector3 target = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
		transform.LookAt(target);
		
		if (healthPoints <= 0)
		{
			UIManager.Instance.timer.time += 5;
			GameController.Instance.ChangeScore(500);
			UIManager.Instance.inGameScoreText.text = "Score: " + GameController.Instance.currentPoints;
			// enemySounds.PlayOneShot(MusicManager.instance.enemyDeathSound);
			deathEffect.Play();
			gameObject.SetActive(false);
		}

		var dist = Vector3.Distance(transform.position, playerTransform.position);
		
		if(dist < distance && !inRange)
		{
			inRange = true;
			
			anim.SetBool("InRange", inRange);
			if (playerTransform.gameObject.GetComponent<PlayerMovement>().states == PlayerMovement.PlayerStates.Dash)
			{
				//Think about changing this (doesnt really work) 
				Physics.IgnoreCollision(transform.gameObject.GetComponent<Collider>(), playerTransform.gameObject.GetComponent<CapsuleCollider>());
			}
		}
		else if (dist > distance)
		{
			inRange = false;
			anim.SetBool("InRange", inRange);
		}
	}

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
		
		InstantiateBullet();
		
		// enemySounds.PlayOneShot(MusicManager.instance.enemyShootSound);
		
	}
	#endregion
}