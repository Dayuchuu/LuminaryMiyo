using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager instance;
	
	[Space] 
	[Header("SFX")] 
	public AudioClip enemyShootSound;
	public AudioClip enemyDeathSound;
	public AudioClip playerAttack;
	public AudioClip playerHurt;
	public AudioClip buttonClickSound;
	public AudioClip landingSound;
	public AudioClip jumpingSound;
	public AudioClip coinCollected;
	public AudioClip[] playerSteps;
	
	/// <summary>
	/// Creates the instance
	/// </summary>
 	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}
	}
}
