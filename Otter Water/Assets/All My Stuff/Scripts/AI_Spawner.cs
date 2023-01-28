using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

 [System.Serializable]
public class AIObjects
{
    // Public Variables
    public string AIGroupName { get { return p_aiGroupName; } }
    public GameObject objectPrefab { get { return p_prefab; } }
    public int maxAI { get { return p_maxAI; } }
    public int spawnRate { get { return p_spawnRate; } }
    public int spawnAmount { get { return p_maxSpawnAmount; } }
    public bool randomizeStats { get { return p_randomizeStats; } }
    public bool enableSpawner { get { return p_enableSpawner; } }

    // Private variables
    [Header("AI Group Stats")]
    [SerializeField]
    private string p_aiGroupName;
    [SerializeField]
    private GameObject p_prefab;
    [SerializeField]
    [Range(0f, 30f)]
    private int p_maxAI;
    [SerializeField]
    [Range(0, 20f)]
    private int p_spawnRate;
    [SerializeField]
    [Range(0f, 10f)]
    private int p_maxSpawnAmount;
    
    [Header("Main Settings")]
    [SerializeField]
    private bool p_enableSpawner;
    [SerializeField]
    private bool p_randomizeStats;

    // Constructor made to randomize the fields outside of the class.
    public AIObjects (string Name, GameObject Prefab, int MaxAI, int SpawnRate, int SpawnAmount, bool RandomizeStats)
    {
        this.p_aiGroupName = Name;
        this.p_prefab = Prefab;
        this.p_maxAI = MaxAI;
        this.p_spawnRate = SpawnRate;
        this.p_maxSpawnAmount = SpawnAmount;
        this.p_randomizeStats = RandomizeStats;
    }

    public void setValues(int MaxAI, int SpawnRate, int SpawnAmount)
    {
        this.p_maxAI = MaxAI;
        this.p_spawnRate = SpawnRate;
        this.p_maxSpawnAmount = SpawnAmount;
    }
}

public class AI_Spawner : MonoBehaviour
{
    //-------
    // Declare Our Variables

    // Waypoint List for Locations to travel to.
    public List<Transform> Waypoints = new List<Transform>();
    
    public float spawnTimer { get { return p_spawnTimer; } }
    public Vector3 spawnArea { get { return p_spawnArea; } }

    //public variables for the variables above.
    [Header("Global Stats")]
    [Range(0f, 600f)]
    [SerializeField]
    private float p_spawnTimer; // How often stuff spawns.
    [SerializeField]
    private Color p_spawnColor = new Color(1.000f, 0.000f, 0.000f, 0.300f); //Uses color for gizmo. Red.
    [SerializeField]
    private Vector3 p_spawnArea = new Vector3 (20f, 10f, 20f);

    // Create array from new class
    [Header("AI Groups Settings")]
    public AIObjects[] AIObject = new AIObjects[5];

    // Empty Game Object to keep our AI in
    private GameObject p_AIGroupSpawn;

    // Start is called before the first frame update
    void Start()
    {
        GetWaypoints();
        RandomiseGroups();
        CreateAIGroups();
        InvokeRepeating("SpawnNPC", 0.5f, spawnTimer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnNPC()
    {
        for (int i = 0; i < AIObject.Count(); i++)
        {
            if (AIObject[i].enableSpawner && AIObject[i].objectPrefab != null)
            {
                GameObject tempGroup = GameObject.Find(AIObject[i].AIGroupName);
                if(tempGroup.GetComponentInChildren<Transform>().childCount < AIObject[i].maxAI)
                {
                    // Spawn Random Amount of NPCs from 0 to Max possible
                    for (int y = 0; y < Random.Range(0, AIObject[i].spawnAmount); y++)
                    {
                        //Random Rotation
                        Quaternion randomRotation = Quaternion.Euler(Random.Range(-20, 30), Random.Range(0, 360), 0);
                        //Create gameObject
                        GameObject tempSpawn;
                        tempSpawn = Instantiate(AIObject[i].objectPrefab, RandomPosition(), randomRotation);
                        //Add the movement script and class to the new NPC.
                        tempSpawn.AddComponent<AIMove>();
                    }
                }
            }
        }
    }

    // Random Position within the Spawn Area
    public Vector3 RandomPosition()
    {
        // Random position within Spawn Area
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            Random.Range(-spawnArea.z, spawnArea.z)
        );
        randomPosition = transform.TransformPoint(randomPosition * 0.5f);
        return randomPosition;
    }

    // Gets a random waypoint from the list of waypoints.
    public Vector3 RandomWaypoint()
    {
        int randomWP = Random.Range(0, (Waypoints.Count - 1));
        Vector3 randomWaypoint = Waypoints[randomWP].transform.position;
        return randomWaypoint;
    }

    // Puts random values in the AI Groups Settings
    void RandomiseGroups()
    {
        for (int i = 0; i < AIObject.Count(); i++)
        {
            //If the randomize box is checked...
            if (AIObject[i].randomizeStats)
            {
                AIObject[i].setValues(Random.Range(1, 30), Random.Range(1, 20), Random.Range(1, 10));
            }
        }
    }

    // Creating the empty worldobject groups
    void CreateAIGroups()
    {
        for (int i = 0; i < AIObject.Count(); i++)
        {    
            // Creates a new game object
            p_AIGroupSpawn = new GameObject(AIObject[i].AIGroupName);
            p_AIGroupSpawn.transform.parent = this.gameObject.transform;
        }
    }

    // Collects a list of our declared waypoints.
    void GetWaypoints()
    {
        Transform[] wpList = this.transform.GetComponentsInChildren<Transform>();
        for(int i = 0; i < wpList.Length; i++)
        {
            if (wpList[i].tag == "Waypoint")
            {
                Waypoints.Add(wpList[i]);
            }
        }
    }

    // Give the gizmos color
    void OnDrawGizmosSelected()
    {
        Gizmos.color = p_spawnColor;
        Gizmos.DrawCube(transform.position, spawnArea);
    }
}
