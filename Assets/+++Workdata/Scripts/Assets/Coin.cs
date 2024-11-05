using UnityEngine;

public class Coin : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GameController.Instance.coinPoints++;
			
			gameObject.SetActive(false);
		}
	}
}
