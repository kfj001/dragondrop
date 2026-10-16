using JetBrains.Annotations;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
    public int m_healthRegenSeconds = 10;
    public int m_knockbackY = 4;
    public int m_knockbackX = 7;
    public float m_knockbackDuration = 1.2f;
    public float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] public float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] public float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    public bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    public LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    public Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    public Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    public Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
    public Animator m_animator;                               // The animator on the player
    public Crossfader m_crossfader;
    public AudioSource hurtSound, deathSound, fireSound;

    const float k_GroundedRadius = .4f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    bool isFacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;
    private bool knockbackLogic;
    private bool knockbackAnimation;
    private int knockback_cycles_remaining;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;
    public bool StuckCrouching { get; private set; }
    public float health = 3;
    private float lastDamagedTime;
    public float RunSpeed = 40f;

    private float hInput = 0f;
    private bool crouch;
    private bool fire;
    private bool jump;

    public void MakeShotNoise()
    {
        if (!fireSound.isPlaying)
        {
            fireSound.Play();
        }
    }

    void FixedUpdate()
    {
        if (!knockbackAnimation)
        {
            Move(hInput * Time.fixedDeltaTime, crouch, jump);
            jump = false;
            fire = false;
        }
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyScript encounteredEnemy = collision.gameObject.GetComponent<EnemyScript>();
        if (encounteredEnemy != null)
        {
            this.knockbackAnimation = true;
            this.knockbackLogic = true;
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            Collider2D collisionObject = Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround);
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (collisionObject != null && false == collisionObject.GetComponent<BoxCollider2D>()?.isTrigger)
            {
                crouch = true;
                StuckCrouching = true;
            }
            else
            {
                StuckCrouching = false;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !isFacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && isFacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    private void Update()
    {
        m_animator.SetBool("Knockback", knockbackAnimation);
        if (!knockbackAnimation && health > 0)
        {
            hInput = RunSpeed * Input.GetAxisRaw("Horizontal");
            m_animator.SetFloat("hVelocity", Mathf.Abs(Mathf.Clamp(hInput, -1, 1)));
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
            }
            if (Input.GetButtonDown("Crouch"))
            {
                crouch = true;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                crouch = false;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                fire = true;
            }

            m_animator.SetBool("isCrouching", crouch || StuckCrouching);
            m_animator.SetBool("Jump", !m_Grounded);
            m_animator.SetBool("isAttacking", fire);
        }

        if (knockbackLogic)
        {
            PlayHurtSound();
            m_animator.SetBool("Jump", false);
            knockbackLogic = false;
            m_Rigidbody2D.AddForce(new Vector2(isFacingRight ? -m_knockbackX : m_knockbackX, m_knockbackY), ForceMode2D.Impulse);
            health--;
            lastDamagedTime = Time.time;
            StartCoroutine(ClearKnockback());
        }

        if (m_GroundCheck.position.y <= 1.68f || health == 0)
        {
            if (!deathSound.isPlaying)
            {
                deathSound.Play();
            }
            health = 0;
            StartCoroutine(TransitionToBeginning());
        }

        if (health < 3 && Time.time - lastDamagedTime > m_healthRegenSeconds)
        {
            health++;
            lastDamagedTime = Time.time;
        }
    }

    public void PlayHurtSound()
    {
        if (!hurtSound.isPlaying)
        {
            hurtSound.Play();
        }
    }

    private IEnumerator TransitionToBeginning()
    {
        yield return new WaitForSeconds(1.6f);
        m_crossfader.FadeOutToScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator ClearKnockback()
    {
        yield return new WaitForSeconds(m_knockbackDuration);
        knockbackAnimation = false;
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;
        transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
    }
}