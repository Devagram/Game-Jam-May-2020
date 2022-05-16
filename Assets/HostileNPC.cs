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
    public float movementTimer;
    public float buffMultiplier;
    public float popForce;
    public float jumpForce;
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
        rigidBody.AddForce(transform.up * popForce);
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
        StartCoroutine(DoAttack(attackPoint));
    }

    IEnumerator DoAttack(Vector3 point)
    {
        float duration = attackRate/3;
        float normalizedTime = 0;
        Vector3 positionOne = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        inAttack = true;
        transform.LookAt(point);
        rigidBody.AddForce(transform.up * jumpForce);
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            rigidBody.MovePosition(transform.position + positionOne * Time.deltaTime * movementSpeed);
            positionOne.y += normalizedTime * 2;
            yield return null;
        }
        StartCoroutine(DoMove(point));
    }
    IEnumerator DoMove(Vector3 point)
    {
        float duration = attackRate/3;
        float normalizedTime = 0;
        transform.LookAt(point);
        rigidBody.AddForce(transform.up * jumpForce);
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            rigidBody.MovePosition(transform.position + point * Time.deltaTime * movementSpeed);
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
