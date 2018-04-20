using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;


        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                //m_Jump = Input.GetKey(KeyCode.W);
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.S);
            bool hold = CrossPlatformInputManager.GetButton("Fire3");       //the button for stopping your running and holding in place
            //bool m_Jump = Input.GetKey(KeyCode.W);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");      //horizontal movement axis
            float v = CrossPlatformInputManager.GetAxis("Vertical");        //vertical movement axis (really just used for aiming
            if (h != 0) {
                //Debug.Log(h.ToString());
                //Debug.Log(hold.ToString());
            }

            // Pass all parameters to the character control script.
            m_Character.Move(h, v, crouch, m_Jump, hold);
            m_Jump = false;
        }
    }
}
