using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    #region Definitions

    private enum PlayerStates
    {
        Default,
        Dash
    }
    #endregion
    
    #region Variables

    #region PlayerValues

    [Header("Player Variables")]

    #region Movement Varaibles
    
    [SerializeField]
    private PlayerStates states =  PlayerStates.Default;
    
    [SerializeField] 
    private float acceleration = 5f;
    
    [SerializeField] 
    private float deceleration = 5f;
    
    [SerializeField] 
    private float maxDefaultMoveSpeed = 10f;

    [SerializeField] 
    private float maxMoveSpeedDuringDash = 0f;

    [SerializeField] 
    private float jumpPower = 10f;

    [SerializeField] 
    private float dashPower = 5f;
    
    [SerializeField] 
    private float defaultGravity = -9.81f;
    
    [SerializeField] 
    private float fallingGravity = -9.81f;

    [SerializeField] 
    private float groundDistance = 0f;

    [SerializeField]
    private float dashTimer = 0f;

    private float noGravity = 0f;
    
    [Space]
    public bool disabled;
    
    [SerializeField]
    private LayerMask groundMask;
    
    private Rigidbody rb;
    
    private Vector3 movementDirection = Vector3.zero;
    
    private float moveSpeed = 0f;
    
    private float gravity = 0f;
    
    private float inputX;
    
    private float inputZ;
    #endregion

    #region Health Variables
    
    public int healthPoints = 0;
    
    #endregion
    
    #endregion

    #region CamerValues
    
    [Header("Camera Variables")]
    [SerializeField] private Transform cameraTransform;
    
    [Range(0, 1)]
    [SerializeField] private float rotationSensibility;

    [SerializeField] private float maxCameraPitch = 80f;

    [SerializeField] private bool invertCameraPitch = false;
    
    private float cameraPitch;
    private float cameraRoll;
    #endregion
    
    #endregion 

    #region Methods
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        gravity = defaultGravity;

        moveSpeed = maxDefaultMoveSpeed;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (disabled) { return; }
        
        if (states == PlayerStates.Dash)
        {
            moveSpeed = maxMoveSpeedDuringDash;
        }
        else
        {
            moveSpeed = maxDefaultMoveSpeed;
        }
        
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        if(invertCameraPitch)
        {
            cameraPitch -= mouseDelta.y * rotationSensibility;
        }
        else
        {
            cameraPitch += mouseDelta.y * rotationSensibility;
        }
        
        cameraPitch = Mathf.Clamp(cameraPitch, -maxCameraPitch, maxCameraPitch);

        cameraRoll += mouseDelta.x * rotationSensibility;

        cameraTransform.localEulerAngles = new Vector3(cameraPitch, cameraRoll, 0f);
    }

    private void FixedUpdate()
    {
        if (disabled) { return; }
        
        if (states == PlayerStates.Dash)
        {
            gravity = noGravity;
        }
        
        if (rb.velocity.y < -0.2f)
        {
            gravity = fallingGravity;
        }
        else
        {
            gravity = defaultGravity;
        }
        
        rb.AddForce(0f, gravity,0f, ForceMode.Acceleration);

        movementDirection = new Vector3(inputX * acceleration, 0f, inputZ * acceleration);

        movementDirection = Quaternion.AngleAxis(cameraTransform.localEulerAngles.y, Vector3.up) * movementDirection;

        if (movementDirection != Vector3.zero)
        {
            rb.AddForce(movementDirection, ForceMode.Acceleration);

            Vector3 magnitudeVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (magnitudeVelocity.magnitude > moveSpeed)
            {
                rb.AddForce(magnitudeVelocity * -deceleration, ForceMode.Acceleration);
            }
        }
        else
        {
            Vector3 magnitudeVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            
            rb.AddForce(magnitudeVelocity * -deceleration, ForceMode.Acceleration);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (disabled) { return; }
        
        inputX = context.ReadValue<Vector3>().x;
        inputZ = context.ReadValue<Vector3>().z;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (disabled) { return; }
        
        if (!IsGrounded()) { return; }
        
        Vector3 jumpVector = new Vector3(0f, jumpPower, 0f);
        
        rb.AddForce(jumpVector, ForceMode.VelocityChange);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (disabled) { return; }
        
        if (states == PlayerStates.Dash) { return; }
        
        states = PlayerStates.Dash;
        
        movementDirection = new Vector3(inputX * acceleration, 0f, inputZ * acceleration);

        movementDirection = Quaternion.AngleAxis(cameraTransform.localEulerAngles.y, Vector3.up) * movementDirection;
        
        movementDirection *= 2;
        
        rb.velocity = movementDirection;

        StartCoroutine(WaitForDash());
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (disabled) { return; }

        
    }

    private bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void DisablePlayerActions()
    {
        disabled = true;
        
        rb.velocity = Vector3.zero;
    }

    public void EnablePlayerActions()
    {
        disabled = false;
    }

    private IEnumerator WaitForDash()
    {
        yield return new WaitForSeconds(dashTimer);
        
        states = PlayerStates.Default;
    }

    #endregion
}
