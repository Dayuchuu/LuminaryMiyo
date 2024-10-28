using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Variables

    public static GameController Instance;

    public GameObject GameOverScreen = null;

    public PlayerMovement player = null;
    
    public int coinPoints = 0;

    public int timePoints = 0;
    
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            DontDestroyOnLoad(this);
        }
    }

    public void ShowGameOverScreen()
    {
        GameOverScreen.SetActive(true);
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        
        player.DisablePlayerActions();

        Time.timeScale = 0f;
    }
}
