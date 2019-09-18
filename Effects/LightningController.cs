using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// this script controls the lighting on the trees in the first scene
public class LightningController : MonoBehaviour
{
    [SerializeField] private Material[] treeMats; // tree materials
    [SerializeField] private Transform[] trees; // trees
    [SerializeField] private float strikeIntervalMinStart, strikeIntervalMaxStart, strikeDurMin, strikeDurMax; // strike parameters (time)
    [SerializeField] private FirstPersonController controller; // player
    [SerializeField] private float sceneDuration; // scenelength

    public bool isBlinking;

    private Vector3[] startRelPositions;
    
    private float t;
    private float nextStrikeTime;
    private float strikeMin, strikeMax;
    private float strikeDuration;
    
    void Start()
    {
        strikeMin = strikeIntervalMinStart;
        strikeMax = strikeIntervalMaxStart;
        startRelPositions = new Vector3[trees.Length];
        nextStrikeTime = Random.Range(strikeMin, strikeMax);
        for (int i = 0; i < trees.Length; i++)
        {
            startRelPositions[i] = trees[i].transform.localPosition; // set the relative start posiitons to the player
        }
    }
    
    private float t2;
    void Update()
    {
        t += Time.deltaTime;
        t2 += Time.deltaTime;

        // every nextStrikeTime seconds, strike the lightning
        if (!isBlinking && t >= nextStrikeTime)
        {
            t = 0f;
            nextStrikeTime = Random.Range(strikeMin, strikeMax);
            StartCoroutine(LightningStrike());
        }

        // strikes become more frequent over time!
        strikeMin = Mathf.Lerp(strikeIntervalMinStart / 3f, strikeIntervalMinStart, 1 - t2 / sceneDuration);
        strikeMax = Mathf.Lerp(strikeIntervalMaxStart / 3f, strikeIntervalMaxStart, 1 - t2 / sceneDuration);
        strikeDuration = Mathf.Lerp(strikeDurMin, strikeDurMax, 1 - t2 / sceneDuration);
    }
    
    // coroutine that controls the lightning strikes
    IEnumerator LightningStrike()
    {

        StartCoroutine(TreeFlicker());
 
        yield return new WaitForSeconds(strikeDuration);
        
        StopAllCoroutines();
        for (int i = 0; i < treeMats.Length; i++)
        {
            treeMats[i].SetColor("_Color", Color.black);
            trees[i].GetComponent<MeshRenderer>().enabled = true;
            trees[i].parent = Camera.main.transform;
            trees[i].localPosition = startRelPositions[i];
        }
        
        //yield return new WaitForSeconds(0.1f);
        controller.m_MouseLook.MaximumX = 0f;
        controller.m_MouseLook.MinimumX = 0f;
        Vector3 oldRot = Camera.main.transform.eulerAngles;
        Camera.main.transform.eulerAngles = new Vector3(0f, oldRot.y, oldRot.z);
        
    }

    // couroutine responsible for the flickering of each tree
    IEnumerator TreeFlicker()
    {
        int treeIdx = Random.Range(0, treeMats.Length);
        
        Vector3 oldRot = trees[treeIdx].localEulerAngles; // set random rotation
        trees[treeIdx].localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
        trees[treeIdx].parent = null;

        controller.m_MouseLook.MaximumX = 50f;
        controller.m_MouseLook.MinimumX = -50f;

        // enable the specific tree
        for (int i = 0; i < trees.Length; i++)
        {
            if (i != treeIdx)
                trees[i].GetComponent<MeshRenderer>().enabled = false;
        }
        

        // use perlin noise to flicker the intensity of tree sillouette
        float t = 0f;
        float colMax = Random.Range(0.25f, 0.5f);
        while (true)
        {
            t += Time.deltaTime;
            float colVal = Mathf.PerlinNoise(colMax, t * 10f) * colMax;
            treeMats[treeIdx].SetColor("_Color", new Vector4(colVal, colVal, colVal, 0.35f));

            yield return null;
        }
        
    }
}
