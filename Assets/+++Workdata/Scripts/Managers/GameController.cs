using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    #region Definitons

    public enum GameStates
    {
        MainMenu,
        InGame
    }
    
    #endregion
    
    #region Variables

    public static GameController Instance;

    public const string level01 = "level01";
    
    public GameStates gameStates;
    
    [FormerlySerializedAs("currenPoints")] public int currentPoints;

    public bool level01Finished;
    public bool level02Finished;
    
    #endregion

    #region Methods
    
    /// <summary>
    /// Creates the instance
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

            return;
        }
        
    }

    /// <summary>
    /// Saves and sets that level 01 is finished
    /// </summary>
    public void FinishedLevel01()
    {
        PlayerPrefs.SetInt(level01, Convert.ToInt32(level01Finished));
        UIManager.Instance.level02.interactable = true;
    }
    
    /// <summary>
    /// safes that level 02 is finished
    /// </summary>
    public void FinishedLevel02()
    {
        PlayerPrefs.SetInt(level01, Convert.ToInt32(level02Finished));
    }

    /// <summary>
    /// Resets coin value
    /// </summary>
    public void ResetGameStats()
    {
        currentPoints = 0;
    }

    /// <summary>
    /// Changes the score depending on the new value incoming. 
    /// </summary>
    /// <param name="currentAdd"></param>
    public void ChangeScore(int currentAdd)
    {
        currentPoints += currentAdd;
        UIManager.Instance.scoreText.text = currentPoints.ToString();
    }
    
    #endregion
}
