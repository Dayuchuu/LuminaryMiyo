using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class UIManager : MonoBehaviour
{
	#region Variables
	
	public static UIManager Instance;

	public const string cameraSensibility = "camera";
	public const string fov = "fov";
	public const string master = "Master";
	public const string music = "Music";
	public const string sfx = "SFX";

	[Header("Screens")]
	public GameObject winScreen = null;
	public GameObject loseScreen = null;
	public GameObject pauseScreen = null;
	public GameObject mainMenuScreen = null;
	public GameObject levelSelectionScreen = null;
	public GameObject credits = null;
	[SerializeField] private List<GameObject> uiScreens;
	[Space]
	
	[Header("Texts")]
	public TextMeshProUGUI scoreText = null;
	public TextMeshProUGUI timeText = null;
	public TextMeshProUGUI inGameScoreText = null;
	[Space]
	
	[Header("Audio")]
	[SerializeField] private AudioMixer mixer;
	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider musicSlider;
	[SerializeField] private Slider sfxSlider;
	[Space]
	
	[Header("Gameplay Settings")]
	public Slider fovSlider;
	public Slider cameraSensitivitySlider;
	[Space] 
	
	[Header("Level Selection")] 
	public Button level01;
	public Button level02;
	[Space]
	
	[Header("InGame")]
	public GameObject inGameUi;
	public Image jumpIndicator;
	public Image dashIndicator;
	public Timer timer;
	public List<Image> hearts;
	public GameObject tutorialDialogue;
	public GameObject levelDialogue;
	
	private bool uiOpen = true;
	private GameObject currentScreen = null;
	private int currentHealth = 5;
	
	private GameObject player;
	
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
		
		if (Convert.ToBoolean(PlayerPrefs.GetInt(GameController.level01)))
		{
			level02.interactable = true;
		}
		
		fovSlider.onValueChanged.AddListener(delegate { OnSliderChanged(fovSlider, fov);});
		cameraSensitivitySlider.onValueChanged.AddListener(delegate { OnSliderChanged(cameraSensitivitySlider, cameraSensibility);});
		masterSlider.onValueChanged.AddListener(delegate { OnSliderChanged(masterSlider, master);});
		musicSlider.onValueChanged.AddListener(delegate { OnSliderChanged(musicSlider, music);});
		sfxSlider.onValueChanged.AddListener(delegate { OnSliderChanged(sfxSlider, sfx);});


		GetSliderValues(fovSlider, fov);
		GetSliderValues(cameraSensitivitySlider, cameraSensibility);
		GetSliderValues(masterSlider, master);
		GetSliderValues(musicSlider, music);
		GetSliderValues(sfxSlider, sfx);

		mixer.SetFloat(master, masterSlider.value);
		mixer.SetFloat(music, musicSlider.value);
		mixer.SetFloat(sfx, sfxSlider.value);

		// sfxSlider.onValueChanged.AddListener((sliderValue) =>
		// {
		// 	mixer.SetFloat(name, sliderValue);
		// 	
		// 	PlayerPrefs.SetFloat(name, 1f);
		// });
	}

	public void ChangeScoreText(int score, string rank)
	{
		scoreText.text = rank + score;
	}

	public void LoadLevel01()
	{
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.Level01;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1));
		
		timer.StartCountdown();
		
		tutorialDialogue.SetActive(true);
		
		ResetInGameUi();
		
		ResetHearts();

		ResetTimer();
		
		GameController.Instance.ResetGameStats();

		GameController.Instance.gameStates = GameController.GameStates.InGame;
		
		CloseMenu(levelSelectionScreen,winScreen, CursorLockMode.Locked, 1f);
		
		OpenMenu(tutorialDialogue, CursorLockMode.None, 1f);
	}
	
	public void LoadLevel02()
	{
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.Level02;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1));
		
		timer.StartCountdown();
		
		ResetInGameUi();
		

		inGameScoreText.text = "Score: 0";

		ResetHearts();

		ResetTimer();
		
		GameController.Instance.ResetGameStats();
		
		CloseMenu(levelSelectionScreen, winScreen, CursorLockMode.Locked, 1f);
		
		OpenMenu(levelDialogue, CursorLockMode.None, 1f);
	}

	public void OpenMenu(GameObject menu, CursorLockMode lockMode, float timeScale)
	{
		if (!uiOpen)
		{
			menu.SetActive(true);
			
			player = GameObject.FindGameObjectWithTag("Player");

			if (player != null)
			{
				player.GetComponent<PlayerMovement>().DisablePlayerActions();
			}

			Cursor.lockState = lockMode;

			Time.timeScale = timeScale;

			uiOpen = true;
		}
	}

	public void CloseMenu(GameObject menu, CursorLockMode lockMode, float timeScale)
	{
		if (uiOpen)
		{
			menu.SetActive(false);
			
			player = GameObject.FindGameObjectWithTag("Player");

			if (player != null)
			{
				player.GetComponent<PlayerMovement>().EnablePlayerActions();
			}
			
			Cursor.lockState = lockMode;

			Time.timeScale = timeScale;
			
			GetCurrentScreen();

			uiOpen = false;
		}
	}
	
	public void CloseMenu(GameObject firstMenu, GameObject secondMenu, CursorLockMode lockMode, float timeScale)
	{
		if (uiOpen)
		{
			firstMenu.SetActive(false);
			
			secondMenu.SetActive(false);
			
			player = GameObject.FindGameObjectWithTag("Player");
			
			if (player != null)
			{
				player.GetComponent<PlayerMovement>().EnablePlayerActions();
			}
			
			Cursor.lockState = lockMode;

			Time.timeScale = timeScale;
			
			GetCurrentScreen();

			uiOpen = false;
		}
	}

	public void Replay()
	{
		CloseMenu(winScreen, loseScreen, CursorLockMode.None, 1f);
		
		inGameUi.SetActive(false);
		
		ResetHearts();

		ResetTimer();
		
		inGameScoreText.text = "Score: 0";
		
		GameController.Instance.ResetGameStats();
		
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, SceneLoader.Instance.currentScene, 1));
	}
	
	public void StopPause()
	{
		inGameUi.SetActive(true);
		CloseMenu(pauseScreen, CursorLockMode.Locked, 1f);
	}
	
	public void GetCurrentScreen()
	{
		for (int i = 0; i < uiScreens.Count; i++)
		{
			if (uiScreens[i].activeInHierarchy)
			{
				currentScreen = uiScreens[i];
			}
		}
	}

	public void BackToPreviousScreen()
	{
		currentScreen.SetActive(true);
	}
	
	public void MainMenu()
	{
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.MainMenu;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1));
		
		GetCurrentScreen();
		CloseMenu(currentScreen, CursorLockMode.None, 1f);

		OpenMenu(mainMenuScreen, CursorLockMode.None, 1f);
	}

	public void ChangeHearts()
	{
		hearts[currentHealth -1].gameObject.SetActive(false);
		currentHealth--;
	}

	public void StartCountdown()
	{
		timer.startCountDown = true;
	}

	private void ResetHearts()
	{
		currentHealth = 5;

		for (int i = 0, maxHearts = hearts.Count; i < maxHearts; i++)
		{
			hearts[i].gameObject.SetActive(true);
		}
	}

	private void OnSliderChanged(Slider slider, string keyName)
	{
		PlayerPrefs.SetFloat(keyName, slider.value);
		
		player = GameObject.FindGameObjectWithTag("Player");

		if (player != null)
		{
			player.GetComponent<PlayerMovement>().ChangeValues();
		}
		
		switch (keyName)
		{
			case master:
			case music:
			case sfx:
				mixer.SetFloat(keyName, slider.value);
				break;
		}
	}

	private void GetSliderValues(Slider slider, string keyName)
	{
		if (PlayerPrefs.GetFloat(keyName) != 0)
		{
			slider.value = PlayerPrefs.GetFloat(keyName);
		}
	}

	private void ResetInGameUi()
	{
		dashIndicator.color = Color.yellow;
		jumpIndicator.color = Color.blue;
	}
	
	private void ResetTimer()
	{
		timer.ResetTimer();
	}

	public void Quit()
	{
		Application.Quit();
	}
	
	#endregion
}