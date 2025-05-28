using UnityEngine;

public class Bullet : GameUnit
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private TrailRenderer trailRenderer;

    private Vector3 spawnPosition;
    private float trailStartDistance = 1f;

    private void OnEnable()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false; // Tắt trail lúc mới bắn
        }
    }

    private void Update()
    {
        if (!trailRenderer.emitting) // Nếu chưa bật trail
        {
            float traveledDistance = Vector3.Distance(spawnPosition, transform.position);
            if (traveledDistance > trailStartDistance)
            {
                trailRenderer.emitting = true; // Bật trail khi đạn bay xa khỏi nòng
            }
        }
    }

    public void Shoot(Vector3 direction, float speed)
    {
        spawnPosition = transform.position;
        rb.velocity = direction * speed;  // Bắn theo đúng hướng thay vì dùng AddForce
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Xử lý va chạm
    }
}
