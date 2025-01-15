using UnityEngine;

public class EnemyController : MonoBehaviour
{
	#region Variables
	
	public static EnemyController Instance;

	public EnemyShooting[] enemies = new EnemyShooting[0];

	public int enemyAmount;
	
	#endregion

	#region Methods
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);

			return;
		}
	}

	public void FindEnemies()
	{
		enemies = FindObjectsOfType<EnemyShooting>();
		enemyAmount = enemies.Length;
	}
	
	public int FindCurrentEnemies()
	{
		int enemyInactive = 0;
		
		for (int enemyIndex = 0,  currentEnemyAmount = enemies.Length; enemyIndex < currentEnemyAmount; enemyIndex++)
		{
			if (!enemies[enemyIndex].gameObject.activeInHierarchy)
			{
				enemyInactive++;
			}
		}

		return enemyInactive;
	}
	#endregion
}
