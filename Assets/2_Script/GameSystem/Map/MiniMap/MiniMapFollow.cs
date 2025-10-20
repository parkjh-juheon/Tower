using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target;      // 플레이어 Transform
    public float fixedX = 0f;     // x축 고정 (타워의 중앙 정도)
    public Vector2 yBounds;       // 미니맵 카메라의 y 이동 범위 (최하단 ~ 최상단)
    public float fixedZ = -10f;   // 2D 카메라용 Z 위치 고정

    void LateUpdate()
    {
        if (target == null) return;

        // y좌표를 맵 범위 내로 제한
        float clampedY = Mathf.Clamp(target.position.y, yBounds.x, yBounds.y);

        // x는 고정, y만 이동
        transform.position = new Vector3(fixedX, clampedY, fixedZ);
    }
}
