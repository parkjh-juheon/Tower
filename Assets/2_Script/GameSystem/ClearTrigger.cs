using UnityEngine;

public class ClearTrigger : MonoBehaviour
{
    private SceneLoader sceneLoader;

    void Start()
    {
        // 씬 로더 참조 (Hierarchy 상에 SceneLoader가 있어야 함)
        sceneLoader = FindAnyObjectByType<SceneLoader>();
    }

    void Update()
    {
        // C 키를 누르면 Clear 씬으로 이동
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene("Clear");
            }
            else
            {
                Debug.LogWarning("SceneLoader가 존재하지 않습니다! 씬에 SceneLoader 오브젝트를 추가하세요.");
            }
        }
    }
}
