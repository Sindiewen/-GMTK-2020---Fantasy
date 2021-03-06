﻿using UnityEngine;
using System.IO;

public class PlayerCombatController : MonoBehaviour
{

    
    // Variables
    #region Variables


    public Rect[] attackRect; // 0 = up, 1 = down, 2 = left, 3 = right
    private Vector2 _attackDirection = Vector2.zero;    // may not be used
    private float _attackDistance;       // may not be used
    public LayerMask hitMask;
    public float invulerabilityTimer = 2;
    private float invulerabilityTimerCountdown;
    public GameObject GameOver;
    public AudioClip swordSlice;
    public AudioClip playerGotHit;

    [SerializeField] private int lastFacingDir;
    [SerializeField] private int numberOfLives;
    private int numLivesMax;
    private bool healthCreated = false;

    private Vector2 rayOrigin;

    // Private variables

    // Component references
    private PlayerInputManager inputManager;
    private PlayerMovement playerMovement;
    private BoxCollider2D box2d;
    private Animator anim;
    private AudioSource audioSource;
    // private PlayerAttributesController playerAttributes;

    [SerializeField] private fileIOManager IOManager;


    // Debuff Variables
    private bool D_Pacifism = false;
    private bool D_AnxietyNoCombat = false;

    private Vector2 attackOffset;

    #endregion


    // Private Methods
    #region private Methods

    /// <summary>
    /// Unity start method
    /// 
    /// Runs at start of initialization
    /// </summary>
    private void Start()
    {
        // Defines the component references
        inputManager = GetComponent<PlayerInputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        box2d = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        numLivesMax = numberOfLives;

        UpdateLives(false);
    }

    private void Update() 
    {
        DetectEnemyHit();

        // TODO: Check for health file discrepency
        /*
        if the aamount of health files dont correspond to the amount of lives the player has, kill
        */
    }


    // Initiate Attack
    public void initiateAttack(PlayerMovement.PLAYER_FACING_DIRECTION attackDirection)
    {
        // If not pacifised, attack
        if (!D_Pacifism || !D_AnxietyNoCombat)
        {
            // activate attack anim
            anim.SetTrigger("isAttacking");
            audioSource.PlayOneShot(swordSlice);

            // Debug.Log("Attacking");
            if (attackDirection == PlayerMovement.PLAYER_FACING_DIRECTION.UP) {
                lastFacingDir = 0;
            }
            else if (attackDirection == PlayerMovement.PLAYER_FACING_DIRECTION.DOWN) {
                lastFacingDir = 1;
            }
            else if (attackDirection == PlayerMovement.PLAYER_FACING_DIRECTION.LEFT) {
                lastFacingDir = 2;
            }
            else if (attackDirection == PlayerMovement.PLAYER_FACING_DIRECTION.RIGHT) {
                lastFacingDir = 3;
            }
            
            

            rayOrigin = (Vector2)transform.position + attackRect[lastFacingDir].center;

            // Create raycast box for checking if enemy has been hit
            RaycastHit2D[] hits = Physics2D.BoxCastAll(rayOrigin, attackRect[lastFacingDir].size, 0, _attackDirection, _attackDistance, hitMask);
            foreach (RaycastHit2D hit in hits) {
                // Debug.Log("Hit " + hit.transform.name);
                hit.transform.GetComponent<Enemy>().TakeDamage();
                Debug.Log("Player Hit" + hit.transform.name + "!");
            }
        }
    }

    public void UpdateLives(bool hit) {
        // for (int i = 0; i < numberOfLives; i++) {
        //     Debug.Log("Loop Iteration: " + i);

        //     if (IOManager.isFileExists(IOManager.mainHealthFileNames[i])) { //if file exists
        //         if (i > numberOfLives) { //and it belongs to a life that you dont have
        //             IOManager.deleteFileFromString(IOManager.mainHealthFileNames[i]); // yeet it
        //         }
        //     }
        //     else if (i < numberOfLives) { //if it doesnt exist and we actually need it
        //         IOManager.createFileFromString("Health"); //create it 
        //     }
        // }

        for (int i = 0; i < numLivesMax && !hit; i++)
        {
            // IOManager.createFileFromString("Health");
            IOManager.CreateFileFromStringNoDuplicates("Health(" + (i+1) + ")");
        }

        if(hit)
        {
            IOManager.DeleteFile("Health" + "(" + (numberOfLives+1) + ")");
        }


    }


    private void DetectEnemyHit()
    {
        // If invul gone, detect hit again
        if(invulerabilityTimerCountdown <= 0)
        {
            // Boxcast hit detection
            Vector2 hitBoxOrigin = (Vector2)transform.position;
            RaycastHit2D hit = Physics2D.BoxCast(hitBoxOrigin, (Vector2)box2d.bounds.size + new Vector2(0.5f, 0.5f), 0, Vector2.zero, 0, hitMask);
            if(hit)
            {
                // reset timer;
                Debug.Log("hit by enemy");
                audioSource.PlayOneShot(playerGotHit);
                invulerabilityTimerCountdown = invulerabilityTimer;
                Hit();
            }
        }

        // countdown timer
        if (invulerabilityTimerCountdown > 0)
        {
            invulerabilityTimerCountdown -= Time.deltaTime;
        }


        // Check for file updates
        string[] files = Directory.GetFiles(IOManager.Path, "Health(*)");

        // If file has been deleted manually, update current lives and give debuff
        if(numberOfLives != files.Length)
        {
            numberOfLives = files.Length;
            IOManager.createFileFromDebuffListRandom();
        }

        if(numberOfLives == 0 || files.Length == 0)
        {
            Die();
        }
    }



    public void Hit() {
        numberOfLives--;
        UpdateLives(true);

        if (numberOfLives == 0) {
            Die();
        }
    }

    private void Die()
    {       
        //TODO do game over stuff
        GameOver.SetActive(true);
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }



    private void OnDrawGizmos()
    {
        rayOrigin = (Vector2)transform.position + attackRect[lastFacingDir].center;
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(rayOrigin, this.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector2.zero, attackRect[lastFacingDir].size);
        Gizmos.matrix = Matrix4x4.TRS(rayOrigin + (_attackDirection.normalized * _attackDistance), this.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector2.zero, attackRect[lastFacingDir].size);
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(rayOrigin, Quaternion.identity, Vector3.one);
        Gizmos.DrawLine(Vector2.zero, _attackDirection.normalized * _attackDistance);
    }




    public bool debuffPacifist
    {
        set { D_Pacifism = value; }
    }

    public bool DebuffSetAnxietyNoCombat
    {
        set { D_AnxietyNoCombat = value; }
    }




    #endregion
}



