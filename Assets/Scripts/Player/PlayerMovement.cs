using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour 
{
    float Speed = 3.0f;

    float RotateSpeed = 3.0f;

    float moveSpeed = 2;

    float rotationSpeed = 4;
    
    float runningSpeed;
    
    float vaxis, haxis;
    
    public bool isJumping, isJumpingAlt, isGrounded = false;
    
    Vector3 movement;

    CharacterController Controller;

    Rigidbody Rb;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();

        Rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Debug.Log("Initialized: (" + this.name + ")");
    }

    void FixedUpdate()
    {
        vaxis = Input.GetAxis("Vertical");

        haxis = Input.GetAxis("Horizontal");
        
        isJumping = Input.GetButton("Jump");
        
        isJumpingAlt = Input.GetKey(KeyCode.Joystick1Button0);

        runningSpeed = vaxis;

        if (isGrounded)
        {
            movement = new Vector3(0, 0f, runningSpeed * 8);
            
            movement = transform.TransformDirection(movement);      
        }

        else
        {
            movement *= 0.70f;
        }

        Rb.AddForce(movement * moveSpeed);

        if ((isJumping || isJumpingAlt) && isGrounded)
        {
            Debug.Log(this.ToString() + " isJumping = " + isJumping);

            GetComponent<Rigidbody>().AddForce(Vector3.up * 150);
        }

        if ((Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f) && !isJumping && isGrounded)
        {
            if (Input.GetAxis("Vertical") >= 0)
            {
                transform.Rotate(new Vector3(0, haxis * rotationSpeed, 0));
            }

            else
            {
                transform.Rotate(new Vector3(0, -haxis * rotationSpeed, 0));
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entered");

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exited");

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}