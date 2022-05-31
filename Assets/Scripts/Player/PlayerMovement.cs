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

    float jumpHeight = 60.0f;

    float gravityValue = -1.5f;
    
    public bool isJumping = false;

    public bool isJumpingAlt = false;

    public bool isGrounded = false;
    
    Vector3 movement;

    CharacterController Controller;

    [SerializeField]
    Vector3 playerVelocity;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            isGrounded = true;

            playerVelocity.y = 0.0f;
        }
        
        movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

        Controller.Move(movement * Time.deltaTime * Speed);

        if (movement != Vector3.zero)
        {
            gameObject.transform.forward = movement;
        }

        if (Input.GetButtonDown("Jump"))
        {
            print("Botão de pulo apertado");

            print("Is Grounder?" + isGrounded);

            if (isGrounded)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        Controller.Move(playerVelocity * Time.deltaTime);
    }
}