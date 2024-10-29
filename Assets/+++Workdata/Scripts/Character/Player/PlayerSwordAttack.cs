using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwordAttack : MonoBehaviour
{
   [SerializeField] private float attackDistance = 0f;

   [SerializeField] private LayerMask attackLayer = 0;

   private Animator swordAnim = null;

   private RaycastHit hit;

   private void Awake()
   {
      swordAnim = GetComponent<Animator>();
   }

   public void Attack(InputAction.CallbackContext context)
   {
      swordAnim.SetTrigger("Attacks");
      
      if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, attackDistance, attackLayer))
      {
         GameObject target = hit.transform.gameObject;
         target.GetComponent<EnemyShooting>().healthPoints--;
         
         target.GetComponent<MeshRenderer>().material.color = Color.red;

         if (target.GetComponent<EnemyShooting>().healthPoints <= 0)
         {
            Destroy(target);
         }
      }
   }

   private void Update()
   {
      Debug.DrawRay(transform.position, transform.forward * attackDistance);
   }
} 
