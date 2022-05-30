using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour 
{
    CharacterController Controller;

    Rigidbody Rb;

    float Speed = 3.0f;

    float RotateSpeed = 3.0f;

    Vector3 PlayerVelocity;

    bool GroundedPlayer;

    bool IsJumpPressed = false;

    bool IsMoving = false;

    float JumpHeight = 7.0f;

    float GravityValue = -1.5f;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();

        Rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        IsJumpPressed = Input.GetButtonDown("Jump");
    }

    void FixedUpdate()
    {
        if (GroundedPlayer && PlayerVelocity.y < 0)
        {
            PlayerVelocity.y = 0.0f;
        }

        Vector3 Move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Controller.Move(Move * Time.deltaTime * Speed);

        if (Move != Vector3.zero)
        {
            gameObject.transform.forward = Move;
        }

        if (GroundedPlayer && IsJumpPressed)
        {
            Rb.velocity = new Vector3(0, 10, 0);

            IsMoving = true;
        }

        PlayerVelocity.y += GravityValue * Time.deltaTime;
        
        Controller.Move(PlayerVelocity * Time.deltaTime);
    }
}