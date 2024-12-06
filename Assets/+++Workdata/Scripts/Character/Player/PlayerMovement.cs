using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : CharacterBase
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

    #region Movement Variables
    
    [SerializeField]
    private PlayerStates states =  PlayerStates.Default;
    
    [SerializeField] private float maxDefaultMoveSpeed = 10f;
    [SerializeField] private int maxVelocityChange;

    [SerializeField] private float maxMoveSpeedDuringDash = 0f;
    [SerializeField] private float dashPower = 5f;
    [SerializeField] private float dashTimer = 0f;
    [SerializeField] private int dashAmount = 0;
    [SerializeField] private int currentDashAmount = 0;
    private bool canDash = true;
    
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float defaultGravity = -9.81f;
    [SerializeField] private float fallingGravity = -9.81f;
    [SerializeField] private float groundDistance = 0f;
    [SerializeField] private int jumpAmount = 0;
    [SerializeField] private LayerMask groundMask;
    private int currentJumpAmount = 0;
    private float noGravity = 0f;

    

    [SerializeField] private AnimationCurve animationCurve;
    
    [Space]
    public bool disabled = false;
    
    private Rigidbody rb;
    
    private Vector3 movementDirection = Vector3.zero;
    
    private float moveSpeed = 0f;


    [Header("Gravity Stuff")]
    //changed to serialize field so i can see shit in the inspector
    [SerializeField] private float gravity = 0f;
    //added to test falling state in inspector, practically useless
    [SerializeField] private bool falling = false;
    //rate by which the gravity gets reduced once falling
    [SerializeField] private float gravityReduction = 1f;
    [SerializeField] private float maxGravity = -30f;

    [Space]

    private float inputX;
    
    private float inputZ;
    #endregion
    
    #endregion

    #region CameraValues
    
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

    private void Start()
    {
        currentDashAmount = dashAmount;
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



        //increases gravity over time once the player starts falling
        if (rb.velocity.y < 0f && !IsGrounded())
        {
            falling = true;
            if (gravity > maxGravity)
            {
                gravity -= gravityReduction * Time.deltaTime;
            }
        }
        else
        {
            falling = false;
            gravity = defaultGravity;
        }
    }

    private void FixedUpdate()
    {
        if (disabled) { return; }
        
        if (states == PlayerStates.Dash)
        {
            gravity = noGravity;
        }
        else
        {
            //gravity = defaultGravity;
        }

        if ( rb.velocity.y >= 15f)
        {
            //gravity = animationCurve.Evaluate()
        }
        
        
        Vector3 targetVelocity = new Vector3(inputX, 0f, inputZ);
        
        targetVelocity = Quaternion.AngleAxis(cameraTransform.localEulerAngles.y, Vector3.up) * targetVelocity;
        
        rb.AddForce(0f, gravity,0f, ForceMode.Acceleration);
        
        targetVelocity = transform.TransformDirection(targetVelocity) * moveSpeed;
        
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

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

        if (!IsGrounded() && currentJumpAmount > 0 && context.performed)
        {
            Vector3 jumpVector = new Vector3(0f, jumpPower, 0f);

            rb.velocity = jumpVector;

            currentJumpAmount--;
        }
        else if (IsGrounded())
        {
            Vector3 jumpVector = new Vector3(0f, jumpPower, 0f);

            currentDashAmount = dashAmount;
            
            rb.velocity = jumpVector;

            //set gravity to default once grounded
            gravity = defaultGravity;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);
        }
    }



    public void Dash(InputAction.CallbackContext context)
    {
        if (disabled) { return; }
        
        if (states == PlayerStates.Dash) { return; }

        if (IsGrounded())
        {
            states = PlayerStates.Dash;
        
            movementDirection = cameraTransform.forward * dashPower;
            
            rb.velocity = movementDirection;
            
            StartCoroutine(WaitForDash());
        }
        else if (currentDashAmount > 0 && !IsGrounded())
        { 
            states = PlayerStates.Dash;
        
            movementDirection = cameraTransform.forward * dashPower;
            
            rb.velocity = movementDirection;

            currentDashAmount--;
            
            currentJumpAmount = jumpAmount;

            StartCoroutine(WaitForDash());
        }
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
        if (!canDash) { yield break; }
        
        yield return new WaitForSeconds(dashTimer);
        
        states = PlayerStates.Default;

        //set gravity back to default after dash is over
        gravity = defaultGravity;
    }

    #endregion
}