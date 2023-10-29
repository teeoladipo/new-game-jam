using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextFader : MonoBehaviour
{
    public Text text;
    public float fadeDuration = 1.0f;
    public float delay = 1.0f;

    void Start()
    {
        StartCoroutine(FadeText());
    }

    IEnumerator FadeText()
    {
        yield return new WaitForSeconds(delay);

        // Fade In
        for (float t = 0; t < 1.0f; t += Time.deltaTime / fadeDuration)
        {
            Color newColor = text.color;
            newColor.a = Mathf.Lerp(0, 1, t);
            text.color = newColor;
            yield return null;
        }

        yield return new WaitForSeconds(2.0f); 

        // Fade Out
        for (float t = 0; t < 1.0f; t += Time.deltaTime / fadeDuration)
        {
            Color newColor = text.color;
            newColor.a = Mathf.Lerp(1, 0, t);
            text.color = newColor;
            yield return null;
        }

        
    }
}
