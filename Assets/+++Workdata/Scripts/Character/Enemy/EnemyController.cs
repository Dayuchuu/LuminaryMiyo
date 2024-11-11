using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	#region Variables
	
	public static EnemyController Instance;

	public EnemyShooting[] enemies = new EnemyShooting[0];
	
	#endregion

	#region Methods
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(this);

			return;
		}
		
		FindEnemies();
	}

	public void FindEnemies()
	{
		enemies = FindObjectsOfType<EnemyShooting>();
	}
	
	public int FindCurrentEnemies()
	{
		int enemyInactive = 0;
		
		for (int enemyIndex = 0,  enemyAmount = enemies.Length; enemyIndex < enemyAmount; enemyIndex++)
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
