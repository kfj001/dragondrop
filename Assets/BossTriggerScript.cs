using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTriggerScript : MonoBehaviour
{
    public JumperLogic jumper;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<CharacterController2D>() != null)
        {
            // you hit the player.
            jumper.Triggered();
            Destroy(gameObject);
        }
    }
}
