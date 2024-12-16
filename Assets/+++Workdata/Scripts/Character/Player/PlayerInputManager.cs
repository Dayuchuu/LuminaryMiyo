using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    #region Variables

    private InputMap inputMap;

    private PlayerMovement playerMovement;

    private PlayerSwordAttack playerAttack;

    #endregion

    #region Methods
    
    private void Awake()
    {
        inputMap = new InputMap();

        playerMovement = GetComponent<PlayerMovement>();

        playerAttack = GetComponentInChildren<PlayerSwordAttack>();
    }

    private void OnEnable()
    {
        inputMap.Enable();

        inputMap.Player.Movement.performed += playerMovement.Move;
        inputMap.Player.Movement.canceled += playerMovement.Move;

        inputMap.Player.Jump.performed += playerMovement.Jump;
        inputMap.Player.Jump.canceled += playerMovement.Jump;
        
        inputMap.Player.Dash.performed += playerMovement.Dash;

        inputMap.Player.Attack.performed += playerAttack.Attack;

        inputMap.Player.PauseGame.performed += playerMovement.PauseGame;
    }

    private void OnDisable()
    {
        inputMap.Disable();
        
        inputMap.Player.Movement.performed -= playerMovement.Move;
        inputMap.Player.Movement.canceled -= playerMovement.Move;
        
        inputMap.Player.Jump.performed -= playerMovement.Jump;
        inputMap.Player.Jump.canceled -= playerMovement.Jump;

        inputMap.Player.Dash.performed -= playerMovement.Dash;
        
        inputMap.Player.PauseGame.performed -= playerMovement.PauseGame;
    }
    #endregion
}
