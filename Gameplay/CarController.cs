using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script controls the cars that come speeding from behind the player as they walk down the road
public class CarController : MonoBehaviour
{

    [SerializeField] private Transform[] cars; // the array of cars
    [SerializeField] private float carFreqMin, carFreqMax, carDriveTimeMin, carDriveTimeMax; // car spawn time mins and maxes

    private Vector3[] startPositions, endPositions; // car start and end positions
    private int idx = 0; // car counter
    private float nextCarTime; // temp value to control car spawn time
    private float t = 0f; // timer
    private Vector3 fwd = new Vector3(0f, 0f, 1f); // world fwd vector

    [HideInInspector] public bool inRange = true;
    
    void Start()
    {
        // establish start and end positions for the cars to spawn and be destroyed at
        startPositions = new Vector3[cars.Length];
        endPositions = new Vector3[cars.Length];

        for (int i = 0; i < cars.Length; i++)
        {
            startPositions[i] = cars[i].position;
            Vector3 sp = startPositions[i];
            endPositions[i] = new Vector3(sp.x, sp.y, sp.z + 225f);
        }

        nextCarTime = Random.Range(carFreqMin, carFreqMax); // first car spawn time
    }

    void Update()
    {
        t += Time.deltaTime;

        // depthDist determines how far the player (camera) is from the start positions, with regard to their depth value
        float depthDist = Mathf.Abs(Camera.main.transform.position.z - startPositions[0].z);

        // this statement ensures that the player is within the space where cars should spawn...
        // additionally, it checks to make sure the player is facing away from the direction from which the cars come
        // this is so that the cars always spawn when the player is not facing them
        // if this is met, spawn a car
        if (inRange && t >= nextCarTime && Vector3.Dot(fwd, Camera.main.transform.forward) >= 0 && depthDist < 165f)
        {
            t = 0f;
            int relIdx = idx % cars.Length;
            StartCoroutine(CarDrive(cars[relIdx], startPositions[relIdx], endPositions[relIdx], relIdx));
            idx++;
            nextCarTime = Random.Range(carFreqMin, carFreqMax);
        }
    }

    // coroutine to control the driving of the car and the playing of the woosh drive sound
    // utilizes a standard lerp
    IEnumerator CarDrive(Transform car, Vector3 startPos, Vector3 endPos, int carIdx)
    {
        float t = 0f;
        float driveTime = Random.Range(carDriveTimeMin, carDriveTimeMax);

        bool wooshed = false;        
        while (t < 1f)
        {
            t += Time.deltaTime / driveTime;
            car.position = Vector3.Lerp(startPos, endPos, t);

            float depthDist = Mathf.Abs(Camera.main.transform.position.z - car.position.z);
            if (!wooshed && depthDist <= 35f)
            {
                wooshed = true;
                car.GetComponent<AudioSource>().Play();
            }
            yield return null;
        }

        car.transform.position = startPos;
    }
}
