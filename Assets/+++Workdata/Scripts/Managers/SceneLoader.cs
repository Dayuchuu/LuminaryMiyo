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
		Portal = 2,
		Level01 = 3,
		Level02 = 4
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

	public IEnumerator LoadScene(int oldScene, int firstNewScene, int timeScale)
	{
		var unloadedScene = SceneManager.GetSceneByBuildIndex(oldScene);
		
		if (unloadedScene.isLoaded)
		{
			yield return SceneManager.UnloadSceneAsync(unloadedScene);

			sceneStates = (SceneStates)firstNewScene;

			if (sceneStates >= SceneStates.Level01)
			{
				ObjectPool.sharedInstance.gameObject.GetComponent<ObjectPool>().enabled = true;
			}
		}

		SceneManager.LoadScene(firstNewScene, LoadSceneMode.Additive);

		currentScene = firstNewScene;

		Time.timeScale = timeScale;
	}
    
	public IEnumerator LoadScene(int oldScene, int firstNewScene , int secondNewScene, int timeScale)
	{
		var unloadedScene = SceneManager.GetSceneByBuildIndex(oldScene);
		
		if (unloadedScene.isLoaded)
		{
			yield return SceneManager.UnloadSceneAsync(unloadedScene);

			sceneStates = (SceneStates)firstNewScene;

			if (sceneStates >= SceneStates.Level01)
			{
				ObjectPool.sharedInstance.gameObject.GetComponent<ObjectPool>().enabled = true;
			}
		}

		SceneManager.LoadScene(firstNewScene, LoadSceneMode.Additive);
		SceneManager.LoadScene(secondNewScene, LoadSceneMode.Additive);

		currentScene = firstNewScene;

		Time.timeScale = timeScale;
	}
}
