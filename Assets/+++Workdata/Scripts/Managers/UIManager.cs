using UnityEngine;

public class UIManager : MonoBehaviour
{
	
	
	#region Variables
	public static UIManager Instance;

	public GameObject winScreen = null;
	public GameObject loseScreen = null;
	public GameObject pauseScreen = null;

	#endregion

	private void Awake()
	{
		if (Instance != null)
		{
			Instance = this;
		}
	}

	public void OpenMenu(GameObject menu, float timeScale)
	{
		menu.SetActive(true);
		
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		
		player.GetComponent<PlayerMovement>().DisablePlayerActions();

		Time.timeScale = timeScale;
	}
}
