using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwordAttack : MonoBehaviour
{
   #region Variables

   [SerializeField] private float attackDistance = 0f;

   [SerializeField] private LayerMask attackLayer = 0;

   [SerializeField] private Transform cameraTransform = null;

   private Animator swordAnim = null;
   
   #endregion

   #region Methods
   
   private void Awake()
   {
      swordAnim = GetComponent<Animator>();
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Enemy"))
      {
         other.GetComponent<CharacterBase>().healthPoints--;
         
         other.GetComponent<MeshRenderer>().material.color = Color.red;
         
         if (other.GetComponent<EnemyShooting>().healthPoints <= 0)
         {
            other.gameObject.gameObject.SetActive(false);
         }
      }
   }

   public void Attack(InputAction.CallbackContext context)
   {
      swordAnim.SetTrigger("Attacks");
   }

   private void Update()
   {
      Debug.DrawRay(transform.parent.position, transform.parent.forward * attackDistance);
   }
   #endregion
} 