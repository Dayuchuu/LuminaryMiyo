using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    #region Variables

    private InputMap inputMap;

    private PlayerMovement playerMovement;

    private PlayerSwordAttack playerAttack;

    #endregion

    #region Methods
    
    /// <summary>
    /// Creates the inputMap, Gets the Player scripts. 
    /// </summary>
    private void Awake()
    {
        inputMap = new InputMap();

        playerMovement = GetComponent<PlayerMovement>();

        playerAttack = GetComponentInChildren<PlayerSwordAttack>();
    }

    /// <summary>
    /// Enables and subscribes Player actions.
    /// </summary>
    private void OnEnable()
    {
        inputMap.Enable();

        inputMap.Player.Movement.performed += playerMovement.Move;
        inputMap.Player.Movement.canceled += playerMovement.Move;

        inputMap.Player.Jump.performed += playerMovement.Jump;
        inputMap.Player.Jump.canceled += playerMovement.Jump;
        
        inputMap.Player.Dash.performed += playerMovement.Dash;

        // inputMap.Player.Attack.performed += playerAttack.Attack;

        inputMap.Player.StartMoving.performed += playerMovement.StartCountdown;

        inputMap.Player.PauseGame.performed += playerMovement.PauseGame;
    }

    /// <summary>   
    /// Disables and subscribes Player actions.
    /// </summary>
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
