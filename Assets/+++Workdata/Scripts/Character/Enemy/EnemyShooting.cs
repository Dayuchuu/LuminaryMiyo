using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyShooting : CharacterBase
{
	#region Variables

	[SerializeField] 
	private Transform bulletSpawnPoint = null;

	[SerializeField] private float bulletSpawnCooldown = 0f;
	
	[SerializeField] private float distance;

	private Coroutine bulletSpawn;

	private Transform playerTransform = null;

	private bool inRange = false;

	[FormerlySerializedAs("enemySounds")] [SerializeField] private AudioSource enemyIdleSounds;
	[SerializeField] private AudioSource enemyDieSounds;
	
	#endregion

	#region Methods

	private void Awake()
	{
		playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
	}

	private void Update()
	{
		transform.LookAt(playerTransform);

		if (healthPoints <= 0)
		{
			gameObject.SetActive(false);
		}
		
		if(Vector3.Distance(transform.position, playerTransform.position) < distance && !inRange)
		{
			inRange = true;
			if (playerTransform.gameObject.GetComponent<PlayerMovement>().states == PlayerMovement.PlayerStates.Dash)
			{
				Physics.IgnoreCollision(transform.gameObject.GetComponent<Collider>(), playerTransform.gameObject.GetComponent<CapsuleCollider>());
			}
			StartCoroutine(SpawnBullet());
		}
		else if (Vector3.Distance(transform.position, playerTransform.position) > distance && inRange)
		{
			inRange = false;
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

	private IEnumerator SpawnBullet()
	{
		if (inRange)
		{ 
			InstantiateBullet();
			
			yield return new WaitForSeconds(bulletSpawnCooldown);

			StartCoroutine(SpawnBullet());
		}

	}
	#endregion
}