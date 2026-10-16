using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoot : MonoBehaviour
{
    public virtual void MakeDeathNoise()
    {
        GetComponent<AudioSource>().Play();
    }
    public void Die()
    {
        GetComponent<EnemyScript>()?.Die();
        GetComponentInChildren<EnemyScript>()?.Die();
    }
}
