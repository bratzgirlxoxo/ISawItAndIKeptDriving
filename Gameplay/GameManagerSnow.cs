using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

// this script serves as the general controller for the entire first scene of the game
public class GameManagerSnow : MonoBehaviour
{
    [Header("Driving Sound")]
    [SerializeField] private float soundsStartTime;
    [SerializeField] private AudioSource drivingSound;
    [SerializeField] private AudioSource drivingMusic;
    
    [Header("End Blinker")]
    [SerializeField] private float changeTime;
    [SerializeField] private int numberOfBlinks;
    [SerializeField] private float timeBetweenBlinks;
    [SerializeField] private Material blinkerMaterial1;
    [SerializeField] private Material blinkerMaterial2;
    [SerializeField] private Texture2D[] blinkerImages;
    [SerializeField] private float frameRate;
    [SerializeField] private AudioSource blinkerSound;
    [SerializeField] private GameObject railing1;
    [SerializeField] private GameObject railing2;
    [SerializeField] private GameObject railing3;
    
    [Header("Trees")] 
    [SerializeField] private GameObject[] trees;
    [SerializeField] private GameObject[] treePrefabs;
    
    [Header("Player")] 
    [SerializeField] private Transform player;
    [SerializeField] private FirstPersonController controller;
    [SerializeField] private float lerpTime;
    [SerializeField] private AnimationCurve accelCurve;
    
    
    private LightningController flashController;

    private bool soundPlaying;

    private bool changeScene;
    private float t;
    
    private float t2;
    private int imgIdx = -1;


    void Awake()
    {
        // set shader values
        blinkerMaterial1.SetFloat("_alive", 0f);
        blinkerMaterial2.SetFloat("_alive", 0f);

        // set components
        flashController = GetComponent<LightningController>();
        // disables railings
        railing1.SetActive(false);
        railing2.SetActive(false);
        railing3.SetActive(false);
    }
    
    void Update()
    {        
        t += Time.deltaTime;
        t2 += Time.deltaTime;

        // after soundStartTime, play the driving sound
        if (!soundPlaying && t > soundsStartTime)
        {
            soundPlaying = true;
            drivingSound.Play();
        }

        // constantly loop through all of the driving images for the blinkers, usually the blinkers are transparent
        if (t2 >= 1f / frameRate)
        {
            blinkerMaterial1.SetTexture("_MainTex", blinkerImages[(++imgIdx) % blinkerImages.Length]);
            blinkerMaterial2.SetTexture("_MainTex", blinkerImages[(++imgIdx) % blinkerImages.Length]);
            t2 = 0f;
        }
        
        // start the blinkers blinking at the end of the scene
        if (t >= changeTime && t <= 10000000f)
        {
            flashController.StopAllCoroutines();
            StartCoroutine(BlinkerBlink());
            controller.enabled = false;
            flashController.isBlinking = true;
            t = 100000000000f;
        }

        // change the scene in changeScene is true
        if (changeScene)
            SceneManager.LoadScene("Road");
    }

    bool treeSet = false;

    // couroutine for making the blinkers blink and the railings spin
    IEnumerator BlinkerBlink()
    {
        for (int i = 0; i < trees.Length; i++)
        {
            trees[i].GetComponent<MeshRenderer>().enabled = false;
        }
        
        
        // change the sound and background
        drivingMusic.Stop(); 
        blinkerSound.Play();
        Camera.main.backgroundColor = Color.white;

        // activate the railings
        railing1.SetActive(true);
        railing1.transform.parent = null;
        railing2.SetActive(true);
        railing2.transform.parent = null;
        railing3.SetActive(true);
        railing3.transform.parent = null;
        
        // railings spin!
        StartCoroutine(RailingStorm());
       
        
        int ticker =0;
        float timer = 0f;

        bool driving = false;
        while (ticker < numberOfBlinks * 2)
        {
            timer += Time.deltaTime;

            // alternate the shader transparency values ("_alive") to turn the blinkers on and off
            // meanwhile the images are looping as the texture for the blinkers the hwole time
            if (timer >= timeBetweenBlinks && ticker % 2 == 0)
            {
                ticker++;
                timer = 0f;
                blinkerMaterial1.SetFloat("_alive", 1);
                blinkerMaterial2.SetFloat("_alive", 1);
                
                Debug.Log("Blink");
            }
            else if (timer >= timeBetweenBlinks)
            {
                ticker++;
                timer = 0f;
                blinkerMaterial1.SetFloat("_alive", 0);
                blinkerMaterial2.SetFloat("_alive", 0);
            }
            
            yield return null;
        }

        
    }

    // couroutine for the railings
    IEnumerator RailingStorm()
    {
        // destroy the trees
        for (int i = 0; i < trees.Length; i++)
        {
            Destroy(trees[i]);
        }

        // set the start scales for all the railings
        Vector3 rail1sizeStart = railing1.transform.localScale;
        Vector3 rail2sizeStart = railing2.transform.localScale;
        Vector3 rail3sizeStart = railing3.transform.localScale;

        float t = 0f;

        // total rotation time
        float rotTime = timeBetweenBlinks * numberOfBlinks * 2f;
        while (t < rotTime)
        {
            // rotate all of the railings pseudorandomly using different perlin inputs

            t += Time.deltaTime;
            float xRot = Mathf.PerlinNoise(t * 0.925f, 0.764f) * 360f;
            float yRot = Mathf.PerlinNoise(0.247f, t * 0.758f) * 360f;
            float zRot = Mathf.PerlinNoise(t * 1.276f, 0.543f) * 360f;
            railing1.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
            railing1.transform.localScale = rail1sizeStart * (1 - t / rotTime);
            
            xRot = Mathf.PerlinNoise(-t * 0.365f, 0.764f) * 360f;
            yRot = Mathf.PerlinNoise(0.247f, -t * 0.358f) * 360f;
            zRot = Mathf.PerlinNoise(t * 0.576f, 0.543f) * 360f;
            railing2.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
            railing2.transform.localScale = rail2sizeStart * (1 - t / rotTime);
            
            xRot = Mathf.PerlinNoise(-t * 0.165f, 0.164f) * 360f;
            yRot = Mathf.PerlinNoise(0.147f, -t * 0.158f) * 360f;
            zRot = Mathf.PerlinNoise(t * 0.276f, 0.343f) * 360f;
            railing3.transform.eulerAngles = new Vector3(xRot, yRot, zRot);
            railing3.transform.localScale = rail3sizeStart * (1 - t / rotTime);
            
            yield return null;
        }

        // set the changeScene bool to be true
        changeScene = true;
    }

    
}
