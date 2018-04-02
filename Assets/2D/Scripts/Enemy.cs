using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    //private Animator enemyAnimator;
    //private SpearKoreanAI spearKorean;

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
            //enemyAnimator = transform.GetComponentInParent<Animator>();
            //animator.SetFloat("vSpeed", 1);
            //enemyAnimator.SetBool("isDead", true);
            Debug.Log("KILL ENEMY");
            GameMaster.updateTotalKills(1);
            GameMaster.KillEnemy(this);
        }
    }
}
