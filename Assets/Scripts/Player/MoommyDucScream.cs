using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class MoommyDucScream : MonoBehaviour 
{
	readonly KeyCode e = KeyCode.E;

	[SerializeField]
    AudioSource ScreamMoommyDuc;
    
    void Update()
    {
        if(Input.GetKey(e))
        {
            ScreamMoommyDuc.Play();
        }
    }
}