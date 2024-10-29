using System;
using System.Collections;
using UnityEngine;

public class EnemyShooting : CharacterBase
{
	[SerializeField]
	private GameObject bullet = null;

	[SerializeField] 
	private Transform bulletSpawnPoint = null;

	[SerializeField] 
	private float bulletSpawnCooldown = 0f;

	private Coroutine bulletSpawn;
	
	public bool inRange = false;
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			inRange = true;

			StartCoroutine(SpawnBullet());
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			inRange = false;
			
			StopCoroutine(SpawnBullet());
		}
	}

	private void InstantiateBullet()
	{
		Instantiate(bullet, bulletSpawnPoint);
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
}