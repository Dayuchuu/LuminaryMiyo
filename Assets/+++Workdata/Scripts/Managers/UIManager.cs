using TMPro;
using UnityEngine;

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

	[SerializeField] private AudioSource buttonSounds;
	
	private bool uiOpen = true;
	
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

		buttonSounds = GetComponent<AudioSource>();
	}

	public void ChangeScoreText(int score, string rank)
	{
		scoreText.text = rank + score;
	}

	public void StartGame()
	{
		buttonSounds.Play();
		
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.Level01;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates,  (int)SceneLoader.SceneStates.Portal, 1));
		
		mainMenuScreen.gameObject.SetActive(false);
	}

	public void OpenMenu(GameObject menu, CursorLockMode lockMode, float timeScale)
	{
		buttonSounds.Play();
		
		if (!uiOpen)
		{
			menu.SetActive(true);
			
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			
			player.GetComponent<PlayerMovement>().DisablePlayerActions();

			Cursor.lockState = lockMode;

			Time.timeScale = timeScale;

			uiOpen = true;
		}
	}

	public void CloseMenu(GameObject menu, CursorLockMode lockMode, float timeScale)
	{
		buttonSounds.Play();
		
		if (uiOpen)
		{
			menu.SetActive(false);
			
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			
			player.GetComponent<PlayerMovement>().EnablePlayerActions();
			
			Cursor.lockState = lockMode;

			Time.timeScale = timeScale;

			uiOpen = false;
		}
	}
	
	public void CloseMenu(GameObject firstMenu, GameObject secondMenu, CursorLockMode lockMode, float timeScale)
	{
		buttonSounds.Play();
		
		if (uiOpen)
		{
			firstMenu.SetActive(false);
			
			secondMenu.SetActive(false);
			
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			
			player.GetComponent<PlayerMovement>().EnablePlayerActions();
			
			Cursor.lockState = lockMode;

			Time.timeScale = timeScale;

			uiOpen = false;
		}
	}

	public void Replay()
	{
		buttonSounds.Play();
		
		CloseMenu(winScreen, loseScreen, CursorLockMode.None, 1f);
		
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, SceneLoader.Instance.currentScene, 1));
	}
	
	public void StopPause()
	{
		buttonSounds.Play();
		
		CloseMenu(pauseScreen, CursorLockMode.Locked, 1f);
	}
	
	public void Settings()
	{
		buttonSounds.Play();
		
		
	}
	
	public void MainMenu()
	{
		buttonSounds.Play();
		
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.MainMenu;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1));
	}

	public void Quit()
	{
		buttonSounds.Play();
		
		Application.Quit();
	}
	
	#endregion
}