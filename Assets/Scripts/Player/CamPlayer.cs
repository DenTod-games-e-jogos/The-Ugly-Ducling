using UnityEngine;

public class CamPlayer : MonoBehaviour 
{
    public Transform MarkVision;

    public Camera CameraPlayer;

    void Awake()
    {
        MarkVision = GetComponent<Transform>();

        CameraPlayer = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Physics.Linecast(CameraPlayer.transform.position, MarkVision.transform.position))
		{
			transform.position = Vector3.Lerp(CameraPlayer.transform.position,
			MarkVision.transform.position, 1.0f);
		}
    }
}