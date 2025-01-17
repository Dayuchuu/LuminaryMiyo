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
			GameController.Instance.ChangeScore(500);
			
			UIManager.Instance.inGameScoreText.text = "Score: " + GameController.Instance.currentPoints;
			
			collectedSound.Play();
			
			gameObject.SetActive(false);
		}
	}
}
