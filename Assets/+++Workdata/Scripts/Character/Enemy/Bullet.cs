using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] 
	public float shootSpeed = 0f;
	
	private Transform playerTransform = null;

	private Rigidbody rb = null;
	
	Vector3 shootDirection = Vector3.zero;

	private void Awake()
	{
		rb = gameObject.GetComponent<Rigidbody>();
		
		playerTransform = GameObject.FindWithTag("Player").transform;

		shootDirection = playerTransform.position - transform.position;
		
		Shoot();
	}
	
	private void OnCollisionEnter(Collision other)
	{
		if (!other.gameObject.CompareTag("Player"))
		{
			Destroy(gameObject);
		}
		else if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerMovement>().healthPoints--;

			if (other.gameObject.GetComponent<PlayerMovement>().healthPoints <= 0)
			{
				GameController.Instance.ShowGameOverScreen();
			}
			
			Destroy(gameObject);
		}
	}

	private void Shoot()
	{
		rb.velocity = shootDirection * shootSpeed;
	}
}
