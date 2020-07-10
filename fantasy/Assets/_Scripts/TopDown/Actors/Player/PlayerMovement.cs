﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Enums
    #region Enums

    // Enum of current facing direction
    public enum PLAYER_FACING_DIRECTION
    {
        UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3
    };

    #endregion

    // variables
    #region Variables

    // public Variables
    // ---------------------------------------------

    [Header("Player Movement Values")]
    public float moveSpeed = 5f;                    // How fast the player will move
    public float sprintSpeed = 10f;                 // How fast the player will sprint
    [SerializeField] private PLAYER_FACING_DIRECTION facingDirection; // Stores the current facing direction
    // Private variables
    // ---------------------------------------------

    // Component references
    private PlayerInputManager inputmanager;    // Reference to the player input manager component
    private Rigidbody2D rb2d;                   // Reference to the player rigidbody component   
    //private PlayerTetherController _tether;     // Gets the tether controller reference
    [SerializeField] private Animator anim;     // Reference to child animator

    #endregion


    #region Private Methods


    /// <summary>
    /// Unity start method
    /// 
    /// Runs once when the scene starts
    /// </summary>
    private void Start()
    {
        // Initialize components
        inputmanager = GetComponent<PlayerInputManager>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Setting facing direction
        // Player moving and facing upwards, moving up diagonals left or right
        if (facingDirection == PLAYER_FACING_DIRECTION.UP)
        {
            // anim.SetFloat("Horizontal", 0);
            // anim.SetFloat("Vertical", 1);
        }
        else if (facingDirection == PLAYER_FACING_DIRECTION.DOWN)
        {
            // anim.SetFloat("Horizontal", 0);
            // anim.SetFloat("Vertical", -1);
        }
        // If player if moving and facing right, moving right diagonals up or down
        else if (facingDirection == PLAYER_FACING_DIRECTION.RIGHT)
        {
            // anim.SetFloat("Horizontal", 1);
            // anim.SetFloat("Vertical", 0);
        }
        else if (facingDirection == PLAYER_FACING_DIRECTION.LEFT)
        {
            // anim.SetFloat("Horizontal", -1);
            // anim.SetFloat("Vertical", 0);
        }
    }

    #endregion

    #region Public Methods 

    // Initiate player Input
    // ----------------------------------------------------------

    /// <summary>
    /// Initiates player movement
    /// </summary>
    public void initiatiteMovement(Vector2 moveDirection, bool isSprinting)
    {

        // Move player position based on the current player position + the direction the player is moving,
        // Then multiply that by the speed the player will move, and then multiplay that by deltaTime to ensure
        // it's moving at realtime

        

        // Player moving and facing upwards, moving up diagonals left or right
        if (moveDirection.y > 0 && (moveDirection.x <= 0.5f || moveDirection.x >= -0.5f))
        {
            facingDirection = PLAYER_FACING_DIRECTION.UP;
            // anim.SetFloat("Horizontal", 0);
            // anim.SetFloat("Vertical", 1);
        }
        else if (moveDirection.y < 0 && (moveDirection.x <= 0.5f || moveDirection.x >= -0.5f))
        {
            facingDirection = PLAYER_FACING_DIRECTION.DOWN;
            // anim.SetFloat("Horizontal", 0);
            // anim.SetFloat("Vertical", -1);
        }
        // If player if moving and facing right, moving right diagonals up or down
        else if (moveDirection.x > 0 && (moveDirection.y <= 0.5f || moveDirection.y >= -0.5f))
        {
            facingDirection = PLAYER_FACING_DIRECTION.RIGHT;
            // anim.SetFloat("Horizontal", 1);
            // anim.SetFloat("Vertical", 0);
        }
        else if (moveDirection.x < 0 && (moveDirection.y <= 0.5f || moveDirection.y >= -0.5f))
        {
            facingDirection = PLAYER_FACING_DIRECTION.LEFT;
            // anim.SetFloat("Horizontal", -1);
            // anim.SetFloat("Vertical", 0);
        }

        // Stores the temp move position
        Vector2 tempMove = Vector2.zero;

        // Defines movement - normal speed vs sprinting
        if (!isSprinting)
        {
            tempMove = rb2d.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            tempMove = rb2d.position + moveDirection * sprintSpeed * Time.fixedDeltaTime;
        }

        
        // Move the player
        if (moveDirection != Vector2.zero)// && movementAllowed(ref tempMove))
        {
            // Sets position, moves player
            rb2d.MovePosition(tempMove);
            // anim.SetTrigger("isMoving");
        }
        else
        {
            rb2d.MovePosition(transform.position);
        }


    }

    #endregion

    #region getters/setters

    // Get player facing direction
    public int FacingDirection
    {
        get { return (int)facingDirection; }
    }

    #endregion

}