using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwordAttack : MonoBehaviour
{
   #region Variables

   [SerializeField] private float attackDistance = 0f;

   [SerializeField] private LayerMask attackLayer = 0;

   private Animator swordAnim = null;

   private RaycastHit hit;
   
   #endregion

   #region Methods
   
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
            target.SetActive(false);
         }
      }
   }

   private void Update()
   {
      Debug.DrawRay(transform.position, transform.forward * attackDistance);
   }
   #endregion
} 