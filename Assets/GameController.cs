using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float timeToFeedDrop;
    public float dashDuration;
    public float dashCooldown;
    public bool gameStart = false;
    public bool foodTime = false;
    public int foodCount = 1;
    public float fallTime = 3;
    public PlayerMovement player;
    public InputManagerScript inputManager;
    public GameObject foodToSpawn;

    private Bounds bounds;
    private GameObject[] pools;
    private GameObject[] grassArray;


    /*Bounds bounds = GetComponent<Collider>().bounds;
    float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
    float offsetY = Random.Range(-bounds.extents.y, bounds.extents.y);
    float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);

    GameObject newHazard = GameObject.Instantiate(hazard);
    newHazard.transform.position = bounds.center + new Vector3(offsetX, offsetY, offsetZ);*/


    // Start is called before the first frame update
    void Start()
    {
        foodTime = false;
        StartCoroutine(CountDownToFeedDrop());
        player = FindObjectOfType<PlayerMovement>();
        player.dashDuration = dashDuration;
        player.dashCooldown = dashCooldown;
        inputManager = FindObjectOfType<InputManagerScript>();
        inputManager.rollCooldown = dashDuration + dashCooldown;
        bounds = GetComponent<Collider>().bounds;
        pools = GameObject.FindGameObjectsWithTag("Pool");
        grassArray = GameObject.FindGameObjectsWithTag("Grass");
    }

    Vector3 randomSpawnPoint()
    {
        float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
        float offsetY = 20;
        float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);

        return bounds.center + new Vector3(offsetX, offsetY, offsetZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

    }

    private IEnumerator CountDownToFeedDrop()
    {
        Debug.Log("-----------STARTING COUNTDOWN TO FEED!--------------");
        float duration = timeToFeedDrop;
        float timeLeft = duration;
        float normalizedTime = 0f;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            timeLeft -= Time.deltaTime;
            //Debug.Log("Time left: " + timeLeft);
            //Debug.Log("normalizedTime:" + normalizedTime);
            yield return null;
        }
        Debug.Log("-----------FOOD TIME!--------------");
        while (foodCount > 0)
        {
            GameObject newFood = GameObject.Instantiate(foodToSpawn);
            newFood.transform.position = bounds.center + randomSpawnPoint();
            Debug.Log("foodCount: " + foodCount);
            --foodCount;
            if (foodCount == 0)
            {
                foreach (GameObject pool in pools)
                {
                    Destroy(pool);
                }
                foreach (GameObject grass in grassArray)
                {
                    Destroy(grass);
                }
                StartCoroutine(WaitForDrop());
            }
        }
    }

    private IEnumerator WaitForDrop()
    {
        float duration = fallTime;
        float normalizedTime = 0f;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        foodTime = true;
    }
}
