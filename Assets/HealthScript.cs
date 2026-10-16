
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    public GameObject playerRef;
    public Slider slider;

    // Update is called once per frame
    void Update()
    {
        slider.value = playerRef.GetComponent<CharacterController2D>().health;
    }
}
