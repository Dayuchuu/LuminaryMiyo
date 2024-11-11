using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
   #region Variables

   public int time = 0;

   [SerializeField] 
   private TextMeshProUGUI timeText;

   [SerializeField] 
   private PlayerMovement player = null;

   #endregion

   #region Methods
   
   private void Awake()
   {
      timeText.text = time.ToString();
      
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
