using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public float fireRate = 0;
    public float damage = 10;
    public int ammoCount = 30; //Standard Size Magazine, 10 for California mode.
    public LayerMask toHit;

    public Transform bulletTrailPrefab;
    public Transform muzzleFlashPrefab;
    float timeToFire = 0f;
    float timeToSpawnEffect = 0f;
    public float effectSpawnRate = 10;
    Transform firePoint;

    private void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("could not find a child named 'FirePoint'!");
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
        else if (fireRate != 0){
            if (Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1/fireRate;
                Shoot();
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            reload();
        }
    }

    private void Shoot()
    {
        if (ammoCount == 0) {
            Debug.Log("Out of Ammo.");
            return;
        }
        Debug.Log("Ammo Count: " + ammoCount);

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
            damage = 40 / hit.distance; //Damage Drop off. 40 Could be replaced with a damage depending on which weapon you are using.
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Debug.Log("Trump hit " + hit.collider.name + " and did " + damage + " damage.");
        }
        ammoCount--; //Decrement Ammo Count
    }

    void Effect()
    {
        Instantiate(bulletTrailPrefab, firePoint.position, firePoint.rotation);
        Transform muzzleClone = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        muzzleClone.parent = firePoint;
        float size = Random.Range(2.5f, 3f);
        muzzleClone.localScale = new Vector3(size, size, size);
        Destroy(muzzleClone.gameObject, 0.05f);
    }

    private void reload() {
        this.ammoCount = 10; //California Magazines
        Debug.Log("Reloaded.");
    }
}
