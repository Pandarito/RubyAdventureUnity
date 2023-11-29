using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject particleSystemPrefab;

    private ParticleSystem spawnedParticles;

    private void Start()
    {
        // Instantiate the particle system but keep it inactive initially
        spawnedParticles = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        spawnedParticles.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Activate the particle system on collision
            ActivateParticles();
        }
    }

    private void ActivateParticles()
    {
        // Set the position and rotation, then activate the particle system
        spawnedParticles.transform.position = transform.position;
        spawnedParticles.transform.rotation = Quaternion.identity;
        spawnedParticles.gameObject.SetActive(true);

        // Enable emission to start emitting particles
        var emissionModule = spawnedParticles.emission;
        emissionModule.enabled = true;

        // Play the particle system (if you want it to start emitting immediately)
        spawnedParticles.Play();

        // Optional: You can add a delay or use other conditions to control when to deactivate the particles
        // For example, you might want to deactivate them after a certain time or when a specific event occurs
        // Invoke("DeactivateParticles", 3.0f); // Deactivate after 3 seconds
    }

    private void DeactivateParticles()
    {
        // Deactivate the particle system
        spawnedParticles.gameObject.SetActive(false);
    }
}
