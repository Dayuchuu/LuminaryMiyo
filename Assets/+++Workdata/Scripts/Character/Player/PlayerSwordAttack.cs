using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSwordAttack : MonoBehaviour
{
   [SerializeField] private float attackDistance = 0f;

   [SerializeField] private LayerMask attackLayer = 0;
} 
