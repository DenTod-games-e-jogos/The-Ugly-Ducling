using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour 
{
    float Speed = 3.0f;

    float RotateSpeed = 3.0f;

    float moveSpeed = 5.0f;

    float rotationSpeed = 4;
    
    float runningSpeed = 10.0f;
    
    float vaxis = 5.0f, haxis = 6.0f;
    
    public bool isJumping, isJumpingAlt, isGrounded = false;
    
    Vector3 movement;

    CharacterController Controller;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        print("Initialized: (" + this.name + ")");
    }

    void Update()
    {
        transform.Rotate(0, Input.GetAxis("Horizontal") * RotateSpeed, 0);

        Vector3 Forward = transform.TransformDirection(Vector3.forward);

        float CurSpeed = Speed * Input.GetAxis("Vertical");
        
        Controller.SimpleMove(Forward * CurSpeed);
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

            isGrounded = Controller.isGrounded;      
        }

        else
        {
            movement *= 0.70f;
        }

        if ((isJumping || isJumpingAlt) && isGrounded)
        {
            print(this.ToString() + " isJumping = " + isJumping);
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
}