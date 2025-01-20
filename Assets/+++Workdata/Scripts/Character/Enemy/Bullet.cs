using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	#region Variables
	
	[SerializeField] 
	private float shootSpeed = 0f;

	[SerializeField]
	private int setTimer = 0;
	
	private Transform playerTransform = null;

	private Rigidbody rb = null;
	
	Vector3 shootDirection = Vector3.zero;

	[SerializeField] private AudioSource bulletSound;

	#endregion

	#region Methods
	
	private void Awake()
	{
		rb = gameObject.GetComponent<Rigidbody>();

		bulletSound = GetComponent<AudioSource>();
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerMovement>().healthPoints--;
			
			UIManager.Instance.ChangeHearts();
			
			if (other.gameObject.GetComponent<PlayerMovement>().healthPoints <= 0)
			{
				Cursor.lockState = CursorLockMode.None;
				
				UIManager.Instance.OpenMenu(UIManager.Instance.loseScreen, CursorLockMode.None, 0f);
			}
			
			gameObject.SetActive(false);
		}
		else if(!other.CompareTag("Player") && !other.CompareTag("Enemy"))
		{
			gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		transform.Rotate(Time.deltaTime * 5, 0, 0);
	}

	public void StartBullet()
	{
		Shoot();

		StartCoroutine(SetInactiveAfterTime());
	}

	private void Shoot()
	{
		playerTransform = GameObject.FindWithTag("Player").transform;
		
		shootDirection = playerTransform.position - transform.position;
		
		rb.velocity = shootDirection * shootSpeed;
	}

	IEnumerator SetInactiveAfterTime()
	{
		yield return new WaitForSeconds(setTimer);
		
		gameObject.SetActive(false);
	}
	#endregion
}
