using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerScript : MonoBehaviour
{

    private Animator anim;

    public float rollCooldown;

    bool inRoll = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        //You can plug these into PlayerMovement.cs when ready. This bool is set in the animator.
        //It is using GetButton for now.
        if (Input.GetButton("Up"))
        {
            anim.SetBool("walk", true);
        }
        else
        {
            anim.SetBool("walk", false);
        }


        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("wack");
        }
        if (Input.GetButtonUp("Fire1"))
        {
            anim.ResetTrigger("wack");
        }

        if (Input.GetButtonDown("Jump") && !inRoll)
        {
            inRoll = true;
            StartCoroutine(RollCooldown());
            anim.Play("Roll");
        }
    }

    private IEnumerator RollCooldown()
    {
        float duration = rollCooldown;
        float normalizedTime = 0;

        while (normalizedTime <= 1f)
        {
            ;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        inRoll = false;
    }
}
