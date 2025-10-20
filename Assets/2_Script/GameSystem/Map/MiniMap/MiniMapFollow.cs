using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target;      // �÷��̾� Transform
    public float fixedX = 0f;     // x�� ���� (Ÿ���� �߾� ����)
    public Vector2 yBounds;       // �̴ϸ� ī�޶��� y �̵� ���� (���ϴ� ~ �ֻ��)
    public float fixedZ = -10f;   // 2D ī�޶�� Z ��ġ ����

    void LateUpdate()
    {
        if (target == null) return;

        // y��ǥ�� �� ���� ���� ����
        float clampedY = Mathf.Clamp(target.position.y, yBounds.x, yBounds.y);

        // x�� ����, y�� �̵�
        transform.position = new Vector3(fixedX, clampedY, fixedZ);
    }
}
