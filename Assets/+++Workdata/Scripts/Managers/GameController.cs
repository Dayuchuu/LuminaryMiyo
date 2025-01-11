using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Definitons

    public enum GameStates
    {
        MainMenu,
        Level
    }
    
    #endregion
    
    #region Variables

    public static GameController Instance;

    public const string level01 = "level01";
    
    public GameStates gameStates;
    
    public int coinPoints = 0;
    public int timePoints = 0;

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

// #if UNITY_EDITOR
//         PlayerPrefs.DeleteAll();
// #endif
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
        coinPoints = 0;
        timePoints = 0;
    }
    
    #endregion
}
