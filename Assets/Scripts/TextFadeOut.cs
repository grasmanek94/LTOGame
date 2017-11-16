using System.Collections;
using UnityEngine;
using UnityEngine.UI;

class TextFadeOut : MonoBehaviour
{
    //Fade time in seconds
    public float fadeOutTime;
    private Color originalColor;
    private Text text;
    public bool faded
    {
        get;
        private set;
    }

    public void Awake()
    {
        text = GetComponent<Text>();
        originalColor = text.color;
    }
    public void OnEnable()
    {
        faded = false;
    }

    public void FadeOut()
    {
        faded = false;
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }

        faded = true;
    }
}
