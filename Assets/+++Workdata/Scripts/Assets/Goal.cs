using UnityEngine;

public class Goal : MonoBehaviour
{
	#region Variables

	private PlayerMovement player = null;

	[SerializeField] 
	private int maxTimePoints = 0;

	[SerializeField] private bool showCredits;

	private int enemyAmount = 0;
	
	private int coinAmount = 0;

	private int scoreAmount = 0;

	private GameObject dialogueSystem;
	
	#endregion

	#region Methods
	
	private void Start()
	{
		EnemyController.Instance.FindEnemies();
		
		player = FindObjectOfType<PlayerMovement>();

		enemyAmount = FindObjectsOfType<EnemyShooting>().Length;

		coinAmount = FindObjectsOfType<Collectable>().Length;
	}

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
		scoreAmount += GameController.Instance.currentPoints;

		scoreAmount += (int)UIManager.Instance.timer.time;
		
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
