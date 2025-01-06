using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    private ParticleSystem.MainModule mainModule;
    public float moveThreshold = 0.01f;
    private Vector3 lastPosition;
    private Transform target;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("ParticleSystem not found on this GameObject.");
            enabled = false;
            return;
        }

        mainModule = particleSystem.main;
        lastPosition = transform.position;

        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            target = GameManager.Instance.player.transform;
        }
        else
        {
            Debug.LogError("GameManager or Player not found. Make sure GameManager is initialized and Player is assigned.");
            enabled = false;
            return;
        }

        particleSystem.Play();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null)
        {
            Debug.LogWarning("GameManager or Player is null. Disabling ParticleController.");
            enabled = false;
            return;
        }

        transform.position = GameManager.Instance.player.transform.position;

        Vector3 movement = transform.position - lastPosition;
        float speed = movement.magnitude / Time.deltaTime;

        if (speed > moveThreshold)
        {
            MoveParticles(5f);
        }

        lastPosition = transform.position;
    }

    public void MoveParticles(float speed)
    {
        if (particleSystem == null) return;

        int particleCount = particleSystem.particleCount;
        if (particleCount == 0) return;

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        particleSystem.GetParticles(particles);

        Vector3 offset = transform.up * speed * Time.deltaTime;
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].position += offset;
        }

        particleSystem.SetParticles(particles, particleCount);
    }
}
