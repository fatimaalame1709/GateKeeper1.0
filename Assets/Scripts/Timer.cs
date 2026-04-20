using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimerText();
    }

    // cette fonction remet le timer à zéro
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerText();
    }

    // cette fonction met à jour le texte affiché
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = elapsedTime.ToString("F1");
        }
    }
}