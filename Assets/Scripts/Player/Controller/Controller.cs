using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] 
    float _Speed = 100;
    
    PlayerControler _Input;
    
    Vector2 _Movement;
    
    Rigidbody _Rigidbody;
    
    Animator _Anim;

    void Awake() 
    {
        _Input = new PlayerControler();

        _Input.Enable();
        
        _Rigidbody = GetComponent<Rigidbody>();
        
        _Anim = GetComponent<Animator>();
    }

    void OnEnable() 
    {
        _Input.Enable();

        _Input.Charactercontroller.Movement.performed += OnMovement;

        _Input.Charactercontroller.Movement.canceled += OnMovement;
    }

    void OnDisable() 
    {
        _Input.Disable();
    }

    void OnMovement(InputAction.CallbackContext ctx) 
    {
        _Movement = ctx.ReadValue<Vector2>();

        _Anim.SetFloat("Speed", _Movement.magnitude);

        if (_Movement.x != 0) 
        {
            _Anim.SetFloat("Horizontal", _Movement.x);

            _Anim.SetFloat("Vertical", 0);
        }

        if (_Movement.y != 0) 
        {
            _Anim.SetFloat("Horizontal", 0);

            _Anim.SetFloat("Vertical", _Movement.y);
        }
    }

    void FixedUpdate() 
    {
        _Rigidbody.velocity = _Movement * _Speed * Time.deltaTime;
    }
}