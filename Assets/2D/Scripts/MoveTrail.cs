using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrail : MonoBehaviour {

    public int moveSpeed = 230;         //the speed at which this MoveTrail moves
    public float bulletDamage = 0;      //store the damage that this bullet is supposed to deal (before distance dropoff)
    public GameObject fp;               //the FirePoint (on the tip of the barrel, it's a child of arm which is a child of player)
    private Vector2 bulletLocation;     //store the location of the MoveTrail in space
    private Vector2 fpLocation;         //store the location of the FirePoint in space
    public int POINT_FOR_HIT = 69;
    public float scoreMultiplier = 1;

    private void Start()
    {
        fp = GameObject.FindGameObjectWithTag("FirePoint");
        if (!fp)
        {
            Debug.LogError("Could not find a child named 'FirePoint'!");
            return;
        }
        fpLocation = new Vector2(fp.transform.position.x, fp.transform.localPosition.y); //we only need to do this once
    }

    public void setScoreMultiplier(float num)
    {
        this.scoreMultiplier = num;
    }

    public float getScoreMultiplier()
    {
        return this.scoreMultiplier;
    }

    public void setDamage(float num)
    {
        bulletDamage = num; //this is so that the gun can pass it's damage to the bullet.
                            //we can use this bullet with many different guns and pass their damage to it
    }

    private void Update()
    {
        //Vector3 vect = new Vector3(Random.Range(0, 100), Random.Range(0, 100), 0);
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);                        //moves the bullet over time
        bulletLocation = new Vector2(transform.position.x, transform.position.y);               //tracks the bullet's location for use in the damage calculation
        Destroy(this.gameObject, 0.75f);                                                        //self destruct after 3/4 second
    }

    private void OnTriggerEnter2D(Collider2D collision)                                         //happens when the Collider2D hits something
    {
        float distanceBetween = Vector2.Distance(fpLocation, bulletLocation);                   //calculate how far from FirePoint to Collider2D
        float damageDealt = Mathf.Round(bulletDamage / distanceBetween);                        //divide damage by the distance
        if(damageDealt > bulletDamage)
        {
            damageDealt = bulletDamage;                                                         //damage caps out at the weapons' damage
        }
        //if(damageDealt <= 0)
        //{
            //Debug.Log("Bullet hit and would have dealt " + damageDealt + " damage!");
            //return;
        //}
        Enemy enemy = collision.GetComponent<Enemy>();                                          //this is the enemy hit by the Collider2D
        GameMaster.updateTotalDamage(damageDealt);                                              //
        GameMaster.updateTotalHits(1);                                                          //update the End of Game stats
        GameMaster.updateAccuracy();
        GameMaster.updateScore(POINT_FOR_HIT);
        enemy.DamageEnemy(damageDealt);                                                         //pass the damageDealt to the Enemy we hit
        Debug.Log("Bullet hit " + collision + " and dealt " + (damageDealt) + " damage.");
        Destroy(this.gameObject);                                                               //destroy the Enemy we hit. we could remove this for bullet penetration
    }
}
