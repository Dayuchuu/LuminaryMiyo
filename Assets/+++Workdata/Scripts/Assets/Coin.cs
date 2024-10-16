using UnityEngine;

public class Coin : MonoBehaviour
{
	private int coinAmount = 5;
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GameController.Instance.coinPoints += coinAmount;
			
			gameObject.SetActive(false);
		}
	}
}
