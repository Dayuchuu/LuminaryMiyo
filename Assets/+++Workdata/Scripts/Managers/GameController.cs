using System;
using System.Collections;
using System.Collections.Generic;
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

    public PlayerMovement player = null;
    
    public int coinPoints = 0;

    public int timePoints = 0;

    public GameStates gameStates;
    
    #endregion

    #region Methods
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void ResetGameStats()
    {
        coinPoints = 0;
        timePoints = 0;
    }
    
    #endregion
}
