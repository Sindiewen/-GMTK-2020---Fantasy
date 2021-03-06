﻿using UnityEngine;
using System.Collections;

public class EnemyDash : MonoBehaviour
{
    private static int EnemyCount = 0;

    public EnemyNameSetter nameSetter;

    public int HP;
    private int maxHP;
    [Range(0, 1)] public float hpLossMultiplier = 0.25f;
    [HideInInspector] public Transform player;
    public float speed = 2f;
    private float playerDistance;
    public float minDistance = 1f;

    private BoxCollider2D boxCollider;
    private LayerMask Sword;
    private Rigidbody2D rb2d;
    private fileIOManager iOManager;


    private bool fileFound = false;

    public float dash_speed = 50f;
    public bool dashing = false;
    public float dash_timer = 3f;
    private Vector2 dash_target;
    public float dashingTime = 2f;

    void Awake()
    {
        EnemyCount++;
        // Debug.Log(enemyCount.ToString());
        iOManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<fileIOManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        maxHP = HP;

        nameSetter = transform.GetComponentInParent<EnemyNameSetter>();
        transform.name = nameSetter.getNameForObject();
        createEnemyFile();
    }

    // Gets called from instantiation
    public void createEnemyFile()
    {
        iOManager.CreateFileFromStringNoDuplicates(transform.name);
        fileFound = true;
    }

    // Update is called once per frame
    void Update()
    {

        //simply move towards player,  like a ghost maybe
        float rot_z = Mathf.Atan2(player.transform.position.y, player.transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        playerDistance = Vector2.Distance(transform.position, player.position);


        

        //dash
        if (playerDistance > 15)
        {
            dashingTime = 2;
            dashing = false;
        }

        if (playerDistance < 5 & !dashing)
        {
            StartCoroutine(Dash());
        }
        
        rb2d.MovePosition(Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime));



        // Check if the file of this enemy exists. If it doesnt, decrease the enmy health by a ton
        if (fileFound)
        {
            if (!iOManager.isFileExists(transform.name))
            {
                fileFound = false;
                HP = (int)(maxHP * hpLossMultiplier);
            }
        }
    }

    public void TakeDamage()
    {
        HP--;

        if (HP <= 0)
        {
            die();
        }
    }

    public void die()
    {
        // maybe stuff
        EnemyCount--;
        if (iOManager.isFileExists(transform.name))
            iOManager.DeleteFile(transform.name);
        Destroy(this.gameObject);
    }

    public int enemyCount
    {
        get { return EnemyCount; }
    }



    
    IEnumerator Dash()
    {
        dashing = true;
        speed = 0;
        
        yield return new WaitForSeconds(dash_timer);

        while (dashingTime > 0f)
        {
            rb2d.MovePosition(Vector2.MoveTowards(transform.position, player.position, dash_speed * Time.deltaTime));
            dashingTime -= Time.deltaTime;

            yield return null;
        }

        dashing = false;
    }
}









