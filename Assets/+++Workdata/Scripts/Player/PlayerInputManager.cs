using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private InputMap inputMap;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        inputMap = new InputMap();

        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        inputMap.Enable();

        inputMap.Player.Movement.performed += playerMovement.Move;
        inputMap.Player.Movement.canceled += playerMovement.Move;

        inputMap.Player.Jump.performed += playerMovement.Jump;
        
        inputMap.Player.Dash.performed += playerMovement.Dash;
    }

    private void OnDisable()
    {
        inputMap.Disable();
        
        inputMap.Player.Movement.performed -= playerMovement.Move;
        inputMap.Player.Movement.canceled -= playerMovement.Move;
        
        inputMap.Player.Jump.performed -= playerMovement.Jump;

        inputMap.Player.Dash.performed -= playerMovement.Dash;
    }
}
