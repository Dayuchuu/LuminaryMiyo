using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
	#region Variables
	
	[SerializeField] 
	private float shootSpeed = 0f;

	[SerializeField] private int setTimer = 0;

	[SerializeField] private AudioSource audioSource;
	
	private Transform playerTransform = null;

	private Rigidbody rb = null;
	
	Vector3 shootDirection = Vector3.zero;

	#endregion

	#region Methods
	
	private void Awake()
	{
		// Gets these components
		rb = gameObject.GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}
	
	/// <summary>
	/// When it hits the Player it counts 1 health point down, gets deactivated when colliding.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerMovement>().healthPoints--;

			audioSource.PlayOneShot(MusicManager.instance.playerHurt);
			
			UIManager.Instance.ChangeHearts();
			
			if (other.gameObject.GetComponent<PlayerMovement>().healthPoints <= 0)
			{
				Cursor.lockState = CursorLockMode.None;
				
				UIManager.Instance.OpenMenu(UIManager.Instance.loseScreen, CursorLockMode.None, 0f);
			}
			
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Rotates the bullet slightly. 
	/// </summary>
	private void Update()
	{
		transform.Rotate(Time.deltaTime * 5, 0, 0);
	}

	/// <summary>
	/// Shoots the bullet and start a coroutine. 
	/// </summary>
	public void StartBullet()
	{
		Shoot();

		StartCoroutine(SetInactiveAfterTime());
	}

	/// <summary>
	/// Calculates the movememnt of the bullet.
	/// </summary>
	private void Shoot()
	{
		playerTransform = GameObject.FindWithTag("Player").transform;
		
		shootDirection = playerTransform.position - transform.position;
		
		rb.velocity = shootDirection * shootSpeed;
	}

	/// <summary>
	/// Set the bullet inactive after certain amou tof time. 
	/// </summary>
	/// <returns></returns>
	IEnumerator SetInactiveAfterTime()
	{
		yield return new WaitForSeconds(setTimer);
		
		gameObject.SetActive(false);
	}
	#endregion
}
