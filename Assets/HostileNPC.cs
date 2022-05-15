using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileNPC : MonoBehaviour
{
    
    public Rigidbody rigidBody;
    public Collider myCollider;
    public float hitPoints;
    public float movementSpeed;
    public float movementTimer;
    public float buffMultiplier;
    public float popForce;
    bool gettingReadyToMove = false;
    public float distanceToGround;
    bool inPool = false;
    

    public float randOffSetRange;
    public float randMoveSetRange;



    // Start is called before the first frame update
    void Start()
    {
        distanceToGround = myCollider.bounds.extents.y;
        rigidBody.AddForce(transform.up * popForce);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //Debug.Log("am i grounded?: " + isGrounded());
        if (isGrounded())
        {
            if (rigidBody.velocity.magnitude > movementSpeed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * movementSpeed;
            }
            
        }

        if (inPool == false && gettingReadyToMove == false)
        {
            //wDebug.Log("started coroutine for movement");
            StartCoroutine(RandomMovement());
        }


        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
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

            if (Random.Range(-1, 1) >= 0)
            {
                //Debug.Log("Applying force on right: " + randToMove + " Without multiplier:" + randToMoveWO);
                forceToMove = transform.right * randToMove;
            }
            else
            {
                //Debug.Log("Applying force on forward: " + randToMove + " Without multiplier:" + randToMoveWO);
                forceToMove = transform.forward * randToMove;
            }

            rigidBody.AddForce(forceToMove);
 

        }
    }

}
