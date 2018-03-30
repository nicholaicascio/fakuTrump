using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    [System.Serializable]
	public class PlayerStats
    {
        public float Health = 100f;
    }

    public PlayerStats playerStats = new PlayerStats();
    public int fallBoundary = -20;
    public Text healthGUI;
    
    //public FollowCam followCam = new FollowCam();

    private void Update()
    {
        healthGUI = GameObject.Find("HealthText").GetComponent<Text>();
        healthGUI.text = "Health: " + playerStats.Health;
        if (transform.position.y <= fallBoundary)
        {
            DamagePlayer(9999);
        }
    }

    public void DamagePlayer(int damage)
    {
        playerStats.Health -= damage;
        if (playerStats.Health <= 0)
        {
            healthGUI.text = "Health: 0";
            Debug.Log("KILL PLAYER");
            GameMaster.KillPlayer(this);
        }
    }
}
