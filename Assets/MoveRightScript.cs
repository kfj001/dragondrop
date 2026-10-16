using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightScript : MonoBehaviour
{
    [Range(1, 50)] public int MoveRate = 7;

    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.right * MoveRate;
    }
}
