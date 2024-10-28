using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
   public int time = 0;

   [SerializeField] 
   private TextMeshProUGUI timeText;

   [SerializeField] 
   private GameObject gameOverScreen = null;

   [SerializeField] 
   private PlayerMovement player = null;
   
   private void Awake()
   {
      timeText.text = time.ToString();
      
      StartCoroutine(CountDown());
   }

   IEnumerator CountDown()
   {
      if (time <= 0)
      {
         gameOverScreen.SetActive(true);

         player.DisableMovement();
         
         Time.timeScale = 0;
         
         yield break;
      }
      
      yield return new WaitForSeconds(1);

      time--;

      timeText.text = time.ToString();

      StartCoroutine(CountDown());
   }
}
