using UnityEngine;
using UnityEngine.Playables;

public class CutsceneSkip : MonoBehaviour
{
    public PlayableDirector director;

    void Update()
    {
        if (Input.anyKeyDown)
            director.Stop();
    }
}
