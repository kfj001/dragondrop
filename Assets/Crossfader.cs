using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Crossfader : MonoBehaviour
{
    public Animator crossfadeAnimator;
    public AudioSource music;
    private string sceneName;
    private float? startingVolume;
    private bool crossfadeBegun;
    private float timeIndex = 0;

    public void FadeOutToScene(string sceneName)
    {
        this.sceneName = sceneName;
        StartCoroutine(executeFadeAsync());
    }

    public void Update()
    {
        if (music != null && crossfadeBegun)
        {
            timeIndex += Time.deltaTime;
            music.volume = Mathf.Lerp(startingVolume.GetValueOrDefault(), 0, timeIndex / 1.3f);
        }
    }

    IEnumerator executeFadeAsync()
    {
        crossfadeBegun = true;
        startingVolume = music?.volume;
        crossfadeAnimator.SetTrigger("Fadeout");
        yield return new WaitForSeconds(1.3f);
        SceneManager.LoadScene(sceneName);
    }
}