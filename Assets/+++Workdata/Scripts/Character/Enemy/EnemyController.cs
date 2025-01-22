using UnityEngine;

public class EnemyController : MonoBehaviour
{
	#region Variables
	
	public static EnemyController Instance;

	public EnemyShooting[] enemies = new EnemyShooting[0];

	public int enemyAmount;
	
	#endregion

	#region Methods
	
	/// <summary>
	/// Creates the instance
	/// </summary>
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

	/// <summary>
	/// Finds all enemy shooting objects, sets the enemyAmount. 
	/// </summary>
	public void FindEnemies()
	{
		enemies = FindObjectsOfType<EnemyShooting>();
		enemyAmount = enemies.Length;
	}
	
	/// <summary>
	/// Calculates all inactive enemies currently in scene. 
	/// </summary>
	/// <returns></returns>
	public int FindInactiveEnemies()
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
