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

	private int scoreAmount = 0;
	
	#endregion

	#region Methods
	
	private void OnValidate()
	{
		player = FindObjectOfType<PlayerMovement>();

		enemyAmount = FindObjectsOfType<EnemyShooting>().Length;

		coinAmount = FindObjectsOfType<Collectable>().Length;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			UIManager.Instance.OpenMenu(UIManager.Instance.winScreen, CursorLockMode.None, 0f);

			player.DisablePlayerActions();
			
			Cursor.lockState = CursorLockMode.None;

			GetMaxScore();
			
			//Time.timeScale = 0f;
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
			UIManager.Instance.ChangeScoreText(currentScore, "C ");
		}
		else if (currentScore >= (int)(currentMaxScore * 0.25) && currentScore <= (int)(currentMaxScore * 0.5))
		{
			UIManager.Instance.ChangeScoreText(currentScore, "B ");
		}
		else if (currentScore >= (int)(currentMaxScore * 0.5) && currentScore <= (int)(currentMaxScore * 0.75))
		{
			UIManager.Instance.ChangeScoreText(currentScore, "A ");
		}
		else if (currentScore >= (int)(currentMaxScore * 0.75))
		{
			UIManager.Instance.ChangeScoreText(currentScore, "S ");
		}
	}
	#endregion
}
