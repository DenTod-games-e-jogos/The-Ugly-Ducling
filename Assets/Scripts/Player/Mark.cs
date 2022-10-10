using UnityEngine;

namespace Player
{
    public class Mark : MonoBehaviour
    {
        float forwardMovement = 3.5f;

        float leftMovement = 8.0f;

        float rightMovement = 8.0f;

        float backMovement = 3.0f;

        float velocidade = 6.5f;

        float girar = 17.5f;

        readonly KeyCode W = KeyCode.W;

        readonly KeyCode S = KeyCode.S;

        readonly KeyCode A = KeyCode.A;

        readonly KeyCode D = KeyCode.D;

        readonly KeyCode Sp = KeyCode.Space;

        readonly KeyCode z = KeyCode.Z;

        public Light LuzPlayer;

        CharacterController mark;

        Animator anim;

        bool AutoJump;

        void Awake()
        {
            LuzPlayer = GetComponentInChildren<Light>();

            mark = GetComponent<CharacterController>();

            anim = GetComponent<Animator>();
        }
        
        void SetAutoJump(bool IsAutoJump)
        {
            AutoJump = true;

            anim.SetBool("AutoJump", AutoJump);
        }

        void Update()
        {
            transform.Rotate(0, Input.GetAxisRaw("Horizontal") * girar * Time.fixedDeltaTime, 0);

            Vector3 forward = transform.TransformDirection(Vector3.forward);

            float Giro = velocidade * Input.GetAxisRaw("Vertical");

            mark.SimpleMove(forward * Giro);

            mark.detectCollisions = true;

            if(mark.isGrounded)
            {
                if(Input.GetKey(A))
                {
                    Vector3 position = transform.TransformDirection(Vector3.right *
                    rightMovement * Time.fixedDeltaTime);
                }

                else if(Input.GetKey(D))
                {
                    Vector3 position = transform.TransformDirection(Vector3.left * 
                    leftMovement * Time.fixedDeltaTime);
                }

                if (Input.GetKey(W)) 
                {
                    Vector3 position = transform.TransformDirection (Vector3.forward *
                    forwardMovement * Time.fixedDeltaTime);
                }

                else if (Input.GetKey(S)) 
                {
                    Vector3 position = transform.TransformDirection (Vector3.back *
                    backMovement * Time.fixedDeltaTime);
                }

                if (Input.GetKeyDown(Sp))
                {
                    SetAutoJump(AutoJump);
                }

                if (Input.GetKeyDown(z)) 
                {
                    LuzPlayer.enabled = !LuzPlayer.enabled;
                }
            }
        }
    }
}