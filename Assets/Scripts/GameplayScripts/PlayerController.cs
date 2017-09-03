﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;
    public float runSpeed;
    public float jumpHeight;

    private float startScaleX; //Test variable for smooth flipping of character
    private bool grounded;
    private bool moveLeft;
    private bool moveRight;
    private bool stop;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startScaleX = transform.localScale.x;
    }

    // FixedUpdate is called once per frame after physics have applied
    private void FixedUpdate()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
        //Old move method replaced with one below in Update that works like the mobile version 
        //float move = Input.GetAxisRaw("Horizontal");
        //Move(move);
        #endif

        //TODO: Add Move() back here so velocity gets updated every frame (even for mobile controls) use moveLeft and moveRight to determine which way to go

    }

    //Check for key inputs every frame
    private void Update()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            MoveLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();
        if (Input.GetKeyUp(KeyCode.RightArrow))
            MoveRight();

        if (Input.GetButtonDown("Jump"))
            Jump();
        if (Input.GetKeyDown(KeyCode.LeftControl))
            gameManager.SwitchColor();
        #endif
    }

    public void MoveLeft()
    {
        if (moveRight && !moveLeft)
            Move(-1);
        else if (moveLeft && !moveRight)
            Move(0);
        else if (!moveLeft)
            Move(-1);
        else if (moveLeft && moveRight)
            Move(1);

        moveLeft = !moveLeft;
    }

    public void MoveRight()
    {
        if (moveLeft && !moveRight)
            Move(1);
        else if (moveRight && !moveLeft)
            Move(0);
        else if (!moveRight)
            Move(1);
        else if (moveLeft && moveRight)
            Move(-1);

        moveRight = !moveRight;
    }

    //Move the character left and right, also flip the sprite of the character
    public void Move(float move)
    {
        rb.velocity = new Vector2(move * runSpeed, rb.velocity.y);
        Flip(move);
    }

    //Flip the sprite of the character
    private void Flip(float move)
    {
        if (move > 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else if (move < 0)
            GetComponent<SpriteRenderer>().flipX = true;
    }

    //Default Jump Method: Uses jumpHeight from player to determine the jump strength
    private void Jump()
    {
        if(grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); //Reset velocity so you can keep bouncing on the bounce pads
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            grounded = false;
        }
    }

    //Jump Method for Bounce Pads: Uses strength from the respective bounce pad to determine the jump strength
    public void Jump(float strength)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); //Reset velocity so you can keep bouncing on the bounce pads
        rb.AddForce(new Vector2(0, strength), ForceMode2D.Impulse);
    }

    //Check when the player collides with an object
    private void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.collider.tag == "Danger")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //Code below only used for manual jump
        Vector2 contactPoint = hit.contacts[0].point;
        Vector2 center = GetComponent<Collider2D>().bounds.center;
        float offset = GetComponent<Collider2D>().bounds.extents.y;

        grounded = contactPoint.y <= center.y - offset;

    }

    //Only used for manual jump
    void OnCollisionStay2D(Collision2D hit)
    {
        Vector2 contactPoint = hit.contacts[0].point;
        Vector2 center = GetComponent<Collider2D>().bounds.center;
        float offset = GetComponent<Collider2D>().bounds.extents.y;

        grounded = contactPoint.y <= center.y - offset;
    }

    //Only used for manual jump
    void OnCollisionExit2D(Collision2D hit)
    {
        grounded = false;
    }

    //Test for smooth flipping of character direction
    //private IEnumerator Flip(float move)
    //{
    //    float t = 0;
    //    float startFlipScaleX = transform.localScale.x;
    //    float endFlipScaleX;
    //    if (move < 0)
    //        endFlipScaleX = -startScaleX;
    //    else
    //        endFlipScaleX = startScaleX;

    //    while (t < 1f)
    //    {
    //        t += Time.deltaTime * (Time.timeScale / 0.25f);
    //        transform.localScale = new Vector3(Mathf.Lerp(startFlipScaleX, endFlipScaleX, t), transform.localScale.y, transform.localScale.z);
    //        yield return 0;
    //    }
    //}
}




