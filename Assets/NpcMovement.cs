using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    public float movementAcceleration = 10f;
    public float movementSpeed = 50f;
    public float movementTimer;
    public float popForce;
    public float offSet;
    public float randOffSetRange;
    public float randMoveSetRange;
    public float poolCastTime;
    public Rigidbody rigidBody;
    public Collider capCollider;
    public Material objMaterial;

    public int multiplier;
    public GameObject toDuplicate;
    public float distanceToGround;

    bool inPool = false;
    bool gettingReadyToMove = false;


    Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    Renderer rend;


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rigidBody.AddForce(transform.up * popForce);
        distanceToGround = capCollider.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (inPool == false && gettingReadyToMove == false)
        {
            //wDebug.Log("started coroutine for movement");
            StartCoroutine(RandomMovement());
        }
    }

    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }

    private IEnumerator RandomMovement()
    {
        float duration = movementTimer;
        float normalizedTime = 0;
        gettingReadyToMove = true;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            //Debug.Log("normalizedTime:" + normalizedTime);
            yield return null;
        }
        gettingReadyToMove = false;
        //Debug.Log(inPool);
        if (isGrounded() && inPool == false) {
            
            rigidBody.AddForce(transform.right * Random.Range(-randMoveSetRange - 1, randMoveSetRange + 1));
            rigidBody.AddForce(transform.forward * Random.Range(-randMoveSetRange - 1, randMoveSetRange + 1));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collidingWith = other.gameObject;
        //Debug.Log("entering: " + collidingWith.tag);
        if (collidingWith.tag == "Pool")
        {
            inPool = true;
            //start timer to multiplication
            Debug.Log("in if: " + collidingWith.tag);
            StartCoroutine(Countdown());

        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject collidingWith = other.gameObject;
        //Debug.Log("exiting: " + collidingWith.tag);
        if (collidingWith.tag == "Pool")
        {
            inPool = false;
            rend.material.color = Color.white;
        }
    }

    private IEnumerator Countdown()
    {
        float duration = poolCastTime; 
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            if (inPool == true) {
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
        if (inPool == true) {
            for (int i = 0; i < multiplier; i++)
            {
                //Debug.Log("in forloop: " + i);
                Instantiate(toDuplicate, new Vector3(transform.position.x + Random.Range(0f, randOffSetRange), offSet, transform.position.z), transform.rotation);
            }
        }
    }
}
