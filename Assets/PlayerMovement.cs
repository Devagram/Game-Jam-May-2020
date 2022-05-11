using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementAcceleration = 10f;
    public float jumpForce = 100f;
    public float movementSpeed = 50f;
    public Rigidbody rigidBody;
    public Collider capCollider;
    //public GameObject groundPlain;

    public float distanceToGround;


    public bool movingForward = false;


    Vector3 movement;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        distanceToGround = capCollider.bounds.extents.y;
    }

    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Cursor.lockState != CursorLockMode.Confined)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (isGrounded()) 
        {
            if (movingForward == true)
            {
                rigidBody.AddForce(transform.forward * movementAcceleration);
            }

            if (Input.GetButtonDown("Up"))
            {
                //Debug.Log("Forward button pushed");
                movingForward = true;
            }
            if (Input.GetButtonDown("Jump"))
            {
                //Debug.Log("Jump button pushed");
                rigidBody.AddForce(transform.up * jumpForce);
            }
        }
        if (Input.GetButtonUp("Up"))
        {
            //Debug.Log("Forward button released");
            movingForward = false;
        }


    }

    void FixedUpdate()
    {
        //Debug.Log("Grounded = " + isGrounded());
        if (isGrounded())
        {
            if (rigidBody.velocity.magnitude > movementSpeed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * movementSpeed;
            }
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Quaternion rotation;
        //Vector3 lookPosition;
        //GameObject hitObject;


        if (Physics.Raycast(ray, out hit, 100))
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 100 + new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            Debug.DrawRay(transform.position, forward, Color.red);
            //Debug.Log("hit.distance: " + hit.distance);//why is this so inconsistent? Seems to not make sense.
            //if(hit.distance > 30f)
            transform.LookAt(new Vector3(hit.point.x, 0, hit.point.z));
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
