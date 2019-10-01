﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int playerNumber;

    public float killCountTimer;
    public float deathCountTimer;

    public int killSpreeCount;
    public int deathSpreeCount;

    [Header("Player Score")]
    public int killCount;
    public int deathCount;
    public int currentScore;

    //Player status
    public bool playerStunned;
    public float stunnedTime;   //Time passed while being stunned

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
    public bool shouldRespawn = true;
    public Transform respawnPosition;
    public float respawnDelay;
    public GameObject lastHitBy;

    // Object's Components
    Animator animator;
    CapsuleCollider capsuleCollider;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Rigidbody rb;
    Shield invincibilityShield;


    [Header("Player Movement")]
    //Player movement
    public float rotAngle = 0;
    public Vector3 CorrectionAngle; //y should be -45... about there. This rotates the movement, such that it is somewhat parallel to camera view
    public float averageInput;
    public int PressCounter = 0; //how many times you pressed the movement key/input
    public float PressCooldownTimer; //you have to press movement input again within this time in order to activate dash; countdown before reset thingy
    public float angleTolerance = 30;
    public float lastInputAngle;


    void Reset()
    {
        //playController = GetComponent<CharacterController>();
        
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }


    private void Awake()
    {
        if (PlayerPrefs.HasKey("Player "+ playerNumber))
        {
            int controllerNo = PlayerPrefs.GetInt("Player " + playerNumber);
            SetControllerNumber(controllerNo);
        }

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (ControllerNumber == 0 && currentScene.name != "Player Select")
        {
            //Testing purposes, do not forrget to erase
            //if(sceneName == "Alpha Game")
            //{
            //    return;
            //}
            this.gameObject.SetActive(false);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // Components
        animator = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        rb = GetComponent<Rigidbody>();
        invincibilityShield = GetComponentInChildren<Shield>();
    }

    public void SetControllerNumber (int controllerNo)
    {
        ControllerNumber = controllerNo; //get the controller number that will control this player (to which this script is attached to)
        HorizontalInputAxis = "Horizontal (Controller " + controllerNo + ")";
        VerticalInputAxis = "Vertical (Controller " + controllerNo + ")";
        AButtonInput = "AButton (Controller " + controllerNo + ")";
        BButtonInput = "BButton (Controller " + controllerNo + ")";
    }
    private void Update()
    {
        killCountTimer -= Time.deltaTime;
        deathCountTimer -= Time.deltaTime;

        //Stunned timing
        if (playerStunned)
        {
            stunnedTime += Time.deltaTime;
            if (stunnedTime >= 0.25f)
            {
                animator.ResetTrigger("Hit");
                playerStunned = false;
                stunnedTime = 0;
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        rb.AddForce(Physics.gravity * 2);

        /// ***********
        /// Move controls
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
            averageInput = Mathf.Sqrt(moveHorizontalAxis * moveHorizontalAxis + moveVerticalAxis * moveVerticalAxis); //find the hypotenuse input; can you bELIEVE iM DOING MATH??? cause im also in disbelief like, oh mai gawddd yessiree desu

            Turn(averageInput); //turn 
            Move(averageInput);
            //print(averageInput);
        }
        if(Input.GetAxis(HorizontalInputAxis) == 0 && Input.GetAxis(VerticalInputAxis) == 0)
        {
            animator.SetFloat("Speed", 0);
        }
        else
            animator.SetFloat("Speed", 1);

        /// ***********
        /// DASH DASH DASH
        /// if add dash, need to add a button used for dashing instead of double tap
        /// ***********

        //if (Input.GetAxis(HorizontalInputAxis) != 0 || Input.GetAxis(VerticalInputAxis) != 0 ) //if there is input
        //{
        //    //get the angle of analog stick 
        //    float angle = Mathf.Atan2(moveVerticalAxis, moveHorizontalAxis) * Mathf.Rad2Deg;

        //    if (PressCooldownTimer > 0 && PressCounter == 1) //if timer has not reset and player has already tapped movement input ONCE
        //    {
        //        //check if the angle of input is within range of tolerance or leeway of previous input

        //        //if true = Has double tapped so do STH
        //        //if (Mathf.Clamp(lastInputAngle, -angleTolerance, angleTolerance) == angle)
        //        if (Mathf.Abs(angle - lastInputAngle) <= angleTolerance)
        //        {
        //            Dash();
        //            //also reset presscounter, timer(?)
        //        }

        //        //if false = treat as first tap and set presscounter as 1, reset timer, set angle of last input
        //        //if (Mathf.Clamp(lastInputAngle, -angleTolerance, angleTolerance) != angle)
        //        if(Mathf.Abs(angle - lastInputAngle) >= angleTolerance)
        //        {
        //            PressCooldownTimer = 1.0f; //set timer for countdown till next tap or reset (which is 1s)
        //            PressCounter = 1; //increase tap counter to 1


        //            lastInputAngle = angle;
        //        }
        //    }

        //    else //if this is the FIRST tap
        //    {
        //        PressCooldownTimer = 1.0f; //set timer for countdown till next tap or reset (which is 0.5s)
        //        PressCounter += 1; //increase tap counter to 1

        //        //set angle of last input
        //        lastInputAngle = angle;
        //    }
        //}

        //if (PressCooldownTimer > 0) //if timer is more than 0
        //{
        //    PressCooldownTimer -= 1 * Time.deltaTime; //timer will count down
        //}
        //else //if time is 0 
        //{
        //    PressCounter = 0; //reset press counter
        //}
    }


    /// ***********
    /// Moving character methods
    /// ***********
    private void Move(float input)
    {
        if (playerStunned)
            return;
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
        if (playerStunned)
            return;
        Vector3 from = new Vector3(0f, 0f, 1f);
        Vector3 to = Quaternion.Euler(CorrectionAngle) * new Vector3(Input.GetAxis(HorizontalInputAxis), 0f, Input.GetAxis(VerticalInputAxis));
      
        rotAngle = Vector3.SignedAngle(from, to, Vector3.up) ; //find the direction/angle player faces (based on world view and axis input)
        transform.eulerAngles =  transform.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, -rotAngle, ref turnSmoothVelocity, turnSmoothTime) ; //turn the player

    }

    public void Hit()
    {
        animator.SetTrigger("Hit");
        playerStunned = true;
        stunnedTime = 0;
    }





    /// ***********
    /// Dashing in progress
    /// ***********

    //private void Dash()
    //{
    //    rb.AddForce(transform.forward * 10, ForceMode.Force); //push player forward
    //    print("dashing");
    //    PressCounter = 0;
    //}    

    /// *********************************
    /// Player Die
    /// *********************************

    // This hides the player when dead, and makes it reappear when alive.
    public void HidePlayerWhenDead()
    {
        if (isDead == true)
        {
            skinnedMeshRenderer.enabled = false;
            capsuleCollider.enabled = false;
        }

        if (isDead == false)
        {
            skinnedMeshRenderer.enabled = true;
            capsuleCollider.enabled = true;
            invincibilityShield.ActivateShield();
        }

    }

    public void Die()
    {
        isDead = true;

        deathCountTimer = GameManager.instance.deathCountDownTimer;

        Instantiate(playerDieEffect, gameObject.transform.position, gameObject.transform.rotation);

        if (lastHitBy != null)
        {
            GameManager.instance.OnPlayerDeath(this, lastHitBy.GetComponent<PlayerController>());
        }
        else
        {
            GameManager.instance.OnPlayerDeath(this, null);
        }

        lastHitBy = null;

        // Makes player disappear
        HidePlayerWhenDead();
        // Respawns player
        if (shouldRespawn)
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

    private void OnTriggerEnter(Collider other)
    {
        // If collide with "Hole", player dies.
        if (other.tag == "Hole")
        {
            Die();
        }
        if(other.GetComponent<IAffectedByWeight>() != null)
        {
            other.GetComponent<IAffectedByWeight>().AddWeight(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IAffectedByWeight>() != null)
        {
            other.GetComponent<IAffectedByWeight>().RemoveWeight(1);
        }
    }

}

