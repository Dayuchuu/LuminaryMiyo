using UnityEngine;

public class Goal : MonoBehaviour
{
	#region Variables

	private PlayerMovement player = null;

	[SerializeField] 
	private int maxTimePoints = 0;

	[SerializeField] private bool isSecondLevel;

	private int enemyAmount = 0;
	
	private int coinAmount = 0;

	private int scoreAmount = 0;

	private GameObject dialogueSystem;
	
	#endregion

	#region Methods
	
	/// <summary>
	/// Gets component on gameObject and finds all enemies.
	/// </summary>
	private void Start()
	{
		EnemyController.Instance.FindEnemies();
		
		player = FindObjectOfType<PlayerMovement>();

		enemyAmount = FindObjectsOfType<EnemyShooting>().Length;

		coinAmount = FindObjectsOfType<Collectable>().Length;
	}

	/// <summary>
	/// pens the win screen, sets the level finished bool to true an calls the FinishedLevel methdo. 
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			UIManager.Instance.inGameUi.SetActive(false);
			UIManager.Instance.OpenMenu(UIManager.Instance.winScreen, CursorLockMode.None, 0f);
			
			if (SceneLoader.Instance.sceneStates == SceneLoader.SceneStates.Level01)
			{
				GameController.Instance.level01Finished = true;
				GameController.Instance.FinishedLevel01();
			}
			else if (SceneLoader.Instance.sceneStates == SceneLoader.SceneStates.Level02)
			{
				GameController.Instance.level01Finished = true;
				GameController.Instance.FinishedLevel02();
			}
			
			GetMaxScore();
		}
	}

	/// <summary>
	/// Calculates the max score amount in the level possible. 
	/// </summary>
	private void GetMaxScore()
	{
		int maxScoreAmount = 0;
		
		maxScoreAmount += enemyAmount * 500;

		maxScoreAmount += coinAmount * 500;

		maxScoreAmount += maxTimePoints;
		
		GetScore(maxScoreAmount);
	}

	/// <summary>
	/// Calculates the score amount. 
	/// </summary>
	/// <param name="currentMaxScore"></param>
	private void GetScore(int currentMaxScore)
	{
		scoreAmount += GameController.Instance.currentPoints;

		scoreAmount += (int)UIManager.Instance.timer.time;
		
		GetRank(scoreAmount, currentMaxScore);
	}

	/// <summary>
	/// Allocates the rank depending on the score achieved and the max score possible. 
	/// </summary>
	/// <param name="currentScore"></param>
	/// <param name="currentMaxScore"></param>
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

		if (isSecondLevel)
		{
			UIManager.Instance.highScoreText.text = currentScore.ToString();
			PlayerPrefs.SetFloat(UIManager.highScore, currentScore);
		}
	}
	#endregion
}
