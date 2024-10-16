using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[SerializeField]
	private GameObject bullet = null;
	
	private GameObject player = null;

	[SerializeField] 
	private Transform bulletSpawnPoint = null;

	[SerializeField]
	private float distanceBetweenPLayerAndEnemy = 0f;

	private void Update()
	{
		
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerAttack"))
		{
			gameObject.SetActive(false);
		}
	}

	private void InstantiateBullet()
	{
		Instantiate(bullet, bulletSpawnPoint);
	}
}