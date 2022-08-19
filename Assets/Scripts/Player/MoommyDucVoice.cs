using UnityEngine;

public class MoommyDucVoice : MonoBehaviour 
{
	readonly KeyCode e = KeyCode.E;

    public AudioSource VoiceMoommyDuc;
    
    void Update()
    {
        if(Input.GetKey(e))
        {
            VoiceMoommyDuc.Play();
        }
    }
}