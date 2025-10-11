using UnityEngine;
using System.Collections;

public class TiltPlatform : MonoBehaviour
{
    [Header("회전 설정")]
    public float rotateAngle = 30f;        // 회전 각도 (음수면 반대 방향)
    public float rotateDuration = 0.4f;    // 회전 속도
    public float returnDelay = 1.5f;       // 원위치로 복귀하기 전 대기 시간

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

        // 1. 회전 (기울이기)
        while (elapsed < rotateDuration)
        {
            transform.rotation = Quaternion.Lerp(initialRot, tiltedRot, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = tiltedRot;

        // 2. 잠시 대기
        yield return new WaitForSeconds(returnDelay);

        // 3. 원래 각도로 복귀
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
