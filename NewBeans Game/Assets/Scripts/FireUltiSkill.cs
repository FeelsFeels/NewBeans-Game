﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireUltiSkill : SkillSetManager.SkillSet
{
    Transform skillUser;

    public bool beyblading;
    public bool showVFX;

    public float knockbackRadius = 10f;
    public float knockbackStrength;
    

    public GameObject ultiPrefab;
    public GameObject fireCharacter;
    public GameObject thisUlti;

    //private GrapplingHook hook;

    public float skillLifetime;
    private SkillSetManager skillManager;
    // Start is called before the first frame update
    void Start()
    {
        skillManager = GetComponent<SkillSetManager>();
        ultiPrefab.transform.Translate(0.001f, 0.001f, 0.001f);
    }

    // Update is called once per frame
    void Update()
    {
        if (thisUlti != null)
        {
            thisUlti.transform.Rotate(0, 10, 0, Space.Self);
            thisUlti.transform.position = new Vector3(fireCharacter.transform.position.x, fireCharacter.transform.position.y + 3.5f, fireCharacter.transform.position.z);
        }
        
        if (fireCharacter.GetComponent<PlayerController>().isDead)
        {
            Destroy(thisUlti);
        }
    }

    public override void SkillAttack(SkillSetManager manager)
    {

        skillUser = manager.GetComponent<Transform>(); // Set the player
        beyblading = true;
        showVFX = true;

        StartCoroutine(EndSkill(manager));
        StartCoroutine(UpdatePlayerBehaviour());

    }

    public void Beyblade()
    {
        if (showVFX == true)
        {
            thisUlti = Instantiate(ultiPrefab, fireCharacter.transform.position, Quaternion.identity);
            showVFX = false;
            return;
        }

        int ignoreLayerMask = ~1 << LayerMask.NameToLayer("Ground");    //Raycasts on everything but ground


        List<Collider> inRange = new List<Collider>();
        inRange.AddRange(Physics.OverlapSphere(skillUser.position, knockbackRadius));
        if (inRange.Contains(skillUser.GetComponent<Collider>())) //If it includes this player,
        {
            inRange.Remove(skillUser.GetComponent<Collider>()); // remove

        }

        //Disrupts all players in range
        foreach (Collider collider in inRange)
        {
            GrapplingHook hook = GetComponent<GrapplingHook>();
            if (hook)
            {
                print("hooki");
                hook.latchedObject = null;
                hook.StartTakeBack();
                print("bye hookie");
            }

            PlayerController player = collider.GetComponent<PlayerController>();
            if (player)
            {
                Vector3 knockbackDirection = (player.transform.position - skillUser.position).normalized;
                player.GetComponent<Rigidbody>().AddForce(knockbackStrength * knockbackDirection);
            }

            PushProjectile projectile = GetComponent<PushProjectile>();
            if (collider.gameObject.tag == "PushProjectile") 
            {
                Vector3 knockbackDirection = (collider.transform.position - skillUser.position).normalized;
                collider.GetComponent<Rigidbody>().AddForce(knockbackStrength * knockbackDirection);
            }
        }
    }

    IEnumerator UpdatePlayerBehaviour()
    {
        while (beyblading) //Keep updating while this is true
        {
            Beyblade();
            
            yield return null;
        }

    }
    IEnumerator EndSkill(SkillSetManager manager)
    {
        Debug.Log("heya");
        yield return new WaitForSeconds(skillDuration);

        // Stops the beyblade
        beyblading = false;
        EndUltimate(manager);
    }
}