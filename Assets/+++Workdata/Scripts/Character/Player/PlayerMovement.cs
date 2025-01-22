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
    
    [HideInInspector]
    public PlayerStates states = PlayerStates.Default;
    
    [Header("Movement Variables")]
    [SerializeField] private float maxDefaultMoveSpeed = 10f;
    [SerializeField] private int maxVelocityChange;
    [SerializeField] private float speedUpTimer = 0f;
    [SerializeField] private float moveSpeedAcceleration = 0f;
    [SerializeField] private float maxSlopeAngle;
    private Vector3 movementDirection = Vector3.zero;
    private float currentMoveSpeed;
    private float moveSpeed = 0f;
    public float speedUpCounter = 0f;
    [Space]
    
    [Header("Dash Variables")]
    [SerializeField] private float maxMoveSpeedDuringDash = 0f;
    [SerializeField] private float dashPower = 5f;
    [SerializeField] private float dashTimer = 0f;
    [SerializeField] private int dashAmount = 0;
    [SerializeField] private float dashCooldown = 0;
    private int currentDashAmount = 0;
    private float currentDashCooldown = 0;
    private bool canDash = true;
    [Space]
    
    [Header("Jump Variables")]
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private int jumpAmount = 0;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private Vector3 boxCastSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float numWaitFrames;
    private float waitFrames;
    private float coyoteTimeCounter = 0;
    private float jumpBufferCounter = 0;
    private int currentJumpAmount = 0;
    [Space]
    
    public bool disableMovement = true;
    
    private Rigidbody rb;
    
    [Header("Gravity Variables")]
    //changed to serialize field so i can see shit in the inspector
    [SerializeField] private float gravity = 0f;

    //rate by which the gravity gets reduced once falling
    [SerializeField] private float gravityReduction = 1f;
    [SerializeField] private float defaultGravity = -9.81f;
    [SerializeField] private float maxGravity = -30f;
    private float noGravity = 0f;
    [Space]
    
    [Header("Effect Variables")]
    [SerializeField] private ParticleSystem speedlines = new ParticleSystem();
    private AudioSource audioSource;

    [Space] 
    [Header("Animations")] 
    [SerializeField] private Animator anim;
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

    /// <summary>
    /// We get multiple values/components at the start.
    /// </summary>
    private void Start()
    {
        //Getting components
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        //Getting Values
        gravity = defaultGravity;
        moveSpeed = maxDefaultMoveSpeed;
        speedUpCounter = speedUpTimer;
        currentDashCooldown = dashCooldown;
        currentDashAmount = dashAmount;
        
        // if (UIManager.Instance.tutorialDialogue.GetComponentInChildren<Dialogue>().isPlaying)
        // {
        //     disableMovement = true;
        // }
        // else
        // {
        //     disableMovement = false;
        // }

        ChangeSettings();

        //This is to allow the Player to move after pressing replay.
        if (UIManager.Instance.tutorialDialogue.activeInHierarchy)
        {
            disableMovement = true;
            return;
        }

        if (UIManager.Instance.levelDialogue.activeInHierarchy)
        {
            disableMovement = true;
            return;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //Attack for the enemies
        if (states == PlayerStates.Dash && other.CompareTag("Enemy"))
        {
            other.GetComponent<CharacterBase>().healthPoints--;
        }

        //Death for the Player
        if (other.CompareTag("Death"))
        {
            UIManager.Instance.inGameUi.SetActive(false);
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

        waitFrames--;
        
        //Sets the coyote time / calculates when off ground 
        if (IsGrounded() && waitFrames <= 0f)
        {
            coyoteTimeCounter = coyoteTime;
            UIManager.Instance.jumpIndicator.color = Color.blue;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        if (IsGrounded() && currentDashAmount == 0)
        {
            currentDashAmount = 1;
        }

        if (!canDash)
        {
            currentDashCooldown -= Time.deltaTime;
            
            if (currentDashCooldown <= 0)
            {
                currentDashCooldown = dashCooldown;

                UIManager.Instance.dashIndicator.color = Color.yellow;
                
                canDash = true;
            }
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
        
        //changes velocity of the rigidbody.
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        
        //Calculates when to speed up the Player.
        if (rb.velocity.magnitude > maxDefaultMoveSpeed - 0.5f && states != PlayerStates.Dash)
        {
            speedUpCounter -= Time.deltaTime;
        }
        else
        {
            speedUpCounter = speedUpTimer;
        }

        //Speeds up the player over time when running for a while.
        if (speedUpCounter <= 0)
        {
            moveSpeed += Time.deltaTime * moveSpeedAcceleration;
            moveSpeed = Mathf.Clamp(moveSpeed ,0, 16);
            
            if (anim.GetBool("IsRunning"))
            {
                anim.speed = moveSpeed * 0.1f;
            }
            else
            {
                anim.speed = 1;
            }
            
            maxMoveSpeedDuringDash += Time.deltaTime * moveSpeedAcceleration;
            maxMoveSpeedDuringDash = Mathf.Clamp(maxMoveSpeedDuringDash ,20, 26);
        }

        Vector3 moveVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 jumpVelocity = new Vector3(0f, rb.velocity.y, 0f);
        
        //Changes the animation.
        if (moveVelocity.magnitude > 0.05f && jumpVelocity.magnitude < 0.05 && jumpVelocity.magnitude > -0.05f)
        {
            anim.SetBool( "IsRunning", true);
        }
        else
        {
            anim.SetBool( "IsRunning", false);
        }
        
        anim.SetFloat("JumpVelocity", rb.velocity.y);
    }
    
    // --- MOVE METHOD --- //
    public void Move(InputAction.CallbackContext context)
    {
        if (disableMovement) { return; }
        
        if (!UIManager.Instance.timer.countDownIsRunning && rb.velocity.magnitude > 0)
        {
            UIManager.Instance.StartCountdown();
        }

        if (SlopeMovement())
        {
            //Get the move values
            inputX = context.ReadValue<Vector3>().x;
            inputZ = context.ReadValue<Vector3>().z;
        }
    }
    
    // --- JUMP METHOD --- //
    public void Jump(InputAction.CallbackContext context)
    {
        if (disableMovement) { return; }
        
        speedlines.Stop();
        
        anim.SetBool( "DashAttack", false);

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
            waitFrames = numWaitFrames;
            audioSource.PlayOneShot(MusicManager.instance.jumpingSound);
            UIManager.Instance.jumpIndicator.color = Color.red;
        }
        //jump in air if Jump amount is larger than 0
        else if (jumpBufferCounter > 0 &&  currentJumpAmount > 0) 
        {
            rb.velocity = new Vector3(0f, jumpPower, 0f);

            //reduce jump amount
            currentJumpAmount--;
            UIManager.Instance.jumpIndicator.color = Color.white;
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
        
        audioSource.PlayOneShot(MusicManager.instance.playerAttack);

        currentMoveSpeed = moveSpeed;
        
        anim.SetBool( "DashAttack", true);
        
        //if on ground and not moving, move in direction of camera
        //checks if player is not moving by checking if the movement inputs return a value
        if (IsGrounded() && inputX == 0 && inputZ == 0 && canDash)
        {
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
        // Looks if the ground is beneath you and that the angle isnt to steep. 
        if (Physics.BoxCast(transform.position, boxCastSize, Vector3.down, out var hit, Quaternion.identity, boxCastSize.y, groundMask))
        {
           if(Vector2.Angle(hit.normal, Vector3.up) < 45) 
                return true;
        }
        return false;
    }

    
    private bool SlopeMovement()
    {
        //Calculates the angle of the ground to remove moving up slopes that are to high.
        if (Physics.BoxCast(transform.position, boxCastSize * 1.5f, Vector3.down, out var hit, Quaternion.identity, boxCastSize.y, groundMask))
        {
            float angle = Vector2.Angle(hit.normal, Vector3.up);
            if (angle > maxSlopeAngle)
            {
                return false;
            }
        }
        return true;
    }
    
    
    public void DisablePlayerActions()
    {
        //Disables the player Actions
        disableMovement = true;
        
        rb.velocity = Vector3.zero;
    }

    public void EnablePlayerActions()
    {
        //Enables the player Actions
        disableMovement = false;
    }
    
    /// <summary>
    /// Waits the given time to Dash.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForDash()
    {
        speedlines.Play();
        
        UIManager.Instance.dashIndicator.color = Color.white;
        
        yield return new WaitForSeconds(dashTimer);
        
        UIManager.Instance.dashIndicator.color = Color.yellow;
        
        anim.SetBool( "DashAttack", false);

        moveSpeed = currentMoveSpeed;
        
        states = PlayerStates.Default;
        
        speedlines.Stop();
        
        //set gravity back to default after dash is over
        gravity = defaultGravity;
        rb.useGravity = true;

        canDash = false;
        
        if (!IsGrounded())
        {
            currentJumpAmount = jumpAmount;
            currentDashAmount--;
        }
    }

    /// <summary>
    /// Changes the settings of the fov and the camera move sensitivity. 
    /// </summary>
    public void ChangeSettings()
    {
        cameraTransform.gameObject.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat(UIManager.fov, 90f);
        rotationSensibility = PlayerPrefs.GetFloat(UIManager.cameraSensibility, 0.2f);
    }

    /// <summary>
    /// Calls the Player steps.
    /// </summary>
    private void PlayerSteps()
    {
        audioSource.PlayOneShot(MusicManager.instance.playerSteps[Random.Range(0,MusicManager.instance.playerSteps.Length)]);
    }

    /// <summary>
    /// Pauses the game in game. 
    /// </summary>
    /// <param name="context"></param>
    public void PauseGame(InputAction.CallbackContext context)
    {
        UIManager.Instance.inGameUi.SetActive(false);
        UIManager.Instance.OpenMenu(UIManager.Instance.pauseScreen, CursorLockMode.None,  0);
    }

    #endregion
}