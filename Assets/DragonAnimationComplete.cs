using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DragonAnimationComplete : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DoWork());
        
    }

    IEnumerator DoWork()
    {
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene(0);
    }
}
