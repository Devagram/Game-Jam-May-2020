using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    public float movementAcceleration;
    public float dashForce;
    public float startingSpeed;
    private float movementSpeed;
    public float attackDamage;
    public Rigidbody rigidBody;
    public Collider capCollider;
    public Collider attackHitBox;
    public float attackDuration;
    private AudioSource audioSource;
    public AudioClip swipeSound;
    public AudioClip hitSound;
    public float dashDuration;
    public float dashCooldown;
    public float distanceToGround;
    public bool movingForward = false;
    public bool trueW = false;
    public bool playerLocked = false;
    public bool inDash = false;


    void Start()
    {
        movementSpeed = startingSpeed;
        Cursor.lockState = CursorLockMode.Confined;
        audioSource = GetComponent<AudioSource>();
        distanceToGround = capCollider.bounds.extents.y;
    }

    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Confined)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (isGrounded() && !playerLocked)
        {
            if (movingForward == true)
            {
                rigidBody.AddForce(transform.forward * movementAcceleration);
            }

            if (Input.GetButtonDown("Up"))
            {
                movingForward = true;
                trueW = true;
            }
            if (Input.GetButtonDown("Jump") && !inDash)
            {
                Debug.Log("Dash button pushed");
                playerLocked = true;
                inDash = true;
                StartCoroutine(Dash());
            }
        }
        if (Input.GetButtonUp("Up"))
        {
            movingForward = false;
            trueW = false;
        }

        //ATTACKING
        if (Input.GetButtonDown("Fire1") && attackHitBox.enabled == false)
        {
            audioSource.PlayOneShot(swipeSound, .5f);
            attackHitBox.enabled = true;
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Dash()
    {
        float duration = dashDuration;
        float normalizedTime = 0;
        
        while (normalizedTime <= 1f)
        {
            rigidBody.AddForce(transform.forward * dashForce);
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        playerLocked = false;
        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        float duration = dashCooldown;
        float normalizedTime = 0;

        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        inDash = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collidingWith = other.gameObject;
        Rigidbody rigidBodyIntersected = collidingWith.gameObject.GetComponent<Rigidbody>();
        NpcMovement npcCollidedWith;
        HostileNPC hostileCollidedWith;
        audioSource.clip = hitSound;

        if (collidingWith.tag == "hittable")
        {
            audioSource.PlayOneShot(hitSound);
            npcCollidedWith = collidingWith.gameObject.GetComponent<NpcMovement>();
            rigidBodyIntersected.AddForce(transform.forward * 5);
            npcCollidedWith.Damage(attackDamage);
            rigidBody.AddForce(transform.forward * -50);
        }
        else if (collidingWith.tag == "BadGuy")
        {
            hostileCollidedWith = collidingWith.gameObject.GetComponent<HostileNPC>();
            audioSource.PlayOneShot(hitSound);
            hostileCollidedWith.Damage(attackDamage);
            rigidBody.AddForce(transform.forward * -100);
        }
        else if (collidingWith.tag == "Grass")
        {
            movementSpeed = 7;
        }
        else if (collidingWith.tag == "Pool")
        {
            movementSpeed = 5;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject collidingWith = other.gameObject;
        Rigidbody rigidBodyIntersected = collidingWith.gameObject.GetComponent<Rigidbody>();
        if (collidingWith.tag == "Grass")
        {
            movementSpeed = startingSpeed;
        }
        if (collidingWith.tag == "Pool")
        {
            movementSpeed = startingSpeed;
        }
    }

    private IEnumerator Attack()
    {
        float duration = attackDuration;
        float normalizedTime = 0;

        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        attackHitBox.enabled = false;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (isGrounded())
        {
            if (rigidBody.velocity.magnitude > movementSpeed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * movementSpeed;
            }
        }

        if (Physics.Raycast(ray, out hit, 100))
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 100 + new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            if (!playerLocked) {
                transform.LookAt(new Vector3(hit.point.x, 0, hit.point.z));
            }
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
