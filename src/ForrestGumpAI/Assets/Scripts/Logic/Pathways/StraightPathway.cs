using UnityEngine;

public class StraightPathway : MonoBehaviour, IPathway, IPoolableObject
{
	private Transform _left = null;
	public Transform Left
	{
		get
		{
			if (_left == null) _left = transform.GetChild(0);
			return _left;
		}
	}

	private Transform _right = null;
	public Transform Right
	{
		get
		{
			if (_right == null) _right = transform.GetChild(1);
			return _right;
		}
	}

	private Transform _back = null;
	public Transform Back
	{
		get
		{
			if (_back == null) _back = transform.GetChild(2);
			return _back;
		}
	}

	public float Width
	{
		get => transform.localScale.x * 10f;
		set => transform.localScale = new Vector3(value / 10f, transform.localScale.y, transform.localScale.z);
	}
	public float Depth
	{
		get => transform.localScale.z * 10f;
		set => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value / 10f);
	}

	private void Start()
	{
		Left.gameObject.GetComponent<Wall>().AgentDied += OnAgentExit;
		Right.gameObject.GetComponent<Wall>().AgentDied += OnAgentExit;
	}

	public void SetDimension(float width = 1f, float depth = 1f)
	{
		transform.localScale = new Vector3(width / 10f, transform.localScale.y, depth / 10f);
	}

	public void OnConstruct()
	{
		gameObject.SetActive(true);
	}

	public void OnDestruct()
	{
		gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Agent"))
		{
			++AgentsInside;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Agent"))
		{
			OnAgentExit(null);
		}
	}
	private void OnAgentExit(Wall innerWall)
	{
		--AgentsInside;
		Dependency.Controller.DestructTerrain();
	}

	public int AgentsInside { get; private set; } = 0;
	public IPathway Next { get; private set; } = null;
	public void ConnectTo(Vector3 position, float rotation)
	{
		Back.gameObject.SetActive(false);
		transform.rotation = Quaternion.Euler(0f, rotation, 0f);
		transform.position = position + Depth * transform.forward / 2f;
	}
	public void ConnectOn(IPathway pathway)
	{
		var position = transform.position + Depth * transform.forward / 2f;
		var rotation = transform.rotation.eulerAngles.y;
		pathway.ConnectTo(position, rotation);
		Next = pathway;
	}
	public void Disconnect()
	{
		Back.gameObject.SetActive(true);
	}
	public void Destruct()
	{
		Next?.Disconnect();
		Next = null;
		AgentsInside = 0;
		ObjectActivator.Destruct(this);
	}
}
