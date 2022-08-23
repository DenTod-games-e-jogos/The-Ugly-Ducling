using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlyingCameraControl : MonoBehaviour
{
    Vector3 lookingDirection;
    Vector3 movingDirection;
    Vector3 currentLookingDirection;
    Vector3 currentPosition;
    [SerializeField] float rotationSpeed;
    [SerializeField] float travelSpeed;
    void OnMouse(InputValue input)
    {
        //lookAnglesy = Mathf.Clamp(lookAnglesy, -89, 89);      Apenas para o eixo Y?
        lookingDirection.y = input.Get<Vector2>().x;
        lookingDirection.x = input.Get<Vector2>().y;
        lookingDirection.z = 0;
    }

    void OnWASD(InputValue input)
    {
        /*
            w => y = 1
            s => y = -1
            a => x = -1
            d => x = 1
        */
        movingDirection = new Vector3 (0,0,0);
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
        //currentLookingDirection += lookingDirection * Time.deltaTime * rotationSpeed;
        //transform.eulerAngles = currentLookingDirection;
        //transform.LookAt(currentLookingDirection);

        //transform.Rotate(Vector3.up * Mathf.Clamp(lookingDirection.y * Time.deltaTime * rotationSpeed, -90, 90));
        transform.Rotate(Vector3.right * Mathf.Clamp(lookingDirection.x * Time.deltaTime * rotationSpeed, -90, 90), Space.World);
        //transform.eulerAngles(new Vector3(Mathf.Clamp(lookingDirection.x * Time.deltaTime * rotationSpeed, -90, 90),Mathf.Clamp(lookingDirection.y * Time.deltaTime * rotationSpeed, -90, 90),0));

        transform.Translate(movingDirection*travelSpeed);
    }
}
