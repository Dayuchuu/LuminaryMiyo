using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwordAttack : MonoBehaviour
{
   #region Variables
   
   [SerializeField] private LayerMask attackLayer = 0;

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
      swordAnim.SetTrigger("Attacks");
      
      swordAttackSound.Play();
   }
   #endregion
} 