using UnityEngine;
using UnityEngine.InputSystem;

public class FlyingCameraControl : MonoBehaviour
{
    Vector3 lookingDirection;

    Vector3 movingDirection;
    
    Vector3 currentLookingDirection;
    
    Vector3 currentPosition;

    [SerializeField] 
    float rotationSpeed;
    
    [SerializeField] 
    float travelSpeed;
    
    void OnMouse(InputValue input)
    {
        lookingDirection.y = input.Get<Vector2>().x;

        lookingDirection.x = input.Get<Vector2>().y;
        
        lookingDirection.z = 0;
    }

    void OnWASD(InputValue input)
    {
        movingDirection = new Vector3 (0, 0, 0);

        movingDirection += transform.forward * input.Get<Vector2>().y;
        
        movingDirection += transform.right * input.Get<Vector2>().x;
    }

    void Start()
    {
        lookingDirection = new Vector3(0, 0, 0);

        movingDirection = new Vector3(0, 0, 0);

        currentLookingDirection = transform.eulerAngles;
    }

    void Update()
    {
        transform.Rotate(Vector3.right * Mathf.Clamp(lookingDirection.x * Time.deltaTime * rotationSpeed, -90, 90), 
        Space.World);

        transform.Translate(movingDirection*travelSpeed);
    }
}