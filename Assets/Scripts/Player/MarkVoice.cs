using UnityEngine;

public class MarkVoice : MonoBehaviour
{
    readonly KeyCode e = KeyCode.E;

    public AudioSource VoiceMark;
    
    void FixedUpdate()
    {
        if(Input.GetKey(e))
        {
            VoiceMark.Play();
        }
    }
}