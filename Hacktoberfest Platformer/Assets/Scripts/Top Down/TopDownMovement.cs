using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class TopDownMovement : MonoBehaviour
{
    public Vector2 movementInput {get; private set;}
    Vector2 rotationInput;
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Animator attackAnimator;
    [SerializeField]
    private PlayerInput playerInput;
    public float moveSpeed = 1f;
    [SerializeField]
    Transform weapon;
    [Range(0,1)]
    public float collisionOffset = 0.05f;
    private Animator dashAnimator;
    [SerializeField]
    public float dashSpeed = 5f;
    private bool dashInput;


    public ContactFilter2D movementFilter;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();


    ///-///////////////////////////////////////////////////////////
    ///
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    ///-///////////////////////////////////////////////////////////
    ///
    private void Update()
    {
        UpdateMovementAnimation();
        
    }

    ///-///////////////////////////////////////////////////////////
    ///
    void FixedUpdate()
    {

        if (movementInput != Vector2.zero)
        {
            //The number of objects we can collide with if we go in this direction
            int count = rb.Cast(movementInput, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

            //if nothing is in the way, move our character
            if (count == 0)
            {
                rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
            }

        }

        HandleRotation(rotationInput);

        Flip(movementInput, GetComponent<SpriteRenderer>());
    }



    ///-///////////////////////////////////////////////////////////
    ///
    void HandleRotation(Vector2 direction)
    {
        direction.Normalize();

        Vector2 v = direction;


        v = Vector2.ClampMagnitude(v, 6);
        Vector2 newLocation = (Vector2)transform.position + v;

        if (direction != Vector2.zero)
        {
            weapon.position = Vector2.Lerp(weapon.position, newLocation, 10 * Time.deltaTime);

            // Rotate towards w/ stick movement
            float zRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            weapon.rotation = Quaternion.Euler(0f, 0f, zRotation);

        }


    }


    ///-///////////////////////////////////////////////////////////
    ///
    private void Flip(Vector2 input, SpriteRenderer sr)
    {
        //Flip sprite based on player x input
        if(input.x > 0f)
        {
            sr.flipX = false;
        }
        else if(input.x < 0f)
        {
            sr.flipX = true;
        }
    }

    ///-///////////////////////////////////////////////////////////
    ///
    void UpdateMovementAnimation()
    {
        if (movementInput != Vector2.zero)
        {
            playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            playerAnimator.SetBool("isRunning", false);
        }

    }

    #region ControllerCallbacks

    //CHALLENGE: Add controller callback for any button

    ///-///////////////////////////////////////////////////////////
    ///
    public void OnLook(CallbackContext inputValue)
    {
        //Vector2 playerOffSet = new Vector2()

        if (inputValue.ReadValue<Vector2>() != Vector2.zero)
        {

            // If the current input is a mouse
            if (inputValue.control.displayName == "Delta")
            {
                // Find the position of the mouse on the screen
                Vector3 mousePos = Mouse.current.position.ReadValue();

                // Convert that mouse position to a coordinate in world space
                Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);

                rotationInput = Worldpos - transform.position;


            }
            // If the current input is a gamepad
            else if (inputValue.control.displayName == "Right Stick")
            {
                // Read rotationInput straight from the joystick movement
                rotationInput = inputValue.ReadValue<Vector2>();

            }
        }

    }
    ///-///////////////////////////////////////////////////////////
    ///
    public void OnMove(CallbackContext inputValue)
    {
        movementInput = inputValue.ReadValue<Vector2>();
    }


    ///-///////////////////////////////////////////////////////////
    ///
    public void OnFire(CallbackContext inputValue)
    {

        //Debug.LogFormat("firing");
        attackAnimator.SetBool("isAttacking", true);

    }

    ///-///////////////////////////////////////////////////////////
    ///
    public void OnDash(CallbackContext inputValue)
    {
        if (inputValue.performed)
        {
            Debug.Log("dashing at " + dashSpeed);
            // Get the movement input from the PlayerInput component
            Vector2 movementInput = inputValue.ReadValue<Vector2>();
            rb.velocity = movementInput * moveSpeed;

            Vector2 dashDirection = rb.velocity.normalized;

            // Add force to the player's rigidbody in the direction of the movement input
            rb.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);
        }
    }

    #endregion


}
