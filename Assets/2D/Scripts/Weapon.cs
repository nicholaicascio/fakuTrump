using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {

    public float fireRate = 0;
    public float damage = 10;
    public float accuracy = 1;
    public int ammoCount = 30; //Standard Size Magazine
    public int ammoStash = 90; //90 extra bullets total to start with.
    public float totalDamage = 0; //For End of Game Stats
    public float totalHits = 0; //For End of Game Stats
    public float totalShots = 0; //For End of Game Stats
    public float overallAccuracy = 0; //For End of Game Stats
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
    public Text accuracyGUI;

    float rotZ = 0;

    private void Awake()
    {
        ammoCountGUI = GameObject.Find("AmmoCountText").GetComponent<Text>();
        accuracyGUI = GameObject.Find("AccuracyText").GetComponent<Text>();
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
        ammoCountGUI.text = "Ammo Count: " + ammoCount + " / " + ammoStash;
        this.overallAccuracy = (this.totalHits / this.totalShots) * 100;
        accuracyGUI.text = "Accuracy: " + this.overallAccuracy.ToString("#.##") + "%"; //Keep at two decimal places.
        //This will prevent just a % from showing up if the player has garbage aim.
        if (this.overallAccuracy == 0)
        {
            accuracyGUI.text = "Accuracy: 0%";
        }
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
        this.totalShots++; //Increment total number of shots fired.
        
        //Debug.Log("shoot");
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, toHit);
        if (Time.time >= timeToSpawnEffect)
        {
            Effect();
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
        //Effect();
        Debug.DrawLine(firePointPosition, (mousePosition-firePointPosition)*100, Color.cyan);
        if (hit.collider != null)
        {
            this.totalHits++; //Increment total number of hits.
            damage = 40 / hit.distance; //Damage Drop off. 40 Could be replaced with a damage depending on which weapon you are using.
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Debug.Log("Trump hit " + hit.collider.name + " and did " + damage + " damage.");
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            this.totalDamage += this.damage; //Track total damage for end of game stats.
            if (enemy != null)
            {
                enemy.DamageEnemy(damage);
            }
        }
        ammoCount--; //Decrement Ammo Count
        Debug.Log("Ammo Count: " + ammoCount);
        Debug.Log("Ammo Stash: " + ammoStash);
    }
    
    void Effect()
    {
        //Quaternion rot = Quaternion.Euler(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);
        Instantiate(bulletTrailPrefab, firePoint.position, firePoint.rotation);
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
        if (this.ammoStash > 0 && ammoCount != 31)
        {
            if ((this.ammoStash + this.ammoCount) < 30)
            {
                this.ammoCount += this.ammoStash;
                this.ammoStash = 0;
            }
            else if (this.ammoCount == 30)
            {
                this.ammoCount = 31; //Load one in chamber.
                this.ammoStash--;
            }
            else
            {
                this.ammoStash -= (30 - this.ammoCount);
                this.ammoCount = 30;
            }
        }
        ammoCountGUI.color = Color.black;
        return this.ammoCount > 0;
    }
}
