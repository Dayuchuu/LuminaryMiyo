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

	private void Shoot()
	{
		rb.velocity = shootDirection * shootSpeed;
	}
}
