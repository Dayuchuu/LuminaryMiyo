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
	public const string highScore = "Highscore";

	[Header("Screens")]
	public GameObject winScreen = null;
	public GameObject loseScreen = null;
	public GameObject pauseScreen = null;
	public GameObject mainMenuScreen = null;
	public GameObject levelSelectionScreen = null;
	[SerializeField] private List<GameObject> uiScreens;
	[Space]
	
	[Header("Texts")]
	public TextMeshProUGUI scoreText = null;
	public TextMeshProUGUI timeText = null;
	public TextMeshProUGUI inGameScoreText = null;
	public TextMeshProUGUI highScoreText = null;
	public TextMeshProUGUI fovTexts = null;

	[Space] [Header("Audio")] 
	[SerializeField] private AudioSource audioSource;
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
	public Timer timer;
	public List<Image> hearts;
	public GameObject preTutorial;
	public GameObject tutorialDialogue;
	public GameObject levelDialogue;
	
	private bool uiOpen = true;
	private GameObject currentScreen = null;
	private int currentHealth = 5;
	
	private GameObject player;
	
	#endregion

	#region Methods
	
	/// <summary>
	/// Creates the instance and sets different values
	/// </summary>
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
		
		//Gets the info if we have beaten level 01 already
		if (Convert.ToBoolean(PlayerPrefs.GetInt(GameController.level01)))
		{
			level02.interactable = true;
		}
		
		//Sets all sliders to the OnSliderChanged method
		fovSlider.onValueChanged.AddListener(delegate { OnSliderChanged(fovSlider, fov);});
		cameraSensitivitySlider.onValueChanged.AddListener(delegate { OnSliderChanged(cameraSensitivitySlider, cameraSensibility);});
		masterSlider.onValueChanged.AddListener(delegate { OnSliderChanged(masterSlider, master);});
		musicSlider.onValueChanged.AddListener(delegate { OnSliderChanged(musicSlider, music);});
		sfxSlider.onValueChanged.AddListener(delegate { OnSliderChanged(sfxSlider, sfx);});
		
		//Gets alls slider values
		GetSliderValues(fovSlider, fov);
		GetSliderValues(cameraSensitivitySlider, cameraSensibility);
		GetSliderValues(masterSlider, master);
		GetSliderValues(musicSlider, music);
		GetSliderValues(sfxSlider, sfx);

		fovTexts.text = fovSlider.value.ToString();

		// sfxSlider.onValueChanged.AddListener((sliderValue) =>
		// {
		// 	mixer.SetFloat(name, sliderValue);
		// 	
		// 	PlayerPrefs.SetFloat(name, 1f);
		// });
	}

	/// <summary>
	/// Gets mixer values when already saved.
	/// </summary>
	private void Start()
	{
		mixer.SetFloat(master, Mathf.Log10(masterSlider.value) * 20);
		mixer.SetFloat(music, Mathf.Log10(musicSlider.value) * 20);
		mixer.SetFloat(sfx, Mathf.Log10(sfxSlider.value) * 20);

		highScoreText.text = PlayerPrefs.GetFloat(highScore).ToString();
	}

	/// <summary>
	/// Changes score text.
	/// </summary>
	/// <param name="score"></param>
	/// <param name="rank"></param>
	public void ChangeScoreText(int score, string rank)
	{
		scoreText.text = rank + score;
	}

	/// <summary>
	/// Loads level 01 and starts everything necessary for it.
	/// </summary>
	public void LoadLevel01()
	{
		
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.Level01;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1));
		
		timer.StartCountdown();
		
		tutorialDialogue.SetActive(true);
		
		ResetHearts();

		ResetTimer();
		
		GameController.Instance.ResetGameStats();

		GameController.Instance.gameStates = GameController.GameStates.InGame;
		
		CloseMenu(levelSelectionScreen,winScreen, CursorLockMode.Locked, 1f);
		
		OpenMenu(tutorialDialogue, CursorLockMode.None, 1f);
		
		tutorialDialogue.GetComponentInChildren<Dialogue>().EmptyValues();
	}
	
	/// <summary>
	/// Loads level 02 and starts everything necessary for it.
	/// </summary>
	public void LoadLevel02()
	{
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.Level02;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1));
		
		timer.StartCountdown();

		inGameScoreText.text = "Score: 0";

		ResetHearts();

		ResetTimer();
		
		GameController.Instance.ResetGameStats();
		
		CloseMenu(levelSelectionScreen, winScreen, CursorLockMode.Locked, 1f);

		OpenMenu(levelDialogue, CursorLockMode.None, 1f);
		
		levelDialogue.GetComponentInChildren<Dialogue>().EmptyValues();
	}

	/// <summary>
	/// Opens a menu with given values and prevents other menu to get opened
	/// </summary>
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

	/// <summary>
	/// Closes a menu with given values and prevents other menu to get opened.
	/// </summary>
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
	
	/// <summary>
	/// Closes multiple menus with given values and prevents other menu to get opened.
	/// </summary>
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

	/// <summary>
	/// Restarts the last scene and resets all values.
	/// </summary>
	public void Replay()
	{
		CloseMenu(winScreen, loseScreen, CursorLockMode.Locked, 1f);
		
		inGameUi.SetActive(true);
		
		ResetHearts();

		ResetTimer();
		
		inGameScoreText.text = "Score: 0";
		
		GameController.Instance.ResetGameStats();
		
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, SceneLoader.Instance.currentScene, 1));
	}
	
	/// <summary>
	/// Stops the pause menu. 
	/// </summary>
	public void StopPause()
	{
		inGameUi.SetActive(true);
		CloseMenu(pauseScreen, CursorLockMode.Locked, 1f);
	}
	
	/// <summary>
	/// Gets the current ui screen and saves it.
	/// </summary>
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

	/// <summary>
	/// Sets the screen back to the last saved screen.
	/// </summary>
	public void BackToPreviousScreen()
	{
		currentScreen.SetActive(true);
	}
	
	/// <summary>
	/// Loads the main menu scene, closes the last open ui menu and opens the main menu screen.
	/// </summary>
	public void MainMenu()
	{
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.MainMenu;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates, 1));
		
		GetCurrentScreen();
		CloseMenu(currentScreen, CursorLockMode.None, 1f);

		OpenMenu(mainMenuScreen, CursorLockMode.None, 1f);
	}

	/// <summary>
	/// Changes hearts depending on life of the player.
	/// </summary>
	public void ChangeHearts()
	{
		hearts[currentHealth -1].gameObject.SetActive(false);
		currentHealth--;
	}

	/// <summary>
	/// Starts the timer countdown.
	/// </summary>
	public void StartCountdown()
	{
		timer.startCountDown = true;
	}

	/// <summary>
	/// Resets the hearts amount when starting or retrying level.
	/// </summary>
	private void ResetHearts()
	{
		currentHealth = 5;

		for (int i = 0, maxHearts = hearts.Count; i < maxHearts; i++)
		{
			hearts[i].gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Saves slider data to given key name, sets mixer value when key name is a correct one.
	/// </summary>
	private void OnSliderChanged(Slider slider, string keyName)
	{
		PlayerPrefs.SetFloat(keyName, slider.value);
		
		player = GameObject.FindGameObjectWithTag("Player");

		if (player != null)
		{
			player.GetComponent<PlayerMovement>().ChangeSettings();
		}
		
		switch (keyName)
		{
			case master:
			case music:
			case sfx:
				mixer.SetFloat(keyName, Mathf.Log10(slider.value) * 20);
				break;
		}

		if (keyName == fov)
		{
			fovTexts.text = PlayerPrefs.GetFloat(fov, slider.value).ToString();
		}
	}

	/// <summary>
	/// Gets the slider values saved in the player prefs. 
	/// </summary>
	private void GetSliderValues(Slider slider, string keyName)
	{
		if (PlayerPrefs.GetFloat(keyName) > 0.05f)
		{
			slider.value = PlayerPrefs.GetFloat(keyName);
		}
	}

	/// <summary>
	/// Plays the Ui button click sound.
	/// </summary>
	public void PlayButtonSound()
	{
		audioSource.PlayOneShot(MusicManager.instance.buttonClickSound);
	}
	
	/// <summary>
	/// Resets the timer.
	/// </summary>
	private void ResetTimer()
	{
		timer.ResetTimer();
	}

	/// <summary>
	/// Quits the game. 
	/// </summary>
	public void Quit()
	{
		Application.Quit();
	}
	
	#endregion
}