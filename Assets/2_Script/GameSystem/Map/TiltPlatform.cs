using UnityEngine;
using System.Collections;

public class TiltPlatform : MonoBehaviour
{
    [Header("회전 설정")]
    public float rotateAngle = 30f;        // 회전 각도
    public float rotateDuration = 0.4f;    // 회전 속도
    public float returnDelay = 1.5f;       // 원위치 대기 시간

    private bool isTilting = false;
    private Quaternion initialRot;
    private Quaternion tiltedRot;

    void Start()
    {
        initialRot = transform.rotation;
        tiltedRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, rotateAngle));

        // PlayerFallTracker 이벤트 연결
        PlayerFallTracker player = FindObjectOfType<PlayerFallTracker>();
        if (player != null)
        {
            player.OnHighFallLanded += (Collider2D hitCollider) => HandleHighFallLand(player, hitCollider);
        }
    }

    private void HandleHighFallLand(PlayerFallTracker player, Collider2D hitCollider)
    {
        // 착지한 발판이 자신이거나, 최근 높은 낙하를 했으면 높이 상관없이 회전
        if (hitCollider.GetComponentInParent<TiltPlatform>() == this || player.hasRecentHighFall)
        {
            TiltOnce();
        }
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

        // 기울이기
        while (elapsed < rotateDuration)
        {
            transform.rotation = Quaternion.Lerp(initialRot, tiltedRot, elapsed / rotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = tiltedRot;

        // 대기
        yield return new WaitForSeconds(returnDelay);

        // 복귀
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
