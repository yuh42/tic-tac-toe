using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    // Start is called before the first frame update
    public AudioClip[] Audios;
    AudioSource source;
    private void Start() {
        source=GetComponent<AudioSource>();
    }
    public void PlaySound(int index){
        // if(!source.isPlaying){
            source.clip=Audios[index];
            source.Play();
        // }
        
    }
}
