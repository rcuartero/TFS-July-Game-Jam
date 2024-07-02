using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private bool isPaused = false;

    [Header("Spawn Properties")]
    [SerializeField] private float spawnTime;
    [SerializeField] private float maxSpawnRange;
    [SerializeField] private float minSpawnRange;
    private float spawnTimer = 0;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask treeLayer;

    // spawn location checks;
    private int spawnCheckCount = 3;


    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance)
        {
            GameManager.instance.onPause += PauseBehaviours;
            GameManager.instance.onResume += ResumeBehaviours;
            isPaused = GameManager.instance.GetIsPaused();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance)
        {
            GameManager.instance.onPause -= PauseBehaviours;
            GameManager.instance.onResume -= ResumeBehaviours;
        }
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        if (isPaused) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnTime)
        {
            spawnTimer = 0;

            for (int i = 0; i < spawnCheckCount; i++)
            {
                Vector3 spawnLocation = GetSpawnLocation();


                // Sphere check to ensure there are no other trees near spawn location
                Physics.SphereCast(spawnLocation, minSpawnRange, transform.up, out RaycastHit treeHitInfo, minSpawnRange, treeLayer);

                if (treeHitInfo.transform != null)
                {
                    // spawn if no trees found
                    Instantiate(gameObject, spawnLocation, transform.rotation);
                    break;
                }
            }

        }
    }

    private void PauseBehaviours()
    {
        isPaused = true;
    }

    private void ResumeBehaviours()
    {
        isPaused = false;
    }

    private Vector3 GetSpawnLocation()
    {
        // Get Random x value within spawn range
        float xPosition = Random.Range(minSpawnRange, maxSpawnRange);
        if (Random.value < 0.5f) xPosition = -xPosition;

        // Get Random z value within spawn range
        float zPosition = Random.Range(minSpawnRange, maxSpawnRange);
        if (Random.value < 0.5f) zPosition = -zPosition;

        // Checck if tree will spawn above ground        
        Ray ray = new Ray(transform.position + new Vector3(xPosition, 1, zPosition), -Vector3.up);
        Physics.Raycast(ray, out RaycastHit raycastHit, 100, groundLayer);

        // if above valid target, return position, else get a new position
        if (raycastHit.transform != null) return raycastHit.point;

        else return GetSpawnLocation();
    }
}