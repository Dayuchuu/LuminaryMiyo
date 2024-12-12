using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	#region Variables
	
	public static UIManager Instance;

	public GameObject winScreen = null;
	public GameObject loseScreen = null;
	public GameObject pauseScreen = null;
	public GameObject mainMenuScreen = null;

	public TextMeshProUGUI scoreText = null;
	public TextMeshProUGUI timeText = null;
	
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
			Destroy(gameObject);
		}
	}

	public void ChangeScoreText(int score, string rank)
	{
		scoreText.text = rank + score;
	}

	public void StartGame()
	{
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.Level01;
		SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1);
		
		mainMenuScreen.gameObject.SetActive(false);
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
		SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, SceneLoader.Instance.currentScene, 1);
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
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.MainMenu;
		SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1);
		
	}

	public void Quit()
	{
		Application.Quit();
	}
	
	#endregion
}