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

	private bool nextShotPossible = false;

	private float waitUntilNextShot = 0f;
	
	[SerializeField] float shootCooldown = 0f;
	
	#endregion

	#region Methods

	private void Awake()
	{
		waitUntilNextShot = shootCooldown;

		playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
	}

	private void Update()
	{
		if (!nextShotPossible)
		{
			waitUntilNextShot -= Time.deltaTime;

			if (waitUntilNextShot <= 0f)
			{
				nextShotPossible = true;
				waitUntilNextShot = shootCooldown;
			}
		}
		
		if(Vector3.Distance(transform.position, playerTransform.position) < distance  && nextShotPossible)
		{
			StartCoroutine(SpawnBullet());
		}
		else if (Vector3.Distance(transform.position, playerTransform.position) > distance)
		{
			StopCoroutine(SpawnBullet());
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
		InstantiateBullet();

		yield return new WaitForSeconds(bulletSpawnCooldown);

		StartCoroutine(SpawnBullet());
	}
	#endregion
}