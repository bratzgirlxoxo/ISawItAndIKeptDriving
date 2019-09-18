using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.SceneManagement;

// this script controlls the fog and some miscellaneous things in the scene
public class FogController : MonoBehaviour
{
    [Header("Fog")]
    [SerializeField] private Transform rearWall; 
    [SerializeField] private float ringFogDensity;
    [SerializeField] private float normalFogDensity;
    [SerializeField] private AnimationCurve fogCurve;
    [SerializeField] private float fogLightenTime;
    [Header("Misc")]
    [SerializeField] private CarController cars;
    [SerializeField] private MeshFilter concreteMesh;
    [SerializeField] private Mesh closedConcrete;
    [SerializeField] private GameObject rings;
    
    private bool inRing;
    private Vector3 startPos;
    private float startDist;

    void Awake()
    {
        startPos = transform.position;
        startDist = Vector2.Distance(new Vector2(0f, startPos.z), new Vector2(0f, rearWall.position.z));
        rings.SetActive(false);
    }

    
    void Update()
    {
        // dist is depth distance between rear wall and player
        float dist = Vector2.Distance(new Vector2(0f, transform.position.z), new Vector2(0f, rearWall.position.z));

        // when the player enters the ring, lighten the fog a bit
        if (!inRing && dist >= startDist)
        {
            RenderSettings.fogDensity = normalFogDensity;
        }
        else if (!inRing)
        {
            float newDensity = Remap(startDist - dist, 0f, startDist, normalFogDensity, 1f);
            RenderSettings.fogDensity = newDensity;
        }
        
    }

    // trigger function for entering the ring and hitting the back wall
    void OnTriggerEnter(Collider coll)
    {
        // if enters ring, set the appropriate bools an dlighten the fog
        if (!inRing && coll.CompareTag("ring"))
        {
            inRing = true;
            cars.inRange = false;
            StartCoroutine(FogLighten());
        }

        // if player hits the rear wall, reload the scene
        if (coll.CompareTag("rear"))
        {
            SceneManager.LoadScene("Road");
        }
    }

    // simple remap function
    float Remap(float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }


    // lightens the fog using an animation curve lerp to get a smooth effect
    IEnumerator FogLighten()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fogLightenTime;
            float curvVal = fogCurve.Evaluate(t);
            RenderSettings.fogDensity = Remap(curvVal, 0f, 1f, ringFogDensity, normalFogDensity);
            yield return null;
        }

        concreteMesh.mesh = closedConcrete; // close of the concrete wall.
        concreteMesh.gameObject.AddComponent<MeshCollider>(); // give it a collider
    }
}
