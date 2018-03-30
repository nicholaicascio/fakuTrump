using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public float fireRate = 0;
    public float damage = 10;
    public float accuracy = 1;
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
    
    float rotZ = 0;

    private void Awake()
    {
        firePoint = transform.Find("FirePoint");
        shellPoint = transform.Find("ShellPoint");
        if (firePoint == null)
        {
            Debug.LogError("Could not find a child named 'FirePoint'!");
        }
        if (shellPoint == null)
        {
            Debug.LogError("COuld not find a child named 'ShellPoint'!");
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
    }

    private void Shoot()
    {
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
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Debug.Log("Trump hit " + hit.collider.name + " and did " + damage + " damage.");
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DamageEnemy(damage);
            }
        }
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
}
