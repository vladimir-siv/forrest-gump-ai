using UnityEngine;

public class PathwayConnector : MonoBehaviour, IPathway, IPoolableObject
{
	private Transform _ground = null;
	public Transform Ground
	{
		get
		{
			if (_ground == null) _ground = transform.GetChild(0);
			return _ground;
		}
	}

	private Transform _leftEnter = null;
	public Transform LeftEnter
	{
		get
		{
			if (_leftEnter == null) _leftEnter = transform.GetChild(1);
			return _leftEnter;
		}
	}

	private Transform _rightEnter = null;
	public Transform RightEnter
	{
		get
		{
			if (_rightEnter == null) _rightEnter = transform.GetChild(2);
			return _rightEnter;
		}
	}

	private Transform _leftExit = null;
	public Transform LeftExit
	{
		get
		{
			if (_leftExit == null) _leftExit = transform.GetChild(3);
			return _leftExit;
		}
	}

	private Transform _rightExit = null;
	public Transform RightExit
	{
		get
		{
			if (_rightExit == null) _rightExit = transform.GetChild(4);
			return _rightExit;
		}
	}

	private Transform _enter = null;
	public Transform Enter
	{
		get
		{
			if (_enter == null) _enter = transform.GetChild(5);
			return _enter;
		}
	}

	private float _angle = 0f;
	public float Angle
	{
		get
		{
			return _angle;
		}
		set
		{
			if (value == _angle) return;
			_angle = value;
			if (_angle > +90f) _angle = +90f;
			if (_angle < -90f) _angle = -90f;
			Adjust();
		}
	}

	private float _groundHalfScale = float.NegativeInfinity;
	private float GroundHalfScale
	{
		get
		{
			if (_groundHalfScale < 0.0f) _groundHalfScale = Ground.localScale.z * 5f;
			return _groundHalfScale;
		}
	}

	private bool generated = false;

	public void Start()
	{
		LeftEnter.GetComponent<Wall>().AgentDied += OnAgentExit;
		RightEnter.GetComponent<Wall>().AgentDied += OnAgentExit;
		LeftExit.GetComponent<Wall>().AgentDied += OnAgentExit;
		RightExit.GetComponent<Wall>().AgentDied += OnAgentExit;
	}

	public void Adjust()
	{
		var rotation = Quaternion.Euler(0f, Angle, 0f);
		var angle = Mathf.Deg2Rad * Angle;

		var sinAngle = Mathf.Sin(angle);
		var cosAngle = Mathf.Cos(angle);
		var signAngle = Mathf.Sign(angle);

		var enterScale = GroundHalfScale * (2f - cosAngle);
		var leftScale = angle >= 0f ? enterScale : GroundHalfScale;
		var rightScale = angle <= 0f ? enterScale : GroundHalfScale;

		LeftEnter .RescaleZ( leftScale);
		RightEnter.RescaleZ(rightScale);
		LeftEnter .MoveZ( leftScale / 2f - GroundHalfScale);
		RightEnter.MoveZ(rightScale / 2f - GroundHalfScale);

		LeftExit .RescaleZ(leftScale);
		RightExit.RescaleZ(rightScale);
		LeftExit .MoveZ(GroundHalfScale / 2f + (+signAngle * GroundHalfScale * (1f - cosAngle) / 2f));
		RightExit.MoveZ(GroundHalfScale / 2f + (-signAngle * GroundHalfScale * (1f - cosAngle) / 2f));

		LeftExit .MoveX(LeftEnter.localPosition.x + leftScale * sinAngle / 2f);
		RightExit.MoveX(RightEnter.localPosition.x + rightScale * sinAngle / 2f);

		LeftExit .localRotation = rotation;
		RightExit.localRotation = rotation;
	}

	public void OnConstruct()
	{
		generated = false;
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
			if (!generated)
			{
				Dependency.Controller.GenerateTerrain();
				generated = true;
			}

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
		Enter.gameObject.SetActive(false);
		transform.rotation = Quaternion.Euler(0f, rotation, 0f);
		transform.position = position + GroundHalfScale * transform.forward;
	}
	public void ConnectOn(IPathway pathway)
	{
		var leftExitEnd = LeftExit.position + LeftExit.localScale.z * LeftExit.forward / 2f;
		var rightExitEnd = RightExit.position + RightExit.localScale.z * RightExit.forward / 2f;
		var position = (leftExitEnd + rightExitEnd) / 2f;
		position.y = 0f;
		var rotation = Angle + transform.rotation.eulerAngles.y;
		pathway.ConnectTo(position, rotation);
		Next = pathway;
	}
	public void Disconnect()
	{
		Enter.gameObject.SetActive(true);
	}
	public void Destruct()
	{
		Next?.Disconnect();
		Next = null;
		AgentsInside = 0;
		ObjectActivator.Destruct(this);
	}
}
