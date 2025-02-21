using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

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

    public bool slopeCheck = false;

    [Space]
    
    [Header("Dash Variables")]
    [SerializeField] private float maxMoveSpeedDuringDash = 0f;
    [SerializeField] private float dashPower = 5f;
    [SerializeField] private float dashTimer = 0f;
    [SerializeField] private int dashAmount = 0;
    [SerializeField] private float dashCooldown = 0;
    private int currentDashAmount = 0;
    private float currentDashCooldown = 0;
    [SerializeField] private bool canDash = true;
    [Space]
    
    [Header("Jump Variables")]
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private int jumpAmount = 0;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private Vector3 boxCastSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float numWaitFrames;
    private float waitFrames;
    private float coyoteTimeCounter = 0;
    private int currentJumpAmount = 0;
    [Space]
    
    public bool disableMovement = true;
    private bool movementKeyPressed = false;

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
    [SerializeField] private Material jumpIndicator;
    [SerializeField] private Material dashIndicator;
    [SerializeField] private Material swordIndicator;
    [SerializeField] private SkinnedMeshRenderer handRenderer;
    private AudioSource audioSource;
    public GameObject localVolume;

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
        
        /*jumpIndicator = new Material(jumpIndicator);
        dashIndicator = new Material(dashIndicator);
        swordIndicator = new Material(swordIndicator);*/

        // if (UIManager.Instance.tutorialDialogue.GetComponentInChildren<Dialogue>().isPlaying)
        // {
        //     disableMovement = true;
        // }
        // else
        // {
        //     disableMovement = false;
        // }
        
        // dashIndicator.SetColor("_EmissionColor", Color.yellow * 10);
        // jumpIndicator.SetColor("_EmissionColor", Color.blue * 10);

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
            // jumpIndicator.SetColor("_EmissionColor", Color.blue);
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

                // dashIndicator.SetColor("_EmissionColor",Color.yellow);
                
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


        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (IsGrounded())
        {
            currentDashAmount = dashAmount;
            currentJumpAmount = 1;
        }
        else if (!IsGrounded() && currentDashAmount > 0)
        {
            currentJumpAmount = 0;
        }
        else if (!IsGrounded() && currentDashAmount == 0)
        {
            jumpAmount = 1;
        }
        
        if (currentJumpAmount > 0)
        {
            jumpIndicator.SetColor("_EmissionColor", Color.yellow * 10);          
        }
        else
        {
            jumpIndicator.SetColor("_EmissionColor", Color.black * 10);
        }

        if (currentDashAmount < 1)
        {
            dashIndicator.SetColor("_EmissionColor", Color.black * 10);
        }
        else
        {
            dashIndicator.SetColor("_EmissionColor", Color.yellow * 10);
        }

        if (moveSpeed >= 15)
        {
            swordIndicator.SetColor("_EmissionColor", Color.yellow * 10);
        }
        else
        {
            swordIndicator.SetColor("_EmissionColor", Color.black * 10);
        }
        
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
        if (IsGrounded() && states == PlayerStates.Default && movementKeyPressed)
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

        //Get the move values
        inputX = context.ReadValue<Vector3>().x;
        inputZ = context.ReadValue<Vector3>().z;


        if (inputX != 0 || inputZ != 0)
        {
            movementKeyPressed = true;
        }
        else
        {
            movementKeyPressed = false;
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
        
        //jump on ground
        if (context.performed && IsGrounded() && coyoteTimeCounter > 0)
        {
            rb.velocity = new Vector3(0f, jumpPower, 0f);

            //set gravity to default once grounded
            gravity = defaultGravity;
            currentDashAmount = dashAmount;
            waitFrames = numWaitFrames;
            audioSource.PlayOneShot(MusicManager.instance.jumpingSound);
            currentJumpAmount = 0;
            // jumpIndicator.SetColor("_EmissionColor", Color.red);
        }
        //jump in air if Jump amount is larger than 0
        else if (context.performed && !IsGrounded() &&  currentJumpAmount > 0) 
        {
            rb.velocity = new Vector3(0f, jumpPower, 0f);

            //reduce jump amount
            currentJumpAmount = 0;
            // jumpIndicator.SetColor("_EmissionColor", Color.white);

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
        

        if (currentDashAmount > 0 && canDash)
        {
            audioSource.PlayOneShot(MusicManager.instance.playerAttack);
            anim.SetBool("DashAttack", true);
        }
        
        currentMoveSpeed = moveSpeed;
        
        //anim.SetBool( "DashAttack", true);
        
        //if on ground and not moving, move in direction of camera
        //checks if player is not moving by checking if the movement inputs return a value
        if (IsGrounded() && inputX == 0 && inputZ == 0 && canDash)
        {
            states = PlayerStates.Dash;
            rb.velocity = Vector3.zero;

            movementDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            movementDirection.Normalize();

            rb.AddForce(cameraTransform.forward * dashPower, ForceMode.VelocityChange);

            canDash = false;
            StartCoroutine(WaitForDash());


        }
        //if on ground and moving, move in current movement direction 
        else if (IsGrounded() && rb.velocity != Vector3.zero && canDash)
        {
            states = PlayerStates.Dash;

            movementDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            movementDirection.Normalize();
            rb.AddForce(movementDirection * dashPower, ForceMode.VelocityChange);

            canDash = false;
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
            currentJumpAmount = 1;
            canDash = false;
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
            canDash = false;
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
        if (Physics.Raycast(transform.position, new Vector3(rb.velocity.x, 0, rb.velocity.z),out var hit, 2f, groundMask))
        {
            //float angle = Vector2.Angle(hit.normal, Vector3.up);
            /*if (angle > maxSlopeAngle)
            {
                return false;
            }*/
            return false;
        }
        return true;
    }

    
    private float CheckAngle()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(rb.velocity.x, 0, rb.velocity.z) * 10 , Color.red, 0.2f);
        if (Physics.Raycast(transform.position, new Vector3(rb.velocity.x, 0, rb.velocity.z),out var hit, 3f, groundMask))
        {
            float angle = Vector2.Angle(hit.normal, Vector3.up);
            return angle;
        }
    
        if (hit.transform != null)
        {
            Debug.Log(hit.transform.gameObject.name);
        }
        return 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * (boxCastSize.y * 2),  boxCastSize * 2);
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

        // dashIndicator.SetColor("_EmissionColor", Color.white);
        states = PlayerStates.Dash;

        yield return new WaitForSeconds(dashTimer);
        
        // dashIndicator.SetColor("_EmissionColor", Color.yellow * 10);
        
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

    public void StartCountdown(InputAction.CallbackContext context)
    {
        
        if (context.performed &&!UIManager.Instance.timer.countDownIsRunning && !disableMovement)
        {
            UIManager.Instance.StartCountdown();
        }
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