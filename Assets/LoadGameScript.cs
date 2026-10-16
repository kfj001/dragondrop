using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameScript : MonoBehaviour
{
    public Crossfader crossfader;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            crossfader.FadeOutToScene("SampleScene");
        }
    }
}
