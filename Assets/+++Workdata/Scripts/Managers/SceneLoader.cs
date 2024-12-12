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

		if (SceneManager.sceneCount < 2)
		{
			SceneManager.LoadScene((int)sceneStates, LoadSceneMode.Additive);
		}

		currentScene = (int)sceneStates; 
	}

	public void LoadScene(int oldScene, int newScene, int timeScale)
	{
		SceneManager.UnloadSceneAsync(oldScene);

		SceneManager.LoadScene(newScene, LoadSceneMode.Additive);

		Time.timeScale = timeScale;
	}
}
