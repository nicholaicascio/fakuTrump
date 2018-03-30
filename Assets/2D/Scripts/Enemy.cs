using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [System.Serializable]
    public class EnemyStats
    {
        public float Health = 100f;
    }

    public EnemyStats stats = new EnemyStats();
    

    public void DamageEnemy(float damage)
    {
        stats.Health -= damage;
        if (stats.Health <= 0)
        {
            Debug.Log("KILL ENEMY");
            GameMaster.KillEnemy(this);
        }
    }
}
