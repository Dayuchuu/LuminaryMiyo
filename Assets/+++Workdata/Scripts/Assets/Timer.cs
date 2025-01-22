using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
   #region Variables

   public float time = 0;
   private float milliseconds;
   
   private TextMeshProUGUI timeText;

   public bool countDownIsRunning;

   public bool startCountDown;
   

   #endregion

   #region Methods
   
   /// <summary>
   /// Starts the countdown of the timer. 
   /// </summary>
   public void StartCountdown()
   {
      timeText = UIManager.Instance.timeText;
      
      timeText.text = string.Format("{0}.{1}", 60, 00);

      if (GameController.Instance.gameStates == GameController.GameStates.InGame)
      {
         timeText.gameObject.SetActive(true);
      }
   }

   /// <summary>
   /// Calcuates and formats the timer.
   /// </summary>
   public void Update()
   {
      if (!startCountDown)
      {
         return;
      }
      
      countDownIsRunning = true;

      if(milliseconds <= 0)
      {
         time--;
         if(time <= 0)
         {
            time = (int)0;
            UIManager.Instance.inGameUi.SetActive(false);
            UIManager.Instance.OpenMenu(UIManager.Instance.loseScreen, CursorLockMode.None, 0f);
         }
         
         milliseconds = 100;
      }
    		
      milliseconds -= Time.deltaTime * 100;
      
      timeText.text = string.Format("{0}.{1}", time, (int)milliseconds);
   }

   /// <summary>
   /// Resets the format and time. 
   /// </summary>
   public void ResetTimer()
   {
      time = 90;
      startCountDown = false;
      timeText.text = string.Format("{0}.{1}", 60, 00);
      countDownIsRunning = false;
   }
   #endregion
}
