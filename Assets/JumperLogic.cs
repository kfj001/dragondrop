using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumperLogic : EnemyScript
{
    enum JumpingState { LEFT, RIGHT };
    private JumpingState CurrentJumpState = JumpingState.LEFT;
    [Range(10, 200)] public float HorizontalJumpForce = 120f;
    [Range(10, 200)] public float VerticalJumpForce = 120f;
    public Crossfader crossfader;

    public void Triggered()
    {
        base.animator.SetTrigger("Activated");
    }

    public override void Die()
    {
        crossfader.FadeOutToScene("Endgame");
    }

    public void DeathStart()
    {
        GetComponent<AudioSource>().Play();
    }

    public void DoJump()
    {
        Rigidbody2D rigidbody2D1 = GetComponent<Rigidbody2D>();
        if (rigidbody2D1 != null && rigidbody2D1.simulated)
        {
            switch (CurrentJumpState)
            {
                case JumpingState.LEFT:

                    rigidbody2D1.velocity = new Vector2(-HorizontalJumpForce, VerticalJumpForce);
                    CurrentJumpState = JumpingState.RIGHT;
                    break;
                case JumpingState.RIGHT:
                    GetComponent<Rigidbody2D>().velocity = new Vector2(HorizontalJumpForce, VerticalJumpForce);
                    CurrentJumpState = JumpingState.LEFT;
                    break;
            }
        }
    }
}