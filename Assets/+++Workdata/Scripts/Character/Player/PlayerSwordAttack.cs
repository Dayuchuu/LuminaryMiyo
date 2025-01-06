using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwordAttack : MonoBehaviour
{
   #region Variables

   [SerializeField] private float attackDistance = 0f;

   [SerializeField] private LayerMask attackLayer = 0;

   [SerializeField] private Transform cameraTransform = null;

   [SerializeField] private Vector3 boxCastSize = new Vector3();

   private Animator swordAnim = null;

   [SerializeField] private AudioSource swordAttackSound;
   
   #endregion

   #region Methods
   
   private void Awake()
   {
      swordAnim = GetComponent<Animator>();

      swordAttackSound = GetComponent<AudioSource>();
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Enemy"))
      {
         other.GetComponent<MeshRenderer>().material.color = Color.red;
         
         other.GetComponent<CharacterBase>().healthPoints--;
      }
   }

   public void Attack(InputAction.CallbackContext context)
   { 
      RaycastHit hit = new RaycastHit(); 
   
      swordAnim.SetTrigger("Attacks");
      
      swordAttackSound.Play();

      // Physics.BoxCast(cameraTransform.position, boxCastSize, cameraTransform.forward * 0.1f, out hit, Quaternion.identity);

      // if (hit.collider != null)
      // {
      //    Debug.Log(hit.collider.gameObject.name);
      // }
      
      // if (hit.collider != null && hit.collider.CompareTag("Enemy"))
      // {
      //    hit.collider.GetComponent<CharacterBase>().healthPoints--;
      //    
      //    hit.collider.GetComponent<MeshRenderer>().material.color = Color.red;
      // }
   }
   #endregion
} 