using UnityEngine;

public class MoommyDucVoice : MonoBehaviour 
{
	readonly KeyCode e = KeyCode.E;

    public AudioSource VoiceMoommyDuc;
    
    void FixedUpdate()
    {
        if(Input.GetKey(e))
        {
            VoiceMoommyDuc.Play();
        }
    }
}