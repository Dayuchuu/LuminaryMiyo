using UnityEngine;

public class Collectable : MonoBehaviour
{
	private AudioSource collectedSound;
	
	private void Awake()
	{
		//Gets the audio source
		collectedSound = GetComponent<AudioSource>();
	}

	/// <summary>
	/// gameObject gets deactivated, sound plays and changes the score.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GameController.Instance.ChangeScore(500);
			
			collectedSound.PlayOneShot(MusicManager.instance.coinCollected);
			
			UIManager.Instance.inGameScoreText.text = "Score: " + GameController.Instance.currentPoints;
			
			collectedSound.Play();
			
			gameObject.SetActive(false);
		}
	}
}
