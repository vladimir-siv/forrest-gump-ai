using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class Agent : MonoBehaviour, IPoolableObject
{
	[SerializeField] private float acceleration = 250f;
	[SerializeField] private float velocity = 7.5f;
	[SerializeField] private float rotationSpeed = 75f;

	private Rigidbody _body = null;
	public Rigidbody body
	{
		get
		{
			if (_body == null) _body = GetComponent<Rigidbody>();
			return _body;
		}
	}

	private Animator _animator = null;
	public Animator animator
	{
		get
		{
			if (_animator == null) _animator = transform.GetChild(0).GetComponent<Animator>();
			return _animator;
		}
	}

	public bool IsDead { get; private set; } = false;
	public bool IsRunning { get; private set; } = false;
	public bool CanSteer { get; private set; } = false;
	public float BuiltVelocity { get; private set; } = 0f;

	public event Action<Agent> AgentPreDeath;
	public event Action<Agent> AgentDeath;

	public IPathway LastPathway { get; private set; } = null;
	public int PathwaysEncountered { get; private set; } = 0;

	public void OnConstruct()
	{
		if (transform.childCount == 0)
		{
			var agentModelIndex = Random.Range(0, Dependency.Controller.AgentModels.Length);
			var agentModel = Dependency.Controller.AgentModels[agentModelIndex];
			var model = Instantiate(agentModel);
			model.transform.SetParent(transform);
			model.transform.localPosition = Vector3.zero;
		}

		IsDead = false;
		LastPathway = null;
		PathwaysEncountered = 0;
		gameObject.SetActive(true);
	}
	public void OnDestruct()
	{
		animator.Rebind();
		gameObject.SetActive(false);
		AgentPreDeath = null;
		AgentDeath = null;
	}

	public void Run()
	{
		if (IsDead || IsRunning) return;
		IsRunning = true;
		animator.SetTrigger("Run");
		StartCoroutine("Accelerate");
	}

	private IEnumerator Accelerate()
	{
		while (body.velocity.magnitude < velocity)
		{
			body.AddForce(transform.forward * acceleration * Time.fixedDeltaTime);
			yield return Timing.FixedUpdate;
		}

		BuiltVelocity = body.velocity.magnitude;
		CanSteer = true;
	}

	public void Steer(int factor)
	{
		if (!CanSteer) return;
		transform.Rotate(0f, factor * Time.fixedDeltaTime * rotationSpeed, 0f);
		body.velocity = transform.forward * BuiltVelocity;
	}

	public void Die()
	{
		if (IsDead) return;
		IsDead = true;
		IsRunning = false;
		CanSteer = false;
		BuiltVelocity = 0f;
		body.velocity = Vector3.zero;
		animator.SetTrigger("Die");
		AgentPreDeath?.Invoke(this);
		AgentDeath?.Invoke(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Wall"))
		{
			Die();
			return;
		}

		if (other.CompareTag("Pathway"))
		{
			var pathway = other.GetComponent<IPathway>();

			if (LastPathway == null || pathway == LastPathway.Next)
			{
				++PathwaysEncountered;
				LastPathway = pathway;
			}

			return;
		}
	}

#if HUMAN_PLAYER
	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.A)) Steer(-1);
		if (Input.GetKey(KeyCode.D)) Steer(+1);
	}
#endif
}
