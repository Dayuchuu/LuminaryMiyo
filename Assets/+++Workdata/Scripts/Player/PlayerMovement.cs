using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("Player Variables")]
    private Rigidbody rb;
    
    [SerializeField] private float defaultMoveSpeed;

    [SerializeField] private float maxMoveSpeed;

    [SerializeField] private float jumpPower;
    
    private float timer = 0f;
    
    private float inputX;
    
    private float inputZ;

    public bool disabled;
    
    [Header("Camera Variables")]
    [SerializeField] private Transform cameraTransform;
    
    [Range(0, 1)]
    [SerializeField] private float rotationSensibility;

    [SerializeField] private float maxCameraPitch = 80f;

    [SerializeField] private bool invertCameraPitch = false;
    
    private float cameraPitch;
    private float cameraRoll;
    
    #endregion 

    #region Methods
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        if(invertCameraPitch)
        {
            cameraPitch = cameraPitch - mouseDelta.y * rotationSensibility;
        }
        else
        {
            cameraPitch = cameraPitch + mouseDelta.y * rotationSensibility;
        }
        
        cameraPitch = Mathf.Clamp(cameraPitch, -maxCameraPitch, maxCameraPitch);

        cameraRoll = cameraRoll + mouseDelta.x * rotationSensibility;

        cameraTransform.localEulerAngles = new Vector3(cameraPitch, cameraRoll, 0f);
    }

    private void FixedUpdate()
    {
        if (disabled) return;
        
        Debug.Log(rb.velocity);
        
        Vector3 movementDirection =  new Vector3(inputX * defaultMoveSpeed, rb.velocity.y, inputZ * defaultMoveSpeed);
        
        movementDirection =
            Quaternion.AngleAxis(cameraTransform.localEulerAngles.y, Vector3.up) * movementDirection;
             
        rb.velocity = movementDirection;
        
        /*Debug.Log(rb.velocity);
        
        Vector3 targetVelocity = new Vector3(inputX, 0f, inputZ);

        targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxMoveSpeed, maxMoveSpeed);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxMoveSpeed, maxMoveSpeed);
        velocityChange.y = 0f;
        
        //velocityChange = Quaternion.AngleAxis()
        
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        
        if (rb.velocity.magnitude >= 0.05f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }*/
    }

    public void Move(InputAction.CallbackContext context)
    {
        inputX = context.ReadValue<Vector3>().x;
        inputZ = context.ReadValue<Vector3>().z;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
    }

    public void DisableMovement()
    {
        disabled = true;
        
        rb.velocity = Vector3.zero;
    }
    #endregion
}
