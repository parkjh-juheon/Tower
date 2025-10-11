using UnityEngine;
using System.Collections;

public class TiltPlatform : MonoBehaviour
{
    [Header("ȸ�� ����")]
    public float rotateAngle = 30f;        // ȸ�� ���� (������ �ݴ� ����)
    public float rotateDuration = 0.4f;    // ȸ�� �ӵ�
    public float returnDelay = 1.5f;       // ����ġ�� �����ϱ� �� ��� �ð�

    private bool isTilting = false;
    private Quaternion initialRot;
    private Quaternion tiltedRot;

    void Start()
    {
        initialRot = transform.rotation;
        tiltedRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, rotateAngle));
    }

    public void TiltOnce()
    {
        if (!isTilting)
            StartCoroutine(TiltRoutine());
    }

    private IEnumerator TiltRoutine()
    {
        isTilting = true;
        float elapsed = 0f;

        // 1. ȸ�� (����̱�)
        while (elapsed < rotateDuration)
        {
            transform.rotation = Quaternion.Lerp(initialRot, tiltedRot, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = tiltedRot;

        // 2. ��� ���
        yield return new WaitForSeconds(returnDelay);

        // 3. ���� ������ ����
        elapsed = 0f;
        while (elapsed < rotateDuration)
        {
            transform.rotation = Quaternion.Lerp(tiltedRot, initialRot, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = initialRot;

        isTilting = false;
    }
}
