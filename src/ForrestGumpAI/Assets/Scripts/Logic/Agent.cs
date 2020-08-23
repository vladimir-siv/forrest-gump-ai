using System.Collections;
using UnityEngine;

public class Agent : MonoBehaviour, IPoolableObject
{
	[SerializeField] private float acceleration = 250f;
	[SerializeField] private float velocity = 7.5f;

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

	public bool IsRunning { get; private set; } = false;
	public bool CanSteer { get; private set; } = false;

	public void OnConstruct()
	{
		gameObject.SetActive(true);
		transform.position = Vector3.zero;
	}

	public void OnDestruct()
	{
		IsRunning = false;
		CanSteer = false;
		animator.Rebind();
		gameObject.SetActive(false);
	}

	public void Run()
	{
		if (IsRunning) return;
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

		CanSteer = true;
		Debug.Log("CanSteer");
	}

	public void Die()
	{
		body.velocity = Vector3.zero;
		animator.SetTrigger("Die");
	}
}
