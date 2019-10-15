﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Transform shootOrigin;

    public GameObject waterProjectile;
    public GameObject hookProjectile;

    public GrapplingHook hProjectile;

    public PlayerController playerScript;

    private float waterGunCooldownTimer;
    public float waterGunCooldown;
    public bool canHook;


    ///If we want to use the Input Manager
    public string watergunInput;
    public string hookInput;
    /// //////////////////////////////////


    private void Start()
    {
        //shootOrigin = transform.Find("ShootOrigin");
        waterGunCooldownTimer = 0;
        canHook = true;

        playerScript = GetComponent<PlayerController>();

        // hProjectile.GetComponent<Shoot>().castedByPlayer = player;
        watergunInput = playerScript.AButtonInput;
        hookInput = playerScript.BButtonInput;
    }

    private void Update()
    {
        waterGunCooldownTimer -= Time.deltaTime;
        //Shoot Watergun
        if (Input.GetButtonDown(watergunInput) && waterGunCooldownTimer <= 0)
        {
            ShootWaterGun();
            waterGunCooldownTimer = waterGunCooldown;
        }


        //Shoot Grappling Hook
        if (Input.GetButtonDown(hookInput))
        {
            if (hProjectile == null && canHook)
            {
                ShootHook();
                canHook = false;
            }
        }

        if (hProjectile != null)
            playerScript.shootingHook = true;
        else
            playerScript.shootingHook = false;


    }

    private void ShootWaterGun()
    {
        if (playerScript.playerStunned)
            return;

        //WaterProjectile projectile = Instantiate(waterProjectile, shootOrigin.transform.position, Quaternion.identity).GetComponent<WaterProjectile>();
        PushProjectile projectile = Instantiate(waterProjectile, new Vector3(shootOrigin.transform.position.x, shootOrigin.transform.position.y, shootOrigin.transform.position.z), Quaternion.identity).GetComponent<PushProjectile>();
        projectile.knockbackDirection = shootOrigin.forward;
        projectile.ownerPlayer = gameObject;

        //playerScript.animator.SetTrigger("Attack");
    }

    private void ShootHook()
    {
        if (playerScript.playerStunned)
            return;

        GrapplingHook projectile = Instantiate(hookProjectile, new Vector3(shootOrigin.transform.position.x, shootOrigin.transform.position.y, shootOrigin.transform.position.z), Quaternion.identity).GetComponent<GrapplingHook>();
        //projectile.Init();
        projectile.direction = shootOrigin.forward;
        projectile.hookOwner = gameObject;
        hProjectile = projectile;

        //It looks terrible, dont
        //playerScript.animator.SetTrigger("Attack");
    }


}
