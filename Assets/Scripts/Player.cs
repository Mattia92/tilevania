using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    // Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;

    // State
    bool isAlive = true;

    // Cached component references
    Animator myAnimator;
    Collider2D myCollider;
    Rigidbody2D myRigidbody;
    float gravityScaleAtStart;

    // Start is called before the first frame update
    void Start() {
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<Collider2D>();
        myRigidbody = GetComponent<Rigidbody2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update() {
        Run();
        Jump();
        ClimbLadder();
        FlipSprite();
    }

    private void Run() {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        ChangePlayerAnimation("Running", myRigidbody.velocity.x);
    }

    private void Jump() {
        if (!myCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if (CrossPlatformInputManager.GetButtonDown("Jump")) {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder() {
        if (!myCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            ChangePlayerAnimation("Climbing", 0);
            myRigidbody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, controlThrow * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        ChangePlayerAnimation("Climbing", myRigidbody.velocity.y);
    }

    private void FlipSprite() {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    private void ChangePlayerAnimation(string animationParamName, float velocityOnAxis) {
        bool playerHasSpeed = Mathf.Abs(velocityOnAxis) > Mathf.Epsilon;
        myAnimator.SetBool(animationParamName, playerHasSpeed);
    }
}
