using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField]
    Sounds[] musics;

    Sounds soundScript = new Sounds();

    // Start is called before the first frame update
    void Start()
    {
        soundScript.LoadSounds(musics);
        soundScript.PlayRandomSound(musics);
    }


}
