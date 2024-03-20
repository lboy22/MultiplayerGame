using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float walkSpeed = 2.5f;
    public float jumpHeight = 5f;

    public Transform groundCheckTransform;
    public float groundCheckRadius = 0.2f;

    public Transform targetTransform;
    public LayerMask mouseAimMask;
    public LayerMask groundMask;

    public GameObject bulletPrefab;
    public Transform muzzleTransform;

    private float inputMovement;
    private Animator animator;
    private Rigidbody playerBody;
    private bool isGrounded;
    private Camera mainCamera;
    private float recoilTimer;

    // These variables are used to control and manipulate the sound effects heard in game.
    [SerializeField] float footstepDelay = 0.35f, lastFootstepDelay = 0f;
    [SerializeField] AudioClip footstepSound, gunshot, ready, jump, tired, deathSound;
    [SerializeField] AudioSource audioSource, gunShotSource, jumpSource, tiredSource, readySource, deathSource;

    // This dictates the player's acing direction, and the value returned is used to accomodate the use of animations.
    private int FacingSign
    {
        get
        {
            Vector3 perp = Vector3.Cross(transform.forward, Vector3.forward);
            float dir = Vector3.Dot(perp, transform.up);
            return dir > 0f ? -1 : dir < 0f ? 1 : 0;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    // Basic player movemment such as jumping, moving and shooting are processed.
    void Update()
    {
        if(playerBody.velocity.x > 0)
        {
            readySource.PlayOneShot(ready);
        }
        inputMovement = Input.GetAxis("Horizontal");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            targetTransform.position = hit.point;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerBody.velocity = new Vector3(playerBody.velocity.x, 0, 0);
            playerBody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -1 * Physics.gravity.y), ForceMode.VelocityChange);
            jumpSource.PlayOneShot(jump);
        }
        else if(Input.GetButtonDown("Jump") && !isGrounded)
        {
            tiredSource.PlayOneShot(tired);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            DeathTest();
        }
    }

    // Player' shooting action is developed.
    // Still in development.
    private void Fire()
    {
        recoilTimer = Time.time;

        var go = Instantiate(bulletPrefab);
        gunShotSource.PlayOneShot(gunshot);
        go.transform.position = muzzleTransform.position;
        var bullet = go.GetComponent<Bullet>();
        bullet.Fire(go.transform.position, muzzleTransform.eulerAngles, gameObject.layer);
    }

    private void FixedUpdate()
    {
        // Basic playre movement with animation in motion.
        playerBody.velocity = new Vector3(inputMovement * walkSpeed, playerBody.velocity.y, 0);
        animator.SetFloat("speed", (FacingSign * playerBody.velocity.x) / walkSpeed);

        // This allows for plyer rotation and still carry the animation and movement in sync.
        playerBody.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0)));

        // Through this we stop the player from jumping while already in the air.
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        animator.SetBool("isGrounded", isGrounded);

        if (Mathf.Abs(inputMovement) > 0.1f && Time.time - lastFootstepDelay > footstepDelay)
        {
            audioSource.PlayOneShot(footstepSound);
            lastFootstepDelay = Time.time;
        }
    }

    void DeathTest()
    {
        deathSource.PlayOneShot(deathSound);
        Destroy(this);
    }
}
