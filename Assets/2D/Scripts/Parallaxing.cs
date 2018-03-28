using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {

    public Transform[] backgrounds; //the layers being paralaxed
    private float[] parallaxScales; // the "depth" they are at
    public float smoothing = 1f; //the smoothness of the paralax

    private Transform cam; //reference to camera
    private Vector3 previousCamPosition; //store the position of the camera in the previous frame

    void Awake()
    {
        cam = Camera.main.transform;
    }

    void Start()
    {
        //previous frame had current frames camera position
        previousCamPosition = cam.position;
        parallaxScales = new float[backgrounds.Length];

        //assigning the parallax scales
        for (int i=0; i < backgrounds.Length; i++){
            parallaxScales[i] = backgrounds[i].position.z*-1;
        }
    }

    private void Update()
    {
        for (int i=0; i < backgrounds.Length; i++)
        {
            //parallax is opposite of camera movement
            float parallax = (previousCamPosition.x - cam.position.x) * parallaxScales[i];
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            //create a target position which is the background's current position with it's target x position
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            //create a smooth transition
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        //set previous cam pos to cam pos at end of frame
        previousCamPosition = cam.position;
    }
}
