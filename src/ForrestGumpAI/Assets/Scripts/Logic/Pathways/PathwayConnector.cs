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

	private bool firstEntered = false;
	private int exited = 0;

	public void Adjust()
	{
		var rotation = Quaternion.Euler(0f, Angle, 0f);
		var angle = Mathf.Deg2Rad * Angle;

		var leftEnterRootPosition  = - (Vector3.forward + Vector3.right) * GroundHalfScale / 2f + LeftEnter .localUp() * LeftEnter .localScale.y / 2f;
		var rightEnterRootPosition = - (Vector3.forward - Vector3.right) * GroundHalfScale / 2f + RightEnter.localUp() * RightEnter.localScale.y / 2f;

		LeftEnter .RescaleZ(GroundHalfScale * (Angle > 0 ? 2f : 1f));
		RightEnter.RescaleZ(GroundHalfScale * (Angle < 0 ? 2f : 1f));

		LeftEnter .MoveZ(leftEnterRootPosition .z + (Angle > 0 ? GroundHalfScale / 2f : 0f));
		RightEnter.MoveZ(rightEnterRootPosition.z + (Angle < 0 ? GroundHalfScale / 2f : 0f));

		var leftExitRootPosition  = (Vector3.forward - Vector3.right) * GroundHalfScale / 2f + LeftEnter .localUp() * LeftEnter .localScale.y / 2f;
		var rightExitRootPosition = (Vector3.forward + Vector3.right) * GroundHalfScale / 2f + RightEnter.localUp() * RightEnter.localScale.y / 2f;

		LeftExit .RescaleZ(GroundHalfScale);
		RightExit.RescaleZ(GroundHalfScale);

		LeftExit .localRotation = rotation;
		RightExit.localRotation = rotation;

		var delta = GroundHalfScale * (1f - 1f / Mathf.Sqrt(1f + Mathf.Pow(Mathf.Tan(angle), 2f))) / 2f;
		LeftExit .localPosition = leftExitRootPosition  - LeftExit .localRight() * delta;
		RightExit.localPosition = rightExitRootPosition + RightExit.localRight() * delta;

		var xdelta = 0f;
		if (Angle > 0)		xdelta = RightEnter.localPosition.x - (RightExit.localPosition - RightExit.localForward() * RightExit.localScale.z / 2f).x;
		else if (Angle < 0) xdelta = LeftEnter .localPosition.x - (LeftExit .localPosition - LeftExit .localForward() * LeftExit .localScale.z / 2f).x;
		LeftExit .MoveX(LeftExit .localPosition.x + xdelta);
		RightExit.MoveX(RightExit.localPosition.x + xdelta);

		var zdelta = 0f;
		if (Angle > 0)		zdelta = (RightExit.localPosition - RightExit.localForward() * RightExit.localScale.z / 2f).z - (RightEnter.localPosition + RightEnter.localForward() * RightEnter.localScale.z / 2f).z;
		else if (Angle < 0) zdelta = (LeftExit .localPosition - LeftExit .localForward() * LeftExit .localScale.z / 2f).z - (LeftEnter .localPosition + LeftEnter .localForward() * LeftEnter .localScale.z / 2f).z;
		LeftExit .MoveZ(LeftExit .localPosition.z - zdelta);
		RightExit.MoveZ(RightExit.localPosition.z - zdelta);

		if (Angle > 0)
		{
			var endPoint = RightExit.localPosition + RightExit.localForward() * RightExit.localScale.z / 2f - RightExit.localRight() * GroundHalfScale;
			var scale = Vector3.Distance(endPoint, LeftExit.localPosition);
			LeftExit.RescaleZ(scale * 2f);
		}
		else if (Angle < 0)
		{
			var endPoint = LeftExit .localPosition + LeftExit .localForward() * LeftExit .localScale.z / 2f + LeftExit .localRight() * GroundHalfScale;
			var scale = Vector3.Distance(endPoint, RightExit.localPosition);
			RightExit.RescaleZ(scale * 2f);
		}
		else
		{
			LeftExit.RescaleZ(GroundHalfScale);
			RightExit.RescaleZ(GroundHalfScale);
		}
	}

	public void OnConstruct()
	{
		firstEntered = false;
		exited = 0;
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
			if (!firstEntered)
			{
				firstEntered = true;
				Dependency.Controller.GenerateTerrain();
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Agent"))
		{
			++exited;
			Dependency.Controller.DestructTerrain();
		}
	}

	public bool WaitingOnAgents => exited == 0 || Dependency.Controller.AgentsLeft > exited;
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
		ObjectActivator.Destruct(this);
	}
}
