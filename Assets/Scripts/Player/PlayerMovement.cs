using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour 
{
    CharacterController Controller;

    float Speed = 3.0f;

    float RotateSpeed = 3.0f;

    Vector3 PlayerVelocity;

    bool GroundedPlayer;

    float JumpHeight = 1.0f;

    float GravityValue = -9.81f;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        GroundedPlayer = Controller.isGrounded;

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

        if (Input.GetButtonDown("Jump") && GroundedPlayer)
        {
            PlayerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
        }

        PlayerVelocity.y += GravityValue * Time.deltaTime;
        
        Controller.Move(PlayerVelocity * Time.deltaTime);
    }
}