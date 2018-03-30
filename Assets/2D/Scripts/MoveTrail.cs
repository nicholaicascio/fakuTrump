using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrail : MonoBehaviour {

    public int moveSpeed = 230;

    private void Update()
    {
        //Vector3 vect = new Vector3(Random.Range(0, 100), Random.Range(0, 100), 0);
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);

        Destroy(this.gameObject, 1);
    }
}
