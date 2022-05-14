using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//TODO ATTACKING




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
    public bool trueW = false;


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
        if (Cursor.lockState != CursorLockMode.Confined)
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
                trueW = true;
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
            trueW = false;
        }

        //ATTACKING
        if (Input.GetButtonDown("Fire1"))
        {

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
        bool waiting = false;
        //Quaternion rotation;
        //Vector3 lookPosition;
        //GameObject hitObject;
        Vector3 playerPositionNormalized = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 mousePositionNormalized = new Vector3(Input.mousePosition.x - Screen.width / 2, 0, Input.mousePosition.y - Screen.height / 2);
        Vector3 rotatedPlayerPosition = new Vector3(transform.position.x + transform.position.y / Mathf.Sqrt(2), 0, transform.position.y - transform.position.x / Mathf.Sqrt(2));
        float distanceToMouse = Vector3.Distance(playerPositionNormalized, mousePositionNormalized);
        //todo, correct for unit difference, and 45degree rotation of stage

        Debug.Log("Player position: " + playerPositionNormalized);
        Debug.Log("Player position Rotated: " + rotatedPlayerPosition);
        //Debug.Log("Mouse position: " + mousePositionNormalized);
        //Debug.Log("Distance to mouse: " + distanceToMouse);

        if (movingForward == true && distanceToMouse <= 25)
        {
            movingForward = false;
            waiting = true;
        }
        else if (waiting == true && distanceToMouse > 25 && trueW == true)
        {
            movingForward = true;
            waiting = false;
        }

        if (Physics.Raycast(ray, out hit, 100) && distanceToMouse >= 20)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 100 + new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            //Debug.DrawRay(transform.position, forward, Color.red);
            //Debug.Log("hit.distance: " + hit.distance);//why is this so inconsistent? Seems to not make sense.
            //if(hit.distance > 30f)
            transform.LookAt(new Vector3(hit.point.x, 0, hit.point.z));
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
