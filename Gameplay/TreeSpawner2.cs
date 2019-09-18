using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is in charge of spawning all of the trees that line the road in the second scene
public class TreeSpawner2 : MonoBehaviour
{
    [SerializeField] private GameObject[] treePrefabs; // the different trees
    [SerializeField] private float treeSizeMin, treeSizeMax; // tree size range
    [SerializeField] private int numTrees; // number of trees
    [SerializeField] private float leftRowX, rightRowX, minZ, maxZ; // left and right X values, min and max Z values

    // this script only acts at the beginning of the scene
    void Start()
    {
        SpawnTrees();
    }
    
    // spawn the trees!
    void SpawnTrees()
    {
        // for each tree, give it a random offset, rotate it randomly and give it a random scale
        for (int i = 0; i < numTrees; i++)
        {
            Vector3 nextTreePos;
            float xOffset = Random.Range(-2f, 2f); // offsets
            float zOffset = Random.Range(-2f, 2f); // ofssets
            if (i < numTrees / 2)
                nextTreePos = new Vector3(leftRowX + xOffset, 0f, minZ + ((maxZ - minZ) * (i / (numTrees/2f))) + zOffset);
            else
                nextTreePos = new Vector3(rightRowX + xOffset, 0f, minZ + ((maxZ - minZ) * ((i - numTrees/2f) / (numTrees/2f))) + zOffset);
            int treeIdx = Random.Range(0, treePrefabs.Length);
            GameObject newTree = Instantiate(treePrefabs[treeIdx], nextTreePos, Quaternion.identity);
            Vector3 oldRot = newTree.transform.eulerAngles;
            newTree.transform.eulerAngles = new Vector3(oldRot.x, Random.Range(0f, 360f), oldRot.z);
            newTree.transform.localScale = Vector3.one * Random.Range(treeSizeMin, treeSizeMax);
        }
    }
}
