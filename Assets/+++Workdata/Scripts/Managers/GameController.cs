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
    
    public GameStates gameStates;
    
    public int coinPoints = 0;
    public int timePoints = 0;
    
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

    public void ResetGameStats()
    {
        coinPoints = 0;
        timePoints = 0;
    }
    
    #endregion
}
