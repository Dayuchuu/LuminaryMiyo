using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShooting : CharacterBase
{
	#region MyRegion

	[SerializeField] 
	private Transform bulletSpawnPoint = null;

	[SerializeField] private float bulletSpawnCooldown = 0f;
	
	[SerializeField] private float distance;

	private Coroutine bulletSpawn;

	private Transform playerTransform = null;

	private bool inRange = false;
	
	#endregion

	#region Methods

	private void Awake()
	{
		playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
	}

	private void Update()
	{
		
		if(Vector3.Distance(transform.position, playerTransform.position) < distance && !inRange)
		{
			inRange = true;
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