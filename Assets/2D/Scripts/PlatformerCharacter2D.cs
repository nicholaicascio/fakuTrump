using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        public float jumpDelay = 1.0f; //the delay before the character's jump happens

        Transform playerGraphics;
        ArmRotation rotator;

        //public int jumpDelay = 200;
        //public IEnumerator jumpUp()
        //{
           // yield return new WaitForSeconds(jumpDelay);
            //m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        //}

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            playerGraphics = transform.Find("Graphics");
            if(playerGraphics == null)
            {
                Debug.LogError("There is no graphics object as a child of the player");
            }
            rotator = GetComponentInChildren<ArmRotation>();
        }


        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }


        public void Move(float moveH, float moveV, bool crouch, bool jump)
        {

            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            if (moveH > 0 && moveV == 0)
            {
                //aim right
                //Debug.Log("aim right");
                rotator.SetRotation(0f);
            }
            else if(moveH > 0 && moveV > 0)
            {
                //aim up right
                //Debug.Log("aim up right");
                rotator.SetRotation(45f);
            }
            else if(moveH > 0 && moveV < 0)
            {
                //aim down right
                //Debug.Log("aim down right");
                rotator.SetRotation(315f);
            }
            else if(moveH < 0 && moveV == 0)
            {
                //aim left
                //Debug.Log("aim left");
                rotator.SetRotation(180f);
            }
            else if(moveH < 0 && moveV > 0)
            {
                //aim up left
                //Debug.Log("aim up left");
                rotator.SetRotation(135f);
            }
            else if(moveH < 0 && moveV < 0)
            {
                //aim down left
                //Debug.Log("aim down left");
                rotator.SetRotation(225f);
            }
            else if(moveH == 0 && moveV > 0)
            {
                //aim up
                //Debug.Log("aim up");
                rotator.SetRotation(90f);
            }
            else if(moveH == 0 && moveV < 0)
            {
                //aim down
                //Debug.Log("aim down");
                rotator.SetRotation(270f);
            }


            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                moveH = (crouch ? moveH * m_CrouchSpeed : moveH);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(moveH));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(moveH * m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (moveH > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (moveH < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                //StartCoroutine(WaitAMoment());
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                //PlatformerCharacter2D pc2d = new PlatformerCharacter2D();
                //pc2d.StartCoroutine(pc2d.jumpUp());
                //pc2d.StartCoroutine(pc2d.HoldOn());


            }
        }

        IEnumerator WaitAMoment()
        {
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);
            yield return new WaitForSeconds(jumpDelay);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            yield break;
        }

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = playerGraphics.localScale;
            theScale.x *= -1;
            playerGraphics.localScale = theScale;
        }
    }
    
}
