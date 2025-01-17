using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
   #region Variables

   public int time = 0;
   
   private TextMeshProUGUI timeText;

   public bool countDownIsRunning;

   #endregion

   #region Methods
   
   public void StartCountdown()
   {
      timeText = UIManager.Instance.timeText;
      
      timeText.text = time.ToString();

      if (GameController.Instance.gameStates == GameController.GameStates.InGame)
      {
         timeText.gameObject.SetActive(true);
      }
   }

   public IEnumerator CountDown()
   {
      countDownIsRunning = true;
      
      if (time <= 0)
      {
         countDownIsRunning = false;
         UIManager.Instance.OpenMenu(UIManager.Instance.loseScreen, CursorLockMode.None, 0f);
         
         yield break;
      }
      
      yield return new WaitForSeconds(1);

      time--;

      timeText.text = time.ToString();

      StartCoroutine(CountDown());
   }

   public void ResetTimer()
   {
      StopAllCoroutines();
      time = 60;
      timeText.text = time.ToString();
      countDownIsRunning = false;
   }
   #endregion
}
