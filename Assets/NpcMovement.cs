using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    public float hitPoints;
    public float movementSpeed;
    public float groundedSpeed;
    public float maxMovementSpeed;
    public float buffMultiplier;
    public float movementTimer;
    public float moveDuration;
    public float popForce;
    public float offSet;
    public float randOffSetRange;
    public float randMoveSetRange;
    public float poolCastTime;
    public float grassSatisfactionTime;
    public float despawnDelay;
    public Rigidbody rigidBody;
    public Collider genericCollider;
    public AudioClip[] popSounds;
    public AudioClip[] hitSounds;
    private foodScript[] foodScriptsOnField;
    private List<GameObject> foodOnField = new List<GameObject>();
    private List<Transform> transformsToSearch = new List<Transform>();
    private Vector3 closestFood;
    private GameObject[] foodArray;
    private GameController gameController;
    private AudioSource audioSource;
    private Bounds bounds;

    public int multiplier;
    public GameObject toDuplicate;
    public GameObject toMutate;
    public float distanceToGround = 1f;

    public bool isGroundedBool = false;

    public bool inPool = false;
    public bool spawned = false;
    public bool inGrass = false;
    public bool eating = false;
    public bool gettingReadyToMove = false;
    public bool foodHasBeenFound = false;
    public bool dead = false;

    Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    Renderer rend;


    void Start()
    {
        gameController = Object.FindObjectOfType<GameController>();
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        
        distanceToGround = genericCollider.bounds.extents.y;
    }

    void Update()
    {
        distanceToGround = genericCollider.bounds.extents.y;
    }

    void FixedUpdate()
    {
        if (spawned == false)
        {
            spawned = true;
            rigidBody.AddForce(transform.up * popForce);
        }
        if (bounds != gameController.bounds)
        {
            bounds = gameController.bounds;
        }
        isGroundedBool = isGrounded();
        if (isGrounded())
        {
            movementSpeed = groundedSpeed;
        }
        else
        {
            movementSpeed = maxMovementSpeed;
        }
        if (!dead) {
            if (rigidBody.velocity.magnitude > movementSpeed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * movementSpeed;
            }
            if (inPool == false && gettingReadyToMove == false)
            {
                StartCoroutine(RandomMovement());
            }
            if (hitPoints <= 0)
            {
                StartCoroutine(Kill());
            }
                FindFood();
        }
    }

    public void Damage(float damageToTake)
    {
        float pitchValue = .75f;
        pitchValue = Random.Range(.75f, 2);
        audioSource.pitch = pitchValue;
        audioSource.PlayOneShot(hitSounds[Random.Range(0, popSounds.Length)]);
        hitPoints -= damageToTake;
    }

    private IEnumerator Kill()
    {
        Quaternion npcRot = transform.localRotation;
        float duration = despawnDelay;
        float normalizedTime = 0;

        dead = true;
        npcRot.y -= 180;
        transform.localRotation = npcRot;
        rigidBody.freezeRotation = false;
        rigidBody.AddForce(transform.up * popForce * 2);
        while (normalizedTime <= 1f)
        {
            
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    private void FindFood()
    {
        if (!foodHasBeenFound) {
            
            foodScriptsOnField = Object.FindObjectsOfType<foodScript>();
            foreach (foodScript script in foodScriptsOnField)
            {
                foodOnField.Add(script.gameObject);
            }
            foodArray = foodOnField.ToArray();
        }
        
        
        foreach (GameObject obj in foodArray)
        {
            transformsToSearch.Add(obj.transform);
        }
        Vector3 tempVect = new Vector3(0,1000000,0);
        if (!foodHasBeenFound && gameController.foodTime == true) {
            tempVect = GetClosestFood(transformsToSearch.ToArray()).position;
        }
        if (tempVect != new Vector3(0, 1000000, 0) && !foodHasBeenFound)
        {
            foodHasBeenFound = true;
            closestFood = tempVect;
        }
    }

    Transform GetClosestFood(Transform[] foodArray)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform f in foodArray)
        {
            float dist = Vector3.Distance(f.position, currentPos);
            if (dist < minDist)
            {
                tMin = f;
                minDist = dist;
            }
        }
        return tMin;
    }

    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + .1f);
    }

    private IEnumerator RandomMovement()
    {
        float duration = movementTimer;
        float normalizedTime = 0;
        float randToMove;

        Vector3 forceToMove = new Vector3();
        gettingReadyToMove = true;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        gettingReadyToMove = false;
        if (isGrounded() && inPool == false)
        {
            randToMove = Random.Range(-randMoveSetRange - 1, randMoveSetRange + 1);
            Quaternion npcRot = transform.localRotation;

            if (Random.Range(-1, 1) >= 0)
            {
                forceToMove.x = transform.position.x + randToMove;
                forceToMove.z = transform.position.z;
            } else {
                
                forceToMove.x = transform.position.x;
                forceToMove.z = transform.position.z + randToMove;
            }

            if (foodScriptsOnField.Length >= 1 && eating == false && gameController.foodTime == true) {
                transform.LookAt(closestFood);

                StartCoroutine(DoMove(closestFood - transform.position * 1.5f));
            } else {
                StartCoroutine(DoMove(randomPoint()));
            }
            
        }
    }

    IEnumerator DoMove(Vector3 point)
    {

        float duration = moveDuration;
        float normalizedTime = 0;

        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            rigidBody.MovePosition(transform.position + point * Time.deltaTime * movementSpeed);
            yield return null;
        }
    }

    Vector3 randomPoint()
    {
        float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
        float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);
        Vector3 point = bounds.center + new Vector3(offsetX, 0, offsetZ);
        point.y = transform.position.y;
        return point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!dead)
        {
            GameObject collidingWith = other.gameObject;
            if (collidingWith.tag == "Pool")
            {
                inPool = true;
                rigidBody.velocity = rigidBody.velocity / 4;
                StartCoroutine(CountdownToPop());

            }
            else if (collidingWith.tag == "Grass")
            {
                inGrass = true;
                rigidBody.velocity = rigidBody.velocity / 2;
                StartCoroutine(Satisfied());

            }
            else if (collidingWith.tag == "Food")
            {
                Instantiate(toMutate,transform.position,transform.rotation);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!dead)
        {
            GameObject collidingWith = other.gameObject;
            if (collidingWith.tag == "Pool")
            {
                inPool = false;
            }
            else if (collidingWith.tag == "Grass")
            {
                inGrass = false;
            }
        }
    }

    private IEnumerator Satisfied()
    {
        float duration = grassSatisfactionTime;
        float normalizedTime = 0;

        while (normalizedTime <= 1f)
        {
            if (inGrass == true)
            {
                buffMultiplier = .5f;
            }
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        buffMultiplier = 1f;
    }

    private IEnumerator CountdownToPop()
    {
        float duration = poolCastTime;
        float normalizedTime = 0;
        float pitchValue = 2.0f;
        pitchValue = Random.Range(.5f, 2);
        audioSource.pitch = pitchValue;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        if (inPool == true && !dead)
        {
            audioSource.PlayOneShot(popSounds[Random.Range(0,popSounds.Length)]);
            for (int i = 0; i < multiplier; i++)
            {
                Instantiate(toDuplicate, new Vector3(transform.position.x + Random.Range(-randOffSetRange, randOffSetRange), offSet + Random.Range(0, randOffSetRange), transform.position.z + Random.Range(-randOffSetRange, randOffSetRange)), transform.rotation);
                rigidBody.AddForce(transform.up * popForce);
            }
        }
    }
}
