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

	public const float MaxRayDistance = 10f;
	public float[] RayValues { get; private set; } = new float[5];

	public event Action<Agent> AgentPreDeath;
	public event Action<Agent> AgentDeath;

	public Collider CurrentPathway;

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

		for (var i = 0; i < RayValues.Length; ++i) RayValues[i] = 1f;

		IsDead = false;
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
		if (other.CompareTag("Wall")) Die();
		if (other.CompareTag("Pathway")) CurrentPathway = other;
	}

	private void FixedUpdate()
	{
		// Human controls
		//if (Input.GetKey(KeyCode.A)) Steer(-1);
		//if (Input.GetKey(KeyCode.D)) Steer(+1);

		// AI raycaster
		if (IsDead) return;
		Vector3 direction(int i)
		{
			switch (i)
			{
				case 0: return -transform.right;
				case 1: return (transform.forward - transform.right).normalized;
				case 2: return transform.forward;
				case 3: return (transform.forward + transform.right).normalized;
				case 4: return transform.right;
			}

			return Vector3.forward;
		}
		for (var i = 0; i < RayValues.Length; ++i)
		{
			if (Physics.Raycast(transform.position, direction(i), out var hit, MaxRayDistance, ~Wall.Mask)) RayValues[i] = hit.distance / MaxRayDistance;
			else RayValues[i] = 1f;
		}
	}
}
