using UnityEngine;
using TMPro;

public class SubtitleController : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;

    public void ShowSubtitle(string text)
    {
        subtitleText.text = text;
        subtitleText.gameObject.SetActive(true);
    }

    public void HideSubtitle()
    {
        subtitleText.gameObject.SetActive(false);
    }
}
