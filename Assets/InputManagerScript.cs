using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerScript : MonoBehaviour
{

    public Animator anim;

    public float rollCooldown;
    //This timeToReset can be set to the Animation, but right now this works.
    public float timeToResetMask = 1;
    bool inRoll = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        anim.SetLayerWeight(1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetLayerWeight(1) != 0)
        {
            ResetWeight();
        }
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
            anim.SetLayerWeight(1, 1);
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
            
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        inRoll = false;
    }


    public void ResetWeight()
    {

        //This Lerps an amount of time fading the Mask off to look more natural.
        anim.SetLayerWeight(1,Mathf.Lerp(anim.GetLayerWeight(1), 0, (1) * Time.deltaTime / timeToResetMask));
        //Debug.Log(anim.GetLayerWeight(1));
    }
}
