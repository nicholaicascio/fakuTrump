using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class SuicideDogAI : MonoBehaviour {

    //what to chase
    public Transform target;
    //how many times per second we will update the path
    public float updateRate = 2f;

    private Seeker seeker;
    private Rigidbody2D rb;

    //the calculated path
    public Path path;

    //the AI's speed per second
    public float speed = 300f;
    public float proximityRequiredToChase = 20;
    public int attack = 10;
    public ForceMode2D fMode;
    private bool blewUp = false;
    [HideInInspector]
    public bool pathIsEnded = false;
    //public bool isDead = false;
    //the max distance from the AI to a waypoint for it to move
    public float nextWaypointDistance = 3;
    //the waypoint we are currently going toward
    private int currentWaypoint = 0;
    private Enemy myself;
    private bool searchingForPlayer = false;
    private Vector2 myLocation;
    private Vector2 targetLocation;
    private GameObject huntedPerson;
    private Animator animator;
    //private Enemy enemy;
    //Transform graphics;
    //private PlatformerCharacter2D m_Character;

    

    private void Start()
    {
        //enemy = new Enemy();
        //graphics = transform.Find("Graphics");
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //StartCoroutine(SearchForPlayer());
        huntedPerson = GameObject.FindGameObjectWithTag("Player");
        //m_Character = GetComponent<PlatformerCharacter2D>();
        myself = this.GetComponent<Enemy>();

        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        //start a new path to the target and return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);
        StartCoroutine(UpdatePath());
    }
    IEnumerator SearchForPlayer()
    {
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        huntedPerson = GameObject.FindGameObjectWithTag("Player");
        if (sResult == null)
        {
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(SearchForPlayer());
        }
        else
        {
            target = sResult.transform;
            searchingForPlayer = false;
            StartCoroutine(UpdatePath());
            yield break;
        }
    }

    IEnumerator UpdatePath()
    {
        if (!myself.stats.isDead)
        {
            if (target == null)
            {
                if (!searchingForPlayer)
                {
                    searchingForPlayer = true;
                    StartCoroutine(SearchForPlayer());
                }

            }
            if (target != null)
            {
                seeker.StartPath(transform.position, target.position, OnPathComplete);
                yield return new WaitForSeconds(1f / updateRate);
                StartCoroutine(UpdatePath());
            }
        }
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("We got a path. Did it have an error?" + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            //animator.SetFloat("vSpeed", 0);
        }

    }

    private void Update()
    {
        if (myself.stats.isDead && !blewUp)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject obj in objs)
            {
                Vector2 enemyLocation = new Vector2(obj.transform.position.x, obj.transform.position.y);
                float distanceBetween = Vector2.Distance(myLocation, enemyLocation);
                Debug.Log("There is " + distanceBetween + " between " + obj.ToString() + " and explosion!");
            }
            float distanceFromTrump = Vector2.Distance(myLocation, target.position);
            Debug.Log("There is " + distanceFromTrump + " between Trump and explosion!");
            blewUp = true;
        }
    }

    private void FixedUpdate()
    {
        if (!myself.stats.isDead)
        {
            if (target == null)
            {
                if (!searchingForPlayer)
                {
                    searchingForPlayer = true;
                    StartCoroutine(SearchForPlayer());
                }
                return;
            }

            //TODO: always look at player

            if (path == null) return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                if (pathIsEnded)
                    return;
                //Debug.Log("End of path reached.");
                pathIsEnded = true;
                return;
            }
            pathIsEnded = false;

            //find direction to next waypoint
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            dir *= speed * Time.fixedDeltaTime;

            //decide if player is "close enough" to chase
            //fpLocation = new Vector2(fp.transform.position.x, fp.transform.localPosition.y);
            myLocation = new Vector2(transform.localPosition.x, transform.localPosition.y);
            targetLocation = new Vector2(huntedPerson.transform.localPosition.x, huntedPerson.transform.localPosition.y);
            float distanceBetween = Vector2.Distance(myLocation, targetLocation);
            //Debug.Log(distanceBetween.ToString());

            if (distanceBetween <= 2.5)                                  //if they are close enough they will suicide and explode
            {
                //Debug.Log("Attacc!");
                //Player player = huntedPerson.GetComponent<Player>();
                //player.DamagePlayer(attack);
                
                myself.DamageEnemy(100);
            }
            //Move the AI
            else if (distanceBetween <= proximityRequiredToChase)       //apply the force so they can move toward player
            {
                //Debug.Log("Player within range, attacc!");
                rb.AddForce(dir, fMode);
                animator.SetFloat("vSpeed", 1);
            }
            else                                                        //if they are too far away they will wait
            {
                animator.SetFloat("vSpeed", 0);
            }

            //m_Character.Move(dir, false, false);
            float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (dist < nextWaypointDistance)
            {
                currentWaypoint++;
                return;
            }
        }

    }
}
