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
	[Space]
	
	[Header("Texts")]
	public TextMeshProUGUI scoreText = null;
	public TextMeshProUGUI timeText = null;
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
	
	private bool uiOpen = true;
	private GameObject currentScreen = null;

	[SerializeField] private List<GameObject> uiScreens;
	
	public GameObject player;
	
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
		
		fovSlider.onValueChanged.AddListener(delegate { OnSliderChanged(fovSlider, fov);});
		cameraSensitivitySlider.onValueChanged.AddListener(delegate { OnSliderChanged(cameraSensitivitySlider, cameraSensibility);});
		masterSlider.onValueChanged.AddListener(delegate { OnSliderChanged(masterSlider, master);});
		musicSlider.onValueChanged.AddListener(delegate { OnSliderChanged(musicSlider, music);});
		sfxSlider.onValueChanged.AddListener(delegate { OnSliderChanged(sfxSlider, sfx);});

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

	public void StartGame()
	{
		SceneLoader.Instance.sceneStates = SceneLoader.SceneStates.Level01;
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, (int)SceneLoader.Instance.sceneStates,  (int)SceneLoader.SceneStates.Portal, 1));
		
		CloseMenu(mainMenuScreen, CursorLockMode.Locked, 1f);
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
			
			player.GetComponent<PlayerMovement>().EnablePlayerActions();
			
			Cursor.lockState = lockMode;

			Time.timeScale = timeScale;

			uiOpen = false;
		}
	}

	public void Replay()
	{
		CloseMenu(winScreen, loseScreen, CursorLockMode.None, 1f);
		
		SceneLoader.Instance.StartCoroutine(SceneLoader.Instance.LoadScene(SceneLoader.Instance.currentScene, SceneLoader.Instance.currentScene, 1));
	}
	
	public void StopPause()
	{
		CloseMenu(pauseScreen, CursorLockMode.Locked, 1f);
	}
	
	public void Settings()
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

	public void Quit()
	{
		Application.Quit();
	}
	
	#endregion
}