using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public static SceneLoader Instance;

	public enum SceneStates
	{
		Manager = 0,
		MainMenu = 1,
		Level01 = 2,
		Level02 = 3
	}

	public SceneStates sceneStates;
	
	[HideInInspector]
	public int currentScene;
	
	private void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);

			return;
		}

		if (SceneManager.sceneCount < 2)
		{
			SceneManager.LoadScene((int)sceneStates, LoadSceneMode.Additive);
		}
		else
		{
			UIManager.Instance.CloseMenu(UIManager.Instance.mainMenuScreen, CursorLockMode.Locked, 1f);
		}

		currentScene = (int)sceneStates; 
	}

	public IEnumerator LoadScene(int oldScene, int newScene, int timeScale)
	{
		var unloadedScene = SceneManager.GetSceneByBuildIndex(oldScene);
		
		if (unloadedScene.isLoaded)
		{
			yield return SceneManager.UnloadSceneAsync(unloadedScene);

			sceneStates = (SceneStates)newScene;

			if (sceneStates >= SceneStates.Level01)
			{
				ObjectPool.sharedInstance.gameObject.GetComponent<ObjectPool>().enabled = true;
			}
		}

		SceneManager.LoadScene(newScene, LoadSceneMode.Additive);

		currentScene = newScene;

		Time.timeScale = timeScale;
	}
}
