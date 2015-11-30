using System.Collections;
using UnityEngine;

public class Innocent : MonoBehaviour
{
    public AudioSource CollisionAudioSource;

    public GameObject CollisionParticle;

    public Rigidbody Rigidbody;
    public MeshRenderer MeshRenderer;
    public SpringJoint Spring;

    public bool Linked = false;

    public const float MovementChangeCooldownMin = 5f;
    public const float MovementChangeCooldownMax = 20f;

    public float SpeedMin;
    public float SpeedMax;

    public float CurrentCoolDown = 0f;
    public Vector3 Velocity;

    public IEnumerator ChangeColor()
    {
        float timer = 0f;
        Color originalColor = MeshRenderer.material.color;
        while (timer < 2f)
        {
            MeshRenderer.material.color = Color.Lerp(originalColor, PlayerCharacter.Instance.TargetedPoolColor, timer / 2.0f);
            yield return new WaitForSeconds(Time.deltaTime);
            timer += Time.deltaTime;
        }
    }

    void Update()
    {
        if (CurrentCoolDown < 0f)
        {
            ChangeSpeedAndDirection();
        }
        CurrentCoolDown -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        Rigidbody.AddForce(Velocity * Time.deltaTime);
    }

	void Start ()
	{
	    ChangeSpeedAndDirection();
	    Rigidbody = GetComponent<Rigidbody>();
	    Spring = GetComponent<SpringJoint>();
	    MeshRenderer = GetComponent<MeshRenderer>();
        MeshRenderer.material.color = new Color(
            Random.value,
            Random.value,
            Random.value,
            0.5f);
	}

    public void ChangeSpeedAndDirection()
    {
        CurrentCoolDown = Random.Range(5f, 10f);
        Velocity = new Vector3((Random.value > 0.5f ? 1f : -1f) * Random.Range(SpeedMin, SpeedMax),
            (Random.value > 0.5f ? 1f : -1f) * Random.Range(SpeedMin, SpeedMax),
            (Random.value > 0.5f ? 1f : -1f) * Random.Range(SpeedMin, SpeedMax));
    }

    void OnCollisionEnter(Collision col)
    {
        Innocent otherInnocent = col.gameObject.GetComponent<Innocent>();

        ParticleSystem particleSystem = 
            (Instantiate(CollisionParticle, col.contacts[0].point, Quaternion.identity) as GameObject).
            GetComponent<ParticleSystem>();
        particleSystem.startColor = (MeshRenderer.material.color + 
            otherInnocent.MeshRenderer.material.color) / 2.0f;
        particleSystem.Play();
        if (!CollisionAudioSource.isPlaying &&
            PlayerCharacter.Instance.GameStarted)
        {
            CollisionAudioSource.Play();
        }
        Destroy(particleSystem.gameObject, 3.0f);
    }
}
