﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController2 : MonoBehaviour
{
    public int playerNumber;
    public int killCount;
    public int deathCount;
    public int currentScore;

    [Header("Player Control")]
    //private float translateSpeed = 2.0f;
    //private float rotateSpeed = 2.0f;
    //public CharacterController playController;
    //private Vector3 moveDirection = Vector3.zero;
    //private float gravity = 0.4f;
    //private Vector3 angle;

    public float rotAngle = 0;

    [Header("Visual Effects")]
    public GameObject playerDieEffect;
    public GameObject playerPushedEffect;
    public GameObject playerPulledEffect;

    private int ControllerNumber;
    public string HorizontalInputAxis;
    public string VerticalInputAxis;
    public string AButtonInput;
    public string BButtonInput;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    public float moveRate = 4;  // units moved per second holding down move input

    [Header("Player Die")]
    public bool isDead = false;

    public Transform respawnPosition;
    public float respawnDelay;

    public GameObject lastHitBy;

    // Object's Components
    private MeshRenderer boxRenderer;
    private CapsuleCollider boxCollider;
    public Rigidbody rb;

    //Player movement
    public Vector3 CorrectionAngle; //y should be -45... about there. This rotates the movement, such that it is somewhat parallel to camera view
    private float averageInput;

    void Reset()
    {
        //playController = GetComponent<CharacterController>();

        boxRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }


    // Start is called before the first frame update
    void Start()
    {
       
        // Components
        boxRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetControllerNumber (int controllerNo)
    {
        ControllerNumber = controllerNo; //get the controller number that will control this player (to which this script is attached to)
        HorizontalInputAxis = "Horizontal (Controller " + controllerNo + ")";
        VerticalInputAxis = "Vertical (Controller " + controllerNo + ")";
        AButtonInput = "AButton (Controller " + controllerNo + ")";
        BButtonInput = "BButton (Controller " + controllerNo + ")";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    

        /// ***********
        /// Move controls (above is obsolete)
        /// ***********

        float moveVerticalAxis = Input.GetAxis(VerticalInputAxis);
        float moveHorizontalAxis = Input.GetAxis(HorizontalInputAxis);


        if (Input.GetAxis(VerticalInputAxis) != 0 && Input.GetAxis(HorizontalInputAxis) == 0) //if there is vertical input but no horizontal input
        {
            Move(moveVerticalAxis);
            Turn(moveVerticalAxis);
        }

        if (Input.GetAxis(HorizontalInputAxis) != 0 && Input.GetAxis(VerticalInputAxis) == 0) //if there is horizontal input but no vertical input
        {
            Move(moveHorizontalAxis);
            Turn(moveHorizontalAxis);
        }

        if (Input.GetAxis(VerticalInputAxis) != 0 && Input.GetAxis(HorizontalInputAxis) != 0) //if there is vertical input AND horizontal input
        {
            averageInput = Mathf.Sqrt(moveHorizontalAxis * moveHorizontalAxis + moveVerticalAxis * moveVerticalAxis); //find the hypotenuse input; can you bELIEVE iM DOING MATH??? cause im also in disbelief

            Turn(averageInput); //turn 
            Move(averageInput);
            //print(averageInput);
        }

    

    }


    /// ***********
    /// Moving character methods
    /// ***********
    private void Move(float input)
    {
        if (input > 0)
        {
            Vector3 movement = transform.forward * input * moveRate * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }

        if (input < 0)
        {
            Vector3 movement = transform.forward * input * moveRate * Time.deltaTime;
            rb.MovePosition(rb.position - movement);  //if input is negative, make it positive
        }


    }

    private void Turn(float input)
    {
        Vector3 from = new Vector3(0f, 0f, 1f);
        Vector3 to = Quaternion.Euler(CorrectionAngle) * new Vector3(Input.GetAxis(HorizontalInputAxis), 0f, Input.GetAxis(VerticalInputAxis));
      
        rotAngle = Vector3.SignedAngle(from, to, Vector3.up) ; //find the direction/angle player faces (based on world view and axis input)
        transform.eulerAngles =  transform.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, -rotAngle, ref turnSmoothVelocity, turnSmoothTime) ; //turn the player


 

        //print("turning and moving");
    }


    // *********************************
    // Player Die
    // *********************************



    private void OnTriggerEnter(Collider other)
    {
        // If collide with "Hole", player dies.
        if (other.tag == "Hole")
        {
            isDead = true;
            GameManager.onePlayerIsKilled = true;
            Die();
        }
    }

    // This hides the player when dead, and makes it reappear when alive.
    public void HidePlayerWhenDead()
    {
        if (isDead == true)
        {
            foreach (Transform trans in transform)
            {
                SkinnedMeshRenderer smr = trans.GetComponent<SkinnedMeshRenderer>();
                if (smr)
                {
                    smr.enabled = false;
                    //boxRenderer.enabled = false;
                    boxCollider.enabled = false;

                }
            }
        }

        if (isDead == false)
        {
            foreach (Transform trans in transform)
            {
                SkinnedMeshRenderer smr = trans.GetComponent<SkinnedMeshRenderer>();
                if (smr)
                {
                    smr.enabled = true;
                    //boxRenderer.enabled = false;
                    boxCollider.enabled = true;

                }
            }
        }

    }

    public void Die()
    {
        //GrapplingHook hook = lastHitBy.GetComponent<GrapplingHook>();
        //WaterProjectile projectile = lastHitBy.GetComponent<WaterProjectile>();

        //if(hook != null)
        //{
        //    GrapplingHook hook = lastHitBy.GetComponent<GrapplingHook>();
        //    hook.hookOwner.GetComponent<PlayerController>().currentScore += GameManager.instance.killScoreToAdd;
        //    GameManager.instance.UpdateScore();
        //    hook.hookOwner = null;
        //    lastHitBy = null;
        //}
        //else if (projectile != null)
        //{
        //    WaterProjectile projectile = lastHitBy.GetComponent<WaterProjectile>();
        //    projectile.ownerPlayer.GetComponent<PlayerController>().currentScore += GameManager.instance.killScoreToAdd;
        //    GameManager.instance.UpdateScore();
        //    lastHitBy = null;
        //}

        Instantiate(playerDieEffect, gameObject.transform.position, gameObject.transform.rotation);

        if(lastHitBy != null)
            lastHitBy.GetComponent<PlayerController>().currentScore += GameManager.instance.killScoreToAdd;

        lastHitBy = null;

        GameManager.instance.UpdateScore();

        // Makes player disappear
        HidePlayerWhenDead();
        // Respawns player
        StartCoroutine(RespawnPlayer());
        
    }

    IEnumerator RespawnPlayer()
    {
        if (isDead == true)
        {
            yield return new WaitForSeconds(respawnDelay);
            gameObject.transform.position = respawnPosition.transform.position;
            isDead = false;
            HidePlayerWhenDead();
        }
    }
}

