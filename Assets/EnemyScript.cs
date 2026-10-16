using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int HitPoints = 3;
    public Animator animator;

    public void Struck()
    {
        HitPoints--;
    }

    void Update()
    {
        if (HitPoints <= 0)
        {
            animator.SetBool("IsDying", true);
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}