using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region Variables
	
	public static UIManager Instance;

	public GameObject winScreen = null;
	public GameObject loseScreen = null;
	public GameObject pauseScreen = null;
	
	[SerializeField] private Button play = null;
	[SerializeField] private Button settings = null;
	[SerializeField] private Button mainMenu = null;

	public TextMeshProUGUI scoreText = null;
	
	#endregion

	#region Methods
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void ChangeScoreText(int score, string rank)
	{
		scoreText.text = rank + score.ToString();
	}

	public void OpenMenu(GameObject menu, float timeScale)
	{
		menu.SetActive(true);
		
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		
		player.GetComponent<PlayerMovement>().DisablePlayerActions();

		Time.timeScale = timeScale;
	}

	public void CloseMenu(GameObject menu, float timeScale)
	{
		menu.SetActive(false);
		
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		
		player.GetComponent<PlayerMovement>().EnablePlayerActions();

		Time.timeScale = timeScale;
	}

	public void Replay()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.name);
	}
	
	public void StopPause()
	{
		CloseMenu(pauseScreen, 1f);
	}
	
	public void Settings()
	{
		
	}
	
	public void MainMenu()
	{
		SceneManager.LoadScene(0);

		Time.timeScale = 1f;
	}
	
	#endregion
}