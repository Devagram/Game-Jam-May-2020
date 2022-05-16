using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    public float hitPoints;
    //public float movementAcceleration;
    public float movementSpeed;
    public float buffMultiplier;
    public float movementTimer;
    public float popForce;
    public float offSet;
    public float randOffSetRange;
    public float randMoveSetRange;
    public float poolCastTime;
    public float grassSatisfactionTime;
    public float despawnDelay;
    public Rigidbody rigidBody;
    public Collider genericCollider;
    public Material objMaterial;
    private foodScript[] foodScriptsOnField;
    private List<GameObject> foodOnField = new List<GameObject>();
    private List<Transform> transformsToSearch = new List<Transform>();
    private Vector3 closestFood;
    private GameObject[] foodArray;
    private GameController gameController;

    public int multiplier;
    public GameObject toDuplicate;
    public GameObject toMutate;
    public float distanceToGround;

    public bool isGroundedBool = false;

    bool inPool = false;
    bool spawned = false;
    bool inGrass = false;
    bool eating = false;
    bool gettingReadyToMove = false;
    bool foodHasBeenFound = false;
    bool dead = false;


    Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    Renderer rend;


    // Start is called before the first frame update
    void Start()
    {
        gameController = Object.FindObjectOfType<GameController>();
        rend = GetComponent<Renderer>();
        distanceToGround = genericCollider.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
       if (spawned == false)
       {
            spawned = true;
            rigidBody.AddForce(transform.up * popForce);
       }
    }

    void FixedUpdate()
    {
        isGroundedBool = isGrounded();
        if (!dead) {
            
            //Debug.Log("am i grounded?: " + isGrounded());
            Debug.Log("am i grounded?: " + isGrounded());
            if (isGrounded()) {
                if (rigidBody.velocity.magnitude > movementSpeed)
                {
                    rigidBody.velocity = rigidBody.velocity.normalized * movementSpeed;
                }
            }

            FindFood();
            if (inPool == false && gettingReadyToMove == false)
            {
                //wDebug.Log("started coroutine for movement");
                StartCoroutine(RandomMovement());
            }
            if (hitPoints <= 0)
            {
                StartCoroutine(Kill());
            }
        }
    }

    private IEnumerator Kill()
    {
        Quaternion npcRot = transform.localRotation;
        float duration = despawnDelay;
        float normalizedTime = 0;

        dead = true;
        npcRot.y -= 180; //subtract 90 degrees from its looking angle
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
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }

    private IEnumerator RandomMovement()
    {
        float duration = movementTimer;
        float normalizedTime = 0;
        float randToMove;
        //float randToMoveWO;
        Vector3 forceToMove;
        gettingReadyToMove = true;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            //Debug.Log("normalizedTime:" + normalizedTime);
            yield return null;
        }
        gettingReadyToMove = false;
        //Debug.Log(inPool);
        if (isGrounded() && inPool == false)
        {
            //Debug.Log("buffMultiplier: " + buffMultiplier);
            randToMove = Random.Range(-randMoveSetRange - 1, randMoveSetRange + 1);
            randToMove *= buffMultiplier;


            Quaternion npcRot = transform.localRotation; //temp variable to store the current rotation

            


            if (Random.Range(-1, 1) >= 0)
            {
                //Debug.Log("Applying force on right: " + randToMove + " Without multiplier:" + randToMoveWO);
                forceToMove = transform.right * randToMove;
                npcRot.y -= 90; //subtract 90 degrees from its looking angle
                transform.localRotation = npcRot; //apply the updated rotaton
            }
            else
            {
                //Debug.Log("Applying force on forward: " + randToMove + " Without multiplier:" + randToMoveWO);
                forceToMove = transform.forward * randToMove;
            }
            if (foodScriptsOnField.Length >= 1 && eating == false && gameController.foodTime == true) {
                transform.LookAt(closestFood);
                Debug.Log("numbers: " + (closestFood - transform.position));
                forceToMove = (closestFood - transform.position);
                if (forceToMove.x > 15 || forceToMove.y > 15) {
                    rigidBody.AddForce(forceToMove/6);
                }
                else if (forceToMove.x <= 15 && forceToMove.x > 8 || forceToMove.y <= 15 && forceToMove.y > 8)
                {
                    rigidBody.AddForce(forceToMove/4);
                }
                else if (forceToMove.x <= 8 && forceToMove.x > 4 || forceToMove.y <= 8 && forceToMove.y > 4)
                {
                    rigidBody.AddForce(forceToMove/2);
                } else
                {
                    rigidBody.AddForce(forceToMove);
                }
            } 
            else {
                rigidBody.AddForce(forceToMove);
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!dead)
        {
            GameObject collidingWith = other.gameObject;
            //Debug.Log("entering: " + collidingWith.tag);
            if (collidingWith.tag == "Pool")
            {
                inPool = true;
                //start timer to multiplication
                //Debug.Log("in if: " + collidingWith.tag);
                rigidBody.velocity = new Vector3(0, 0, 0);
                StartCoroutine(Countdown());

            }
            else if (collidingWith.tag == "Grass")
            {
                inGrass = true;
                rigidBody.velocity = rigidBody.velocity / 2;
                rend.material.color = Color.green;
                //start timer to multiplication
                //Debug.Log("in if: " + collidingWith.tag);
                StartCoroutine(Satisfied());

            }
            else if (collidingWith.tag == "Food")
            {
                //eating = true;
                //Eatfood
                Instantiate(toMutate,transform.position,transform.rotation);
                Destroy(this.gameObject);
            }
        }
    }

    public void Damage(float damageToTake)
    {
        hitPoints -= damageToTake;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!dead)
        {
            GameObject collidingWith = other.gameObject;
            //Debug.Log("exiting: " + collidingWith.tag);
            if (collidingWith.tag == "Pool")
            {
                inPool = false;
                rend.material.color = Color.white;
            }
            else if (collidingWith.tag == "Grass")
            {
                inGrass = false;
                rend.material.color = Color.white;
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
                rend.material.color = Color.blue;
                buffMultiplier = .5f;
                //Debug.Log("buffMultiplier: " + buffMultiplier);
            }
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        buffMultiplier = 1f;
        rend.material.color = Color.white;
    }

    private IEnumerator Countdown()
    {
        float duration = poolCastTime;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            if (inPool == true)
            {
                gradient = new Gradient();

                // Populate the color keys at the relative time 0 and 1 (0 and 100%)
                colorKey = new GradientColorKey[2];
                colorKey[0].color = Color.white;
                colorKey[0].time = 0.0f;
                colorKey[1].color = Color.red;
                colorKey[1].time = 1.0f;

                // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
                alphaKey = new GradientAlphaKey[2];
                alphaKey[0].alpha = 1.0f;
                alphaKey[0].time = 0.0f;
                alphaKey[1].alpha = 0.0f;
                alphaKey[1].time = 1.0f;

                gradient.SetKeys(colorKey, alphaKey);

                // What's the color at the relative time 0.25 (25 %) ?

                rend.material.color = gradient.Evaluate(normalizedTime);
                //Debug.Log(gradient.Evaluate(0.25f));
            }
            yield return null;
        }
        if (inPool == true && !dead)
        {
            for (int i = 0; i < multiplier; i++)
            {
                //Debug.Log("in forloop: " + i);
                Instantiate(toDuplicate, new Vector3(transform.position.x + Random.Range(0f, randOffSetRange), offSet, transform.position.z), transform.rotation);
                rigidBody.AddForce(transform.up * popForce);
            }
        }
    }
}
