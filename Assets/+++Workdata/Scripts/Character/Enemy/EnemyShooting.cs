using System.Collections;
using UnityEngine;

public class EnemyShooting : CharacterBase
{
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
}