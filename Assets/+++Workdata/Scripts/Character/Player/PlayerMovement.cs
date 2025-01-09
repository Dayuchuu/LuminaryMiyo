using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : CharacterBase
{
    #region Definitions

    public enum PlayerStates
    {
        Default,
        Dash
    }
    #endregion
    
    #region Variables

    #region PlayerValues

    [Header("Player Variables")]

    #region Movement Variables
    
    public PlayerStates states = PlayerStates.Default;
    
    [SerializeField] private float maxDefaultMoveSpeed = 10f;
    [SerializeField] private int maxVelocityChange;

    [SerializeField] private float maxMoveSpeedDuringDash = 0f;
    public float currentMoveSpeed;
    
    [SerializeField] private float dashPower = 5f;
    [SerializeField] private float dashTimer = 0f;
    [SerializeField] private int dashAmount = 0;
    [SerializeField] private int currentDashAmount = 0;
    [SerializeField] private float dashCooldown = 0;
    [SerializeField] private float currentDashCooldown = 0;
    private bool canDash = true;
    
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float defaultGravity = -9.81f;
    [SerializeField] private float fallingGravity = -9.81f;
    [SerializeField] private float groundDistance = 0f;
    [SerializeField] private int jumpAmount = 0;

    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [SerializeField] private Vector3 boxCastSize;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float speedUpTimer = 0f;
    public float speedUpCounter = 0f;
    
    [SerializeField] private int currentJumpAmount = 0;
    private float noGravity = 0f;

    [SerializeField] private float moveSpeedAcceleration = 0f;

    [SerializeField] private ParticleSystem speedlines = new ParticleSystem();

    [Space]
    public bool disableMovement = false;
    
    private Rigidbody rb;
    
    [SerializeField] private Vector3 movementDirection = Vector3.zero;
    
    private float moveSpeed = 0f;


    [Header("Gravity Stuff")]
    //changed to serialize field so i can see shit in the inspector
    [SerializeField] private float gravity = 0f;

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
        speedUpCounter = speedUpTimer;
    }

    private void Start()
    {
        currentDashAmount = dashAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (states == PlayerStates.Dash && other.CompareTag("Enemy"))
        {
            other.GetComponent<CharacterBase>().healthPoints--;
        }

        if (other.CompareTag("Death"))
        {
            UIManager.Instance.OpenMenu(UIManager.Instance.loseScreen, CursorLockMode.None, 0f);
        }
    }

    private void Update()
    {
        if (disableMovement) { return; }
        
        if (states == PlayerStates.Dash)
        {
            moveSpeed = maxMoveSpeedDuringDash;
        }
        else if(rb.velocity.magnitude < 0.5f)
        {
            moveSpeed = maxDefaultMoveSpeed;
        }
        
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        //inverts the vertical camera movement
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
        if (rb.velocity.y < 0f && !IsGrounded() && states != PlayerStates.Dash)
        {
            //if the character is falling slowly increase gravity over time
            if (gravity > maxGravity)
            {
                gravity -= gravityReduction * Time.deltaTime;
            }
        }
        else
        {
            //if the character isn't falling set gravity back to normal
            gravity = defaultGravity;
        }

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        if (IsGrounded() && currentDashAmount == 0)
        {
            currentDashAmount = 1;
        }
    }
    
    private void FixedUpdate()
    {
        if (disableMovement) { return; }
        
        if (states == PlayerStates.Dash)
        {
            gravity = noGravity;
            rb.useGravity = false;
            rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
            return;
        }
        
        Vector3 targetVelocity = new Vector3(inputX, 0f, inputZ);
        
        targetVelocity = Quaternion.AngleAxis(cameraTransform.localEulerAngles.y, Vector3.up) * targetVelocity;
        
        rb.AddForce(0f, gravity,0f, ForceMode.Acceleration);
        
        targetVelocity = transform.TransformDirection(targetVelocity) * moveSpeed;
        
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        
        //changes velocity of the rigidbody, e.g. moves the player
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        
        if (rb.velocity.magnitude > maxDefaultMoveSpeed - 0.5f && states != PlayerStates.Dash)
        {
            speedUpCounter -= Time.deltaTime;
        }
        else
        {
            speedUpCounter = speedUpTimer;
        }

        if (speedUpCounter <= 0)
        {
            moveSpeed += Time.deltaTime * moveSpeedAcceleration;
            moveSpeed = Mathf.Clamp(moveSpeed ,0, 16);
            
            maxMoveSpeedDuringDash += Time.deltaTime * moveSpeedAcceleration;
            maxMoveSpeedDuringDash = Mathf.Clamp(maxMoveSpeedDuringDash ,20, 26);
        }
    }
    
    // --- MOVE METHOD --- //
    public void Move(InputAction.CallbackContext context)
    {
        if (disableMovement) { return; }
        
        inputX = context.ReadValue<Vector3>().x;
        inputZ = context.ReadValue<Vector3>().z;
    }
    
    // --- JUMP METHOD --- //
    public void Jump(InputAction.CallbackContext context)
    {
        if (disableMovement) { return; }
        
        speedlines.Stop();

        if (states == PlayerStates.Dash)
        {
            moveSpeed = currentMoveSpeed;
        }

        states = PlayerStates.Default;
        StopAllCoroutines();
        gravity = defaultGravity;
        rb.useGravity = true;

        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.time;
        }

        //jump on ground
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.velocity = new Vector3(0f, jumpPower, 0f);

            //set gravity to default once grounded
            gravity = defaultGravity;
            currentDashAmount = dashAmount;
            jumpBufferCounter = 0;
        }
        //jump in air if Jump amount is larger than 0
        else if (jumpBufferCounter > 0 &&  currentJumpAmount > 0) 
        {
            rb.velocity = new Vector3(0f, jumpPower, 0f);

            //reduce jump amount
            currentJumpAmount--;
        }

        //jump button released
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);
            coyoteTimeCounter = 0;
        }
    }
    
    // --- DASH METHOD --- //
    public void Dash(InputAction.CallbackContext context)
    {
        if (disableMovement) { return; }
        
        if (states == PlayerStates.Dash) { return; }

        currentMoveSpeed = moveSpeed;
        
        //if on ground and not moving, move in direction of camera
        //checks if player is not moving by checking if the movement inputs return a value
        if (IsGrounded() && inputX == 0 && inputZ == 0 && canDash)
        {
            Debug.Log("on ground and not moving");
            states = PlayerStates.Dash;
            rb.velocity = Vector3.zero;

            movementDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            movementDirection.Normalize();

            rb.AddForce(cameraTransform.forward * dashPower, ForceMode.VelocityChange);
            
            
            StartCoroutine(WaitForDash());
        }
        //if on ground and moving, move in current movement direction 
        else if (IsGrounded() && rb.velocity != Vector3.zero && canDash)
        {
            Debug.Log("on ground and moving");
            states = PlayerStates.Dash;

            movementDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            movementDirection.Normalize();
            rb.AddForce(movementDirection * dashPower, ForceMode.VelocityChange);
            
            StartCoroutine(WaitForDash());
        }
        //if in air and not moving, move in direction of camera
        //checks if player is not moving by checking if the movement inputs return a value
        else if (currentDashAmount > 0 && !IsGrounded() && inputX == 0 && inputZ == 0)
        {
            Debug.Log("in air and not moving");
            states = PlayerStates.Dash;
            
            rb.velocity = Vector3.zero;

            rb.AddForce(cameraTransform.forward * dashPower, ForceMode.VelocityChange);
            
            currentDashAmount--;
            currentJumpAmount = jumpAmount;
            StartCoroutine(WaitForDash());
        }
        //if in air and moving, move in current movement direction 
        else if (currentDashAmount > 0 && !IsGrounded() && rb.velocity != Vector3.zero)
        {
            Debug.Log("in air and moving");
            states = PlayerStates.Dash;

            movementDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            movementDirection.Normalize();
            rb.AddForce(movementDirection * dashPower, ForceMode.VelocityChange);

            currentDashAmount--;
            currentJumpAmount = jumpAmount;
            StartCoroutine(WaitForDash());
        }
    }
    
    private bool IsGrounded()
    {        
        if (Physics.BoxCast(transform.position, boxCastSize, Vector3.down, Quaternion.identity, boxCastSize.y, groundMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, boxCastSize);
        Gizmos.DrawLine(cameraTransform.position, cameraTransform.forward);
    }
    
    public void DisablePlayerActions()
    {
        disableMovement = true;
        
        rb.velocity = Vector3.zero;
    }

    public void EnablePlayerActions()
    {
        disableMovement = false;
    }
    
    private IEnumerator WaitForDash()
    {
        if (!canDash) { yield break; }
        
        speedlines.Play();
        
        yield return new WaitForSeconds(dashTimer);

        moveSpeed = currentMoveSpeed;
        
        states = PlayerStates.Default;
        
        speedlines.Stop();
        
        //set gravity back to default after dash is over
        gravity = defaultGravity;
        rb.useGravity = true;
        
        // StartDashCooldown();
        
        if (!IsGrounded())
        {
            currentJumpAmount = jumpAmount;
            currentDashAmount--;
        }
    }
    
    private void StartDashCooldown()
    {
        currentDashCooldown = dashCooldown;
        
        canDash = false;

        while (currentDashCooldown > 0)
        {
            currentDashCooldown -= Time.deltaTime;
        }

        canDash = true;
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        UIManager.Instance.OpenMenu(UIManager.Instance.pauseScreen, CursorLockMode.None,  0);
    }

    #endregion
}