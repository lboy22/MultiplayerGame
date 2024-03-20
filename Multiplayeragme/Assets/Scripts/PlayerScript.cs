using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float moveHorizontal, speed = 4f, bulletVelocity = 5f;
    [SerializeField] bool shooting = false;
    Rigidbody playerBody;
    Animator animator;
    public GameObject bullet1;
    [SerializeField] ProjectileBehaviour ProjectilePrefab;
    public Transform LaunchOffSet;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Movement();
        Shooting();
    }

    void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        playerBody.velocity = new Vector2(moveHorizontal * speed, playerBody.velocity.y);
        CheckAnimation();
    }

    void Shooting()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            shooting = true;
            Instantiate(ProjectilePrefab, LaunchOffSet.position, transform.rotation);
        }

        CheckAnimation();
    }

    void CheckAnimation()
    {
        if(speed > 0f)
        {
            animator.SetFloat("speed", playerBody.velocity.x / speed);
        }
        else if(speed < 0f)
        {

        }
        if (shooting)
        {
            animator.SetBool("shooting", true);
            // Reset the Animator's shooting boolean after a short delay or at the end of the animation.
            StartCoroutine(ResetShootingAnimation());
            shooting = false;
        }
        else
        {
            animator.SetBool("shooting", false);
        }
    }

    IEnumerator ResetShootingAnimation()
    {
        // Wait for a bit before resetting the shooting animation.
        // Adjust the wait time based on the length of your shooting animation.
        yield return new WaitForSeconds(0.5f); // Example: 0.5 seconds
        animator.SetBool("shooting", false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("" + collision.gameObject.name);
            animator.SetBool("hit", true);
            Destroy(collision.gameObject);
        }

    }
}
