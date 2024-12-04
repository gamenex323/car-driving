using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] float acceleration = 4;
    [SerializeField] float maxSpeed = 10;
    [SerializeField] float turnFactor = 2;
    [SerializeField] float driftFactor = 0.95f;
    [SerializeField] float deceleration = 2f; // The rate at which the car slows down
    [SerializeField] GameObject collisionParticles; // Assign a particle system prefab in the Inspector
    public AudioSource carCrash;
    public AudioSource carEngineSound;

    [SerializeField] Joystick joystick; // Assign joystick in the Inspector for mobile controls

    Rigidbody2D carRigidbody2D;
    float accelerationInput;
    Vector2 moveDirection;

    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Handle keyboard input
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
        }
        // Handle joystick input
        else if (joystick != null)
        {
            moveDirection = new Vector2(joystick.Horizontal, joystick.Vertical).normalized;
        }
        else
        {
            moveDirection = Vector2.zero; // No input, car will coast
        }

        // Acceleration input is based on the magnitude of the movement direction
        accelerationInput = moveDirection.magnitude;
    }

    void FixedUpdate()
    {
        if (moveDirection.magnitude > 0.1f) // When there is input, move the car
        {
            ApplyMovement();
            ReduceLateralVelocity();
            UpdateEngineSound();
        }
        else // When no input, slow down the car and keep it moving forward
        {
            ApplyCoasting();
        }
    }

    void ApplyMovement()
    {
        // Rotate the car towards the movement direction
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
        carRigidbody2D.MoveRotation(Mathf.LerpAngle(carRigidbody2D.rotation, angle, Time.fixedDeltaTime * turnFactor));

        // Apply forward force
        Vector2 forwardForce = transform.up * acceleration * accelerationInput;
        carRigidbody2D.AddForce(forwardForce, ForceMode2D.Force);

        // Clamp speed
        if (carRigidbody2D.velocity.magnitude > maxSpeed)
        {
            carRigidbody2D.velocity = carRigidbody2D.velocity.normalized * maxSpeed;
        }
    }

    void ApplyCoasting()
    {
        // Apply a deceleration force to gradually slow down the car
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 lateralVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        // Reduce lateral velocity (drift) to simulate coasting with a natural slowdown
        carRigidbody2D.velocity = forwardVelocity + lateralVelocity * driftFactor;

        // Apply deceleration if there's no input
        if (carRigidbody2D.velocity.magnitude > 0.1f)
        {
            carRigidbody2D.velocity = Vector2.Lerp(carRigidbody2D.velocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    void ReduceLateralVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 lateralVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        // Apply drift factor to lateral velocity
        carRigidbody2D.velocity = forwardVelocity + lateralVelocity * driftFactor;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if a particle system prefab is assigned
        if (collisionParticles != null)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Instantiate(collisionParticles, contact.point, Quaternion.identity);
            }
        }

        if (carCrash != null)
        {
            carCrash.Play();
        }

        Debug.Log($"Collided with {collision.gameObject.name}");
    }

    void UpdateEngineSound()
    {
        if (carEngineSound != null)
        {
            // Map speed to pitch: from idle pitch to a max pitch
            float speedPercent = carRigidbody2D.velocity.magnitude / maxSpeed;
            carEngineSound.pitch = Mathf.Lerp(1f, 2f, speedPercent);
        }
    }
}

