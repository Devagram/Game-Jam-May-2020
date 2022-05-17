using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileNPC : MonoBehaviour
{
    
    public Rigidbody rigidBody;
    public Collider myCollider;
    public PlayerMovement player;
    public float hitPoints;
    public float movementSpeed;
    public float maxMovementSpeed;
    public float movementTimer;
    public float buffMultiplier;
    public float popForce;
    public float jumpHeight;
    public float attackRate;
    public float coolDown;
    public bool gettingReadyToMove = false;
    public float distanceToGround;
    public bool inPool = false;
    public bool inAttack = false;
    

    public float randOffSetRange;
    public float randMoveSetRange;



    // Start is called before the first frame update
    void Start()
    {
        player = Object.FindObjectOfType<PlayerMovement>();
        rigidBody = GetComponent<Rigidbody>();
        distanceToGround = myCollider.bounds.extents.y;
        //rigidBody.AddForce(transform.up * popForce);
    }

    private void Update()
    {
        //rigidBody.AddForce(transform.up * popForce);
        if (!inAttack)
        {
            Attack();
        }
    }


    void FixedUpdate()
    {
        
        if (isGrounded())
        {
            if (rigidBody.velocity.magnitude > movementSpeed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * movementSpeed;
            }
            
        }

        if (rigidBody.velocity.magnitude > maxMovementSpeed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * maxMovementSpeed;
        }


        /* if (inPool == false && gettingReadyToMove == false)
         {
             //StartCoroutine(RandomMovement());
         }*/

        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }

    public void Damage(float damageToTake)
    {
        hitPoints -= damageToTake;
    }

    public void Attack()
    {
        //Debug.Log("player.gameObject.gameObject.transform.position: " + player.transform.position);
        Vector3 attackPoint = new Vector3(player.gameObject.gameObject.transform.position.x - transform.position.x, transform.position.y, player.gameObject.gameObject.transform.position.z - transform.position.z);
        //attackPoint = player.transform.position - attackPoint;
        transform.LookAt(attackPoint);
        StartCoroutine(DoAttack(attackPoint));
    }

    IEnumerator DoAttack(Vector3 point)
    {
        inAttack = true;

        float duration = attackRate/8;
        float normalizedTime = 0;
        float elapsedTime = 0;
        //Vector3 positionOne = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        //rigidBody.AddForce(transform.up * jumpForce);
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            //if (normalizedTime > .5f) {
            rigidBody.MovePosition(transform.position + new Vector3(0, Mathf.Lerp(transform.position.y, transform.position.y + jumpHeight, elapsedTime / duration), 0) * Time.deltaTime * movementSpeed);
            elapsedTime += Time.deltaTime;
            //}
            //positionOne.y += normalizedTime * 10;
            yield return null;
        }
        StartCoroutine(DoMove(point));
    }
    IEnumerator DoMove(Vector3 point)
    {
        float duration = attackRate;
        float normalizedTime = 0;
        Vector3 transition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //float lerpingX = 0;
        //float lerpingY = 0;
        float elapsedTime = 0;
        //float elapsedTime = 0;
        //transform.LookAt(point);
        //rigidBody.AddForce(transform.up * jumpForce);
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            transition.x = Mathf.Lerp(transform.position.x, player.gameObject.gameObject.transform.position.x, elapsedTime / duration);
            transition.z = Mathf.Lerp(transform.position.z, player.gameObject.gameObject.transform.position.z, elapsedTime / duration);
            rigidBody.MovePosition(transition * Time.deltaTime * movementSpeed);
            //rigidBody.MovePosition(transform.position + new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, point.y, elapsedTime / duration), transform.position.z) * Time.deltaTime);
            //elapsedTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        float duration = attackRate/3;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        inAttack = false;
    }
}
