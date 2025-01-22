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
   
   /// <summary>
   /// Gets components.
   /// </summary>
   private void Awake()
   {
      swordAnim = GetComponent<Animator>();

      swordAttackSound = GetComponent<AudioSource>();
   }

   /// <summary>
   /// Changes enemies color and damages them.
   /// </summary>
   /// <param name="other"></param>
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Enemy"))
      {
         other.GetComponent<MeshRenderer>().material.color = Color.red;
         
         other.GetComponent<CharacterBase>().healthPoints--;
      }
   }

   /// <summary>
   /// Plays the attack animation and a sound effect. 
   /// </summary>
   /// <param name="context"></param>
   public void Attack(InputAction.CallbackContext context)
   { 
      swordAnim.SetTrigger("Attacks");
      
      swordAttackSound.Play();
   }
   #endregion
} 