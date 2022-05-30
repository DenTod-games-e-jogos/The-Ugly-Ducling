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

    float jumpHeight = 6.0f;

    float gravityValue = -1.5f;
    
    bool isJumping, isJumpingAlt, isGrounded, groundedPlayer;
    
    Vector3 movement;

    CharacterController Controller;

    Vector3 playerVelocity;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            isGrounded = false;

            isJumping = true;

            playerVelocity.y = 0f;
        }
        
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Controller.Move(move * Time.deltaTime * Speed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        if (Input.GetButtonDown("Jump") && groundedPlayer == true)
        {
            isJumpingAlt = true;
            
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        Controller.Move(playerVelocity * Time.deltaTime);
    }
}