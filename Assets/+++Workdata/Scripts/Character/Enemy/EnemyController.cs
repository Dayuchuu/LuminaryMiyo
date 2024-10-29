using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public int enemyAmount = 0;

	private void OnValidate()
	{
		enemyAmount = GameObject.FindObjectsOfType<EnemyShooting>().Length;
	}
}
