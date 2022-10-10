using UnityEngine;

public class MarkAnimation : MonoBehaviour 
{
    float vel = 8.5f;

    float JumpForce = 10.0f;

    Rigidbody rb;

    Animator an;

    Vector3 mover;

    bool AutoJump;

    void Awake()
    {
        an = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        float v = Input.GetAxisRaw("Vertical");

        Mover(h, v);

        Animar(h, v);

        SetAutoJump(AutoJump);
    }

    void Mover(float h, float v)
    {
        mover.Set(h, 0.0f, v);

        mover = mover.normalized * vel * Time.fixedDeltaTime;

        rb.MovePosition(transform.position + mover);
    }

    void Animar(float h, float v)
    {
        bool walking = h != 0.0f || v != 0.0f;

        an.SetBool("IsWalking", walking);
    }

    void SetAutoJump(bool IsAutoJump)
    {
        AutoJump = true;

        an.SetBool("AutoJump", AutoJump);

        rb.AddForce(Vector3.up * JumpForce * Time.deltaTime);
    }
}