using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script controls the audio and lights for the last section of the second scene, inside the ring
public class AudioController : MonoBehaviour
{
    [Header("Misc")]
    [SerializeField] private AudioSource source1;
    [SerializeField] private AudioSource source2;
    [SerializeField] private AudioSource lamboRev;
    [SerializeField] private AudioSource truckRev;
    [SerializeField] private AudioSource muscleRev;
    [Header("Lights")]
    [SerializeField] private Light lamboLight;
    [SerializeField] private Light trucklight;
    [SerializeField] private Light musclelight;
    [Header("Misc")]
    [SerializeField] private Transform gateway;
    [SerializeField] private CrashManager crash;
    [SerializeField] private Material fadeToBlackMat;
    [SerializeField] private float fadeTime;

    private AudioClip clip;
    private float startdist;

    private bool readyToSwitch;
    private bool switched;
    private float timeLeft;
    
    
    void Start()
    {
        clip = source1.clip;
        source1.volume = 0f;

        startdist = Vector3.Distance(transform.position, gateway.position);
        lamboLight.enabled = false;
        trucklight.enabled = false;
        musclelight.enabled = false;
        
        fadeToBlackMat.SetFloat("_Alpha", 0f);
    }

    private float t;
    private float t2;
    private float t3;

    private bool mat1, mat2, mat3, light1, light2, light3;
    void Update()
    {
        // the first two segments of this if/elseif/else statement set up the scene for the final part:
        // the first one increases the volume of the initial track aas the player approaches the ring
        // the second one switches the track at a smooth time
        if (!readyToSwitch)
        {
            float dist = Vector3.Distance(transform.position, gateway.position);
            source1.volume = (startdist - dist) / startdist;
        }
        else if (!switched)
        {
            t += Time.deltaTime;
            if (t >= timeLeft)
            {
                switched = true;
                source2.Play();
                source1.Stop();
                t = 0f;
            }
        }
        else
        {
            t += Time.deltaTime;

            // all of these if statements essentially control the lights and audio tracks and materials for the final sequence
            // it is all just a crude timing based system based on the timing of the audio.
            // im sure theres a more elegant way to do this  but im only using it once so, o well.

            if (!light1 && t >= 23.3f)
            {
                light1 = true;
                trucklight.enabled = true;
                truckRev.Play();
            }
            
            if (!light2 && t >= 28.6f)
            {
                light2 = true;
                lamboLight.enabled = true;
                lamboRev.Play();
            }

            if (!light3 && t >= 33.49)
            {
                light3 = true;
                musclelight.enabled = true;
                muscleRev.Play();
            }
            
            if (!mat1 && t >= 38.6f)
            {
                mat1 = true;
                crash.mat1crash = true;
            }
            
            if (!mat2 && t >= 44f)
            {
                mat2 = true;
                crash.mat2crash = true;
            }
            
            if (!mat3 && t >= 49f)
            {
                mat3 = true;
                crash.mat3crash = true;
            }

            if (t > 121.5f)
            {
                Application.Quit();
            }
        }  
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("ring"))
        {
            readyToSwitch = true;
            timeLeft = clip.length - Time.time % clip.length;
        }
    }
}
