﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    UnityEvent playerDied = new UnityEvent();

    public int playerNumber;

    [Header("Player Control")]
    //private float translateSpeed = 2.0f;
    //private float rotateSpeed = 2.0f;
    //public CharacterController playController;
    //private Vector3 moveDirection = Vector3.zero;
    //private float gravity = 0.4f;
    //private Vector3 angle;

    public float rotAngle = 0;
    public string HorizontalInputAxis;
    public string VerticalInputAxis;
    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    public float moveRate = 10;  // units moved per second holding down move input

    [Header("Player Die")]
    public bool isDead = false;

    public Transform respawnPosition;
    public float respawnDelay;

    // Object's Components
    private MeshRenderer boxRenderer;
    private BoxCollider boxCollider;
    public Rigidbody rb;

    void Reset()
    {
        //playController = GetComponent<CharacterController>();

        boxRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }


    // Start is called before the first frame update
    void Start()
    {
       
        // Components
        boxRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        print(transform.eulerAngles.y);
        ////move front
        //moveDirection = new Vector3(0, 0, Input.GetAxis(VerticalInput));
        //moveDirection = transform.TransformDirection(moveDirection);
        //moveDirection *= translateSpeed;

        ////rotate and change direction
        //angle = transform.eulerAngles;
        //angle.y += Input.GetAxis(HorizontalInput) * rotateSpeed;
        //transform.eulerAngles = angle;
        //moveDirection.y -= gravity * Time.deltaTime;
        //playController.Move(moveDirection * Time.deltaTime);

        /// ***********
        /// Move controls (above is obsolete)
        /// ***********

        float moveVerticalAxis = Input.GetAxis(VerticalInputAxis);
        float moveHorizontalAxis = Input.GetAxis(HorizontalInputAxis);

        //if (Input.GetAxis(HorizontalInputAxis) == 0 && Input.GetAxis(VerticalInputAxis) != 0) //if there is vertical input but no horizontal input
        //    Move(moveVerticalAxis);

        if (Input.GetAxis(VerticalInputAxis) != 0) //if there is vertical input
            Move(moveVerticalAxis);

        if (Input.GetAxis(VerticalInputAxis) != 0 && Input.GetAxis(HorizontalInputAxis) == 0) //if there is vertical input but no horizontal input
            Turn(moveVerticalAxis); //turn to face front or back

        if (Input.GetAxis(HorizontalInputAxis) != 0) //if there is horizontal input
            Turn(moveHorizontalAxis); //turn
            //Move(moveHorizontalAxis);
    }


    /// ***********
    /// Moving character methods
    /// ***********
    private void Move(float input)
    {
        // Make sure to set drag high so the sliding effect is very minimal (5 drag is acceptable for now)

        // mention this trash function automatically converts to local space
        
        rb.AddForce(Vector3.forward * input * moveRate, ForceMode.Force); 
        print("moving");
    }

    private void Turn(float input)
    {
        Vector3 from = new Vector3(0f, 0f, 1f);
        Vector3 to = new Vector3(Input.GetAxis(HorizontalInputAxis), 0f, Input.GetAxis(VerticalInputAxis));
      
        rotAngle = Vector3.SignedAngle(from, to, Vector3.up); //find the direction/angle player faces (based on world view and axis input)
        transform.eulerAngles = transform.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, -rotAngle, ref turnSmoothVelocity, turnSmoothTime); //turn the player


        // move forward in the direction it faces
        if (input>0)
            rb.AddForce(transform.forward * input * moveRate, ForceMode.Force);

        if (input<0)
            rb.AddForce(transform.forward * -input * moveRate, ForceMode.Force); //since input < 0, must make positive so that force exerted is positive

        print("turning and moving");
    }


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
            boxRenderer.enabled = false;
            boxCollider.enabled = false;
        }

        if (isDead == false)
        {
            boxRenderer.enabled = true;
            boxCollider.enabled = true;
        }

    }

    public void Die()
    {
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

