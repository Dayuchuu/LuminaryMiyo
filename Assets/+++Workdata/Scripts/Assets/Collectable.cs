using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
	private AudioSource collectedSound;

	private void Awake()
	{
		collectedSound = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GameController.Instance.coinPoints++;
			
			collectedSound.Play();
			
			gameObject.SetActive(false);
		}
	}
}
