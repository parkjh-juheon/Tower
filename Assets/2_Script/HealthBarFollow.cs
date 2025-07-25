using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0);
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 worldPos = target.position + offset;
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
            transform.position = screenPos;
        }
    }
}
