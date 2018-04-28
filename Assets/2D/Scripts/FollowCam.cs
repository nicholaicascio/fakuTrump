using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float yMinClamp = -1;
    public float yMaxClamp = -1;
    public float xMinClamp = -1;
    public float xMaxClamp = -1;
    public bool isAlive = true; //turn this off when we dont want the camera to be following trump

    float nextTimeToSearch = 0;
    //Vector3 newPos;

    private void Start()
    {
        target = target.transform;
    }
    private void LateUpdate()
    {
        if (isAlive)
        {
            if (target == null)
            {
                FindPlayer();
                return;
            }
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            smoothedPosition = new Vector3(Mathf.Clamp(smoothedPosition.x, xMinClamp, xMaxClamp), Mathf.Clamp(smoothedPosition.y, yMinClamp, yMaxClamp), smoothedPosition.z);
            transform.position = smoothedPosition;
        }
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
