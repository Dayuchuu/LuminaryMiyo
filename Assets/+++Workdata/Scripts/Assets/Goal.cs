using System;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
	#region Variables

	private PlayerMovement player = null;

	[SerializeField]
	private TextMeshProUGUI scoreText;

	[SerializeField] 
	private int maxTimePoints = 0;

	private int enemyAmount = 0;
	
	private int coinAmount = 0;

	private int timeAmount = 0;

	private int scoreAmount = 0;
	
	#endregion

	#region Methods
	
	private void OnValidate()
	{
		player = FindObjectOfType<PlayerMovement>();

		enemyAmount = FindObjectsOfType<EnemyShooting>().Length;

		coinAmount = FindObjectsOfType<Coin>().Length;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			UIManager.Instance.OpenMenu(UIManager.Instance.winScreen, 0f);

			player.DisablePlayerActions();

			GetMaxScore();
			
			Time.timeScale = 0f;
		}
	}

	private void GetMaxScore()
	{
		int maxScoreAmount = 0;
		
		maxScoreAmount += enemyAmount * 500;

		maxScoreAmount += coinAmount * 500;

		maxScoreAmount += maxTimePoints;
		
		GetScore(maxScoreAmount);
	}

	private void GetScore(int currentMaxScore)
	{
		scoreAmount += EnemyController.Instance.FindCurrentEnemies() * 500;
		
		scoreAmount += GameController.Instance.coinPoints * 500;

		scoreAmount += GameController.Instance.timePoints;
		
		GetRank(scoreAmount, currentMaxScore);
	}

	private void GetRank(int currentScore, int currentMaxScore)
	{
		if (currentScore >= (int)(currentMaxScore * 0) && currentScore <= (int)(currentMaxScore * 0.25))
		{
			scoreText.text = "C " + currentScore;
		}
		else if (currentScore >= (int)(currentMaxScore * 0.25) && currentScore <= (int)(currentMaxScore * 0.5))
		{
			scoreText.text = "B " + currentScore;
		}
		else if (currentScore >= (int)(currentMaxScore * 0.5) && currentScore <= (int)(currentMaxScore * 0.75))
		{
			scoreText.text = "A " + currentScore;
		}
		else if (currentScore >= (int)(currentMaxScore * 0.75))
		{
			scoreText.text = "S " + currentScore;
		}
	}
	#endregion
}
