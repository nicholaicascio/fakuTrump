using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float yClampingRestriction = -1;


    float nextTimeToSearch = 0;
    //Vector3 newPos;

    private void Start()
    {
        target = target.transform;
    }
    private void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        smoothedPosition = new Vector3(smoothedPosition.x, Mathf.Clamp(smoothedPosition.y, yClampingRestriction, Mathf.Infinity), smoothedPosition.z);
        transform.position = smoothedPosition;
    }

    void FindPlayer()
    {
        if (nextTimeToSearch <= Time.time)
        {
            GameObject searchResult = GameObject.FindGameObjectWithTag("Player");
            if (searchResult != null)
            {
                target = searchResult.transform;
            }
            nextTimeToSearch = Time.time + 0.5f;
        }
    }
}
