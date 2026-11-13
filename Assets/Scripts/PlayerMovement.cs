using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private float velocity;
    private float acceleration;
    private float jumpForce;
    private float groundCheckDistance;
    private float turnSpeed;
    private LayerMask groundMask;

    private Vector3 startPosition;
    private float fallLimit;

    private Rigidbody rb;
    private Collider col;
    private Transform cameraTransform;
    private PlayerStats stats;
    private IMovementStrategy movementStrategy;

    private Vector2 moveInput;
    private bool jumpRequested;

    public PlayerStats Stats => stats;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        velocity = 8f;
        acceleration = 40f;
        jumpForce = 7f;
        groundCheckDistance = 0.2f;  
        turnSpeed = 720f;            

        groundMask = LayerMask.GetMask("Ground");
        if (groundMask == 0)
        {
            Debug.LogWarning("PlayerMovement: no existe la capa Ground. Crearla y asignarla al terreno. Mientras tanto se usan todas las capas.");
            groundMask = ~0;
        }

        startPosition = transform.position;  
        fallLimit = startPosition.y - 15f;    

        stats = new PlayerStats(velocity, acceleration);
        SetMovementStrategy(new AccelerateMovement());
    }

    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        movementStrategy = strategy;
    }

    
    public void ResetToStart()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
    }

   
    private void Update()
    {
        if (Time.timeScale == 0f) return; 

        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;

        moveInput = CameraRelative(new Vector2(x, z));

        if (Input.GetKeyDown(KeyCode.Space))
            jumpRequested = true;

        RotateTowardsMovement();
    }

    
    private void FixedUpdate()
    {
        movementStrategy.Move(rb, stats, moveInput);

        if (jumpRequested && IsGrounded())
        {
            Vector3 v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(v.x, jumpForce, v.z);
        }
        jumpRequested = false;

        if (transform.position.y < fallLimit)
            ResetToStart();
    }


    private Vector2 CameraRelative(Vector2 input)
    {
        if (cameraTransform == null) return input;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        return new Vector2(direction.x, direction.z);
    }

    private void RotateTowardsMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        Bounds b = col.bounds;
        Vector3 feet = new Vector3(b.center.x, b.min.y + 0.05f, b.center.z);
        float distance = 0.05f + groundCheckDistance;

        Debug.DrawRay(feet, Vector3.down * distance, Color.green); 

        return Physics.Raycast(feet, Vector3.down, distance, groundMask, QueryTriggerInteraction.Ignore);
    }
}