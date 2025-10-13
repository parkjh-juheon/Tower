using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneEnd : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName = "GameScene";

    void Start()
    {
        director.stopped += OnCutsceneEnd;
    }

    void OnCutsceneEnd(PlayableDirector obj)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
