using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {

    public float fireRate = 0;
    public float weaponDamage = 40;
    public int magazineSize = 30; //the number of bullets a magazine can theoretically hold. 30 is a standard size on civilian rifles
    public float lastShotTime = 0;
    public bool didHit = false;
    //float damageDealt = 0;
    //public float weaponAccuracy = 1;
    //private float accuracyMin = 0;
    //private float accuracyMax = 0;

    public int ammoCount = 30; //holds number of bullets in magazine at any given time
    public int ammoStash = 90; //90 extra bullets total to start with.
    //public float totalDamage = 0; //For End of Game Stats
    //public float totalHits = 0; //For End of Game Stats
    //public float totalShots = 0; //For End of Game Stats
    //public float overallAccuracy = 0; //For End of Game Stats
    public LayerMask toHit;

    public AudioClip gunShotAudio;
    public float gunShotVolume = 1.0f;

    public Transform bulletTrailPrefab;
    public Transform muzzleFlashPrefab;
    public Transform bulletShellPrefab;
    public float shellScale = 5;
    float timeToFire = 0f;
    float timeToSpawnEffect = 0f;
    public float effectSpawnRate = 10;
    Transform firePoint;
    Transform shellPoint;

    public Text ammoCountGUI;
    //public Text accuracyGUI;

    float rotZ = 0;

    private void Start()
    {
        //accuracyMin -= weaponAccuracy;
        //accuracyMax += weaponAccuracy;
    }

    private void Awake()
    {
        ammoCountGUI = GameObject.Find("AmmoCountText").GetComponent<Text>();
        if (!ammoCountGUI)
            {
                Debug.LogError("Could not find a child named 'AmmoCountText'!");
            }
        ammoCountGUI.color = Color.black;
        ammoCountGUI.text = "Ammo Count: " + ammoCount + " / " + ammoStash;
        firePoint = transform.Find("FirePoint");
        shellPoint = transform.Find("ShellPoint");
        if (firePoint == null)
        {
            Debug.LogError("Could not find a child named 'FirePoint'!");
        }
        if (shellPoint == null)
        {
            Debug.LogError("Could not find a child named 'ShellPoint'!");
        }

    }

    void Update()
    {
        
        //Shoot();
        if (fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else if (fireRate != 0) {
            if (Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Reload();
        }
    }

    private void Shoot()
    {
        
        if (ammoCount == 0)
        {
            ammoCountGUI.color = Color.red;
            Debug.Log("Out of Ammo.");
            return;
        }

        GameMaster.updateTotalShots(1);
        GameMaster.updateAccuracy();
        //Debug.Log("shoot");

        //this line here might solve the "bullet spray" problem. instead of taking accurate mouse position, you can add numbers to it's x and y
        //float numberX = (Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 10);

        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        //Vector2 randVect = new Vector2((Random.Range(-10, 10)), (Random.Range(-10, 10)));
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, toHit);
        //RaycastHit2D swag = Physics2D.Raycast()
        
        if (Time.time >= timeToSpawnEffect)
        {
            Effect();
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
        //Effect();
        Debug.DrawLine(firePointPosition, (mousePosition-firePointPosition)*100, Color.cyan);
        if (hit.collider != null)
        {
            this.didHit = true;
            //GameMaster.updateTotalHits(1);
            //this.totalHits++; //Increment total number of hits.
            //damageDealt = weaponDamage / hit.distance; //Damage Drop off. 40 Could be replaced with a damage depending on which weapon you are using.
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
            //Debug.Log("Trump hit " + hit.collider.name + " and did " + damageDealt + " damage.");
            //Enemy enemy = hit.collider.GetComponent<Enemy>();
            //GameMaster.updateTotalDamage(damageDealt);
            //this.totalDamage += this.damageDealt; //Track total damage for end of game stats.
            
            //if (enemy != null)
            //{
                //enemy.DamageEnemy(damageDealt);
            //}
        }
        else
        {
            this.didHit = false; //Track that the bullet did not hit the target.
        }
        this.lastShotTime = Time.time; //Track the time the gun was last shot.
        //GameMaster.updateAccuracy();
        ammoCount--; //Decrement Ammo Count
        ammoCountGUI.text = "Ammo Count: " + ammoCount + " / " + ammoStash;
        //Debug.Log("Ammo Count: " + ammoCount);
        //Debug.Log("Ammo Stash: " + ammoStash);
    }
    
    void Effect()
    {
        //Quaternion rot = Quaternion.Euler(Random.Range(-25.0f, 25.0f), Random.Range(-25.0f, 25.0f), 0);
        Transform bullet = Instantiate(bulletTrailPrefab, firePoint.position, firePoint.rotation);
        MoveTrail trail = bullet.GetComponent<MoveTrail>();
        trail.setDamage(weaponDamage);

        if (!didHit)
        {
             trail.setScoreMultiplier(1); //Reset Score Multiplier.
        }
        else if ((Time.time - this.lastShotTime) < 1) //If the time from the last shot is less than 1 second, then add to the score multiplier.
        {
             trail.setScoreMultiplier(trail.getScoreMultiplier() + (Time.time - this.lastShotTime)); //Increment Score Multiplier.
        }
        GameMaster.updateScoreMultiplier(trail.getScoreMultiplier()); //Update the score multiplier.

        //bulletTrailPrefab.rotation *= rot;

        AudioSource.PlayClipAtPoint(gunShotAudio, new Vector3(firePoint.position.x, firePoint.position.y, firePoint.position.z), gunShotVolume);

        Transform muzzleClone = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        muzzleClone.parent = firePoint;
        float size = Random.Range(5.5f, 6f);
        muzzleClone.localScale = new Vector3(size, size, size);
        Destroy(muzzleClone.gameObject, 0.05f);

        rotZ += Random.Range(0, 360);
        shellPoint.rotation = Quaternion.Euler(shellPoint.rotation.x, shellPoint.rotation.y, rotZ);
        Transform shellClone = Instantiate(bulletShellPrefab, shellPoint.position, shellPoint.rotation) as Transform;
        
        shellClone.localScale = new Vector3(shellScale, shellScale, shellScale);
        Destroy(shellClone.gameObject, 10f);
        
    }

    //Returns true if reload was successful.
    private bool Reload()
    {
        if (this.ammoStash > 0 && ammoCount != (magazineSize+1))
        {
            if ((this.ammoStash + this.ammoCount) < magazineSize)
            {
                this.ammoCount += this.ammoStash;
                this.ammoStash = 0;
            }
            else if (this.ammoCount == magazineSize)
            {
                this.ammoCount = (magazineSize+1); //Load one in chamber.
                this.ammoStash--;
            }
            else
            {
                this.ammoStash -= (magazineSize - this.ammoCount);
                this.ammoCount = magazineSize;
            }
        }
        ammoCountGUI.color = Color.black;
        ammoCountGUI.text = "Ammo Count: " + ammoCount + " / " + ammoStash;
        return this.ammoCount > 0;
    }
}
