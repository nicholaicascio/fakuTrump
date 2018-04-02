using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {
    public static GameMaster gm;

    private void Start()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public Transform spawnPrefab;
    public int spawnDelay = 2;
    public float spawnParticleTimeOnScreen = 3f;


    //for updating player accuracy and dameg dealt in end game 
    public float totalDamage = 0; //For End of Game Stats
    public float totalHits = 0; //For End of Game Stats
    public float totalShots = 0; //For End of Game Stats
    public float overallAccuracy = 0; //For End of Game Stats
    public float totalKills = 0; //for End of Game Stats
    public float totalDeaths = 0; //for End of Game Stats

    

    public Text accuracyGUI;
    private void Awake()
    {
        accuracyGUI = GameObject.Find("AccuracyText").GetComponent<Text>();
        if(!accuracyGUI)
        {
            Debug.LogError("Could not find a child named 'AccuracyText'!");
        }
    }

    public IEnumerator RespawnPlayer()
    {
            yield return new WaitForSeconds(spawnDelay);
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);
            Destroy(clone.gameObject, spawnParticleTimeOnScreen);
            //this is where you would add spawn particles or play a sound effect
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.RespawnPlayer());
    }
    
    public static void KillEnemy(Enemy enemy)
    {
        Animator enemyAnimator = new Animator();
        //SpriteRenderer enemyRenderer = new SpriteRenderer();
        //enemyRenderer = enemy.GetComponent<SpriteRenderer>();
        enemyAnimator = enemy.GetComponent<Animator>();
        //Transform renderer = enemyRenderer.transform;
        enemyAnimator.SetBool("isDead", true);
        SpearKoreanAI spearKorean = enemy.GetComponent<SpearKoreanAI>();
        spearKorean.setDead();
        BoxCollider2D collider = enemy.GetComponent<BoxCollider2D>();
        collider.enabled = false;
        Rigidbody2D body = enemy.GetComponent<Rigidbody2D>();
        body.gravityScale = 0;
        //Destroy(enemy.gameObject);
    }

    public static void updateAccuracy()
    {
        gm.overallAccuracy = (gm.totalHits / gm.totalShots) * 100;
        gm.accuracyGUI.text = "Accuracy: " + gm.overallAccuracy.ToString("#.##") + "%"; //Keep at two decimal places.
        //This will prevent just a % from showing up if the player has garbage aim.
        return;
    }

    public static void updateTotalDamage(float num)
    {
        gm.totalDamage += num;
    }

    public static void updateTotalShots(float num)
    {
        gm.totalShots += num;
    }

    public static void updateTotalHits(float num)
    {
        gm.totalHits += num;
    }
    public static void updateTotalKills(float num)
    {
        gm.totalKills += num;
    }
    public static void updateTotalDeaths(float num)
    {
        gm.totalDeaths += num;
    }
}
