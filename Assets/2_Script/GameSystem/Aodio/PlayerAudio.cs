using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("�÷��̾� ����")]
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip meleeAttackClip;
    public AudioClip shootClip;
    public AudioClip hitClip;

    [Header("�̵� ����")]
    public float walkThreshold = 0.1f;
    public float runThreshold = 2f;

    private Rigidbody rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovementSounds();
    }

    void HandleMovementSounds()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        if (isGrounded)
        {
            if (speed > runThreshold)
            {
                PlaySFX(runClip);
            }
            else if (speed > walkThreshold)
            {
                PlaySFX(walkClip);
            }
        }
    }

    public void Jump()
    {
        if (!isGrounded) return;

        isGrounded = false;
        PlaySFX(jumpClip);
    }

    public void Land()
    {
        isGrounded = true;
        PlaySFX(landClip);
    }

    public void MeleeAttack()
    {
        PlaySFX(meleeAttackClip);
    }

    public void Shoot()
    {
        PlaySFX(shootClip);
    }

    public void TakeHit()
    {
        PlaySFX(hitClip);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            AudioManager.Instance.PlaySFX(clip);
    }
}
