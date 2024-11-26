using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
   #region Variables

   public int time = 0;
   
   private TextMeshProUGUI timeText;

   #endregion

   #region Methods
   
   private void Start()
   {
      timeText = UIManager.Instance.timeText;
      
      timeText.text = time.ToString();

      if (GameController.Instance.gameStates == GameController.GameStates.Level)
      {
         timeText.gameObject.SetActive(true);
      }
      
      StartCoroutine(CountDown());
   }

   IEnumerator CountDown()
   {
      if (time <= 0)
      {
        UIManager.Instance.OpenMenu(UIManager.Instance.loseScreen, 0f);
         
         yield break;
      }
      
      yield return new WaitForSeconds(1);

      time--;

      GameController.Instance.timePoints = time;

      timeText.text = time.ToString();

      StartCoroutine(CountDown());
   }
   #endregion
}
