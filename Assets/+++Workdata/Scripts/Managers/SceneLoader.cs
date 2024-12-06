using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
	
	private void Awake()
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

		int loadedScenes = SceneManager.sceneCount;
		
		for (int sceneIndex = 0; sceneIndex <= loadedScenes; sceneIndex++)
		{
			if (SceneManager.GetSceneAt(sceneIndex) != SceneManager.GetSceneAt(0))
			{
				SceneManager.UnloadSceneAsync(sceneIndex);
			}
		}
		
		SceneManager.LoadScene((int)sceneStates, LoadSceneMode.Additive);

		currentScene = (int)sceneStates; 
	}

	public void LoadScene(int oldScene, int newScene, int timeScale)
	{
		SceneManager.UnloadSceneAsync(oldScene);

		SceneManager.LoadScene(newScene, LoadSceneMode.Additive);

		Time.timeScale = timeScale;
	}
}
