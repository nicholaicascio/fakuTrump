using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [System.Serializable]
	public class PlayerStats
    {
        public float Health = 100f;
    }

    public PlayerStats playerStats = new PlayerStats();
    public int fallBoundary = -20;

    //public FollowCam followCam = new FollowCam();

    private void Update()
    {
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
            Debug.Log("KILL PLAYER");
            GameMaster.KillPlayer(this);
        }
    }
}
