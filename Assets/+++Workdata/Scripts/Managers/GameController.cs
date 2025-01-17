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

    public void FinishedLevel01()
    {
        PlayerPrefs.SetInt(level01, Convert.ToInt32(level01Finished));
        UIManager.Instance.level02.interactable = true;
    }
    
    public void FinishedLevel02()
    {
        PlayerPrefs.SetInt(level01, Convert.ToInt32(level02Finished));
    }

    public void ResetGameStats()
    {
        currentPoints = 0;
    }

    public void ChangeScore(int currentAdd)
    {
        currentPoints += currentAdd;
        UIManager.Instance.scoreText.text = currentPoints.ToString();
    }
    
    #endregion
}
