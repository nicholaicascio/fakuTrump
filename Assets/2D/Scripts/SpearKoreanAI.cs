using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
//using UnityStandardAssets._2D;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class SpearKoreanAI : MonoBehaviour {

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
    [HideInInspector]
    public bool pathIsEnded = false;
    public bool isDead = false;
    //the max distance from the AI to a waypoint for it to move
    public float nextWaypointDistance = 3;
    //the waypoint we are currently going toward
    private int currentWaypoint = 0;

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
        if (!isDead)
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

    public void setDead()
    {
        isDead = true;
        return;
    }

    private void FixedUpdate()
    {
        if (!isDead)
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

            if(distanceBetween <= 2.5)                                  //if they are close enough they will deal damage
            {
                Debug.Log("Attacc!");
                Player player = huntedPerson.GetComponent<Player>();
                player.DamagePlayer(attack);
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
