using System;
using UnityEngine;

public class SpreadPathway : MonoBehaviour, IPathway, IPoolableObject
{
	[Flags] public enum Gate
	{
		None = 0,
		West = 1,
		North = 2,
		East = 4,
	}

	private Transform _leftEnter = null;
	public Transform LeftEnter
	{
		get
		{
			if (_leftEnter == null) _leftEnter = transform.GetChild(0);
			return _leftEnter;
		}
	}

	private Transform _rightEnter = null;
	public Transform RightEnter
	{
		get
		{
			if (_rightEnter == null) _rightEnter = transform.GetChild(1);
			return _rightEnter;
		}
	}

	private Transform _leftWestExit = null;
	public Transform LeftWestExit
	{
		get
		{
			if (_leftWestExit == null) _leftWestExit = transform.GetChild(2);
			return _leftWestExit;
		}
	}

	private Transform _rightWestExit = null;
	public Transform RightWestExit
	{
		get
		{
			if (_rightWestExit == null) _rightWestExit = transform.GetChild(3);
			return _rightWestExit;
		}
	}

	private Transform _leftNorthExit = null;
	public Transform LeftNorthExit
	{
		get
		{
			if (_leftNorthExit == null) _leftNorthExit = transform.GetChild(4);
			return _leftNorthExit;
		}
	}

	private Transform _rightNorthExit = null;
	public Transform RightNorthExit
	{
		get
		{
			if (_rightNorthExit == null) _rightNorthExit = transform.GetChild(5);
			return _rightNorthExit;
		}
	}

	private Transform _leftEastExit = null;
	public Transform LeftEastExit
	{
		get
		{
			if (_leftEastExit == null) _leftEastExit = transform.GetChild(6);
			return _leftEastExit;
		}
	}

	private Transform _rightEastExit = null;
	public Transform RightEastExit
	{
		get
		{
			if (_rightEastExit == null) _rightEastExit = transform.GetChild(7);
			return _rightEastExit;
		}
	}

	private Transform _back = null;
	public Transform Back
	{
		get
		{
			if (_back == null) _back = transform.GetChild(8);
			return _back;
		}
	}

	private Transform _west = null;
	public Transform West
	{
		get
		{
			if (_west == null) _west = transform.GetChild(9);
			return _west;
		}
	}

	private Transform _north = null;
	public Transform North
	{
		get
		{
			if (_north == null) _north = transform.GetChild(10);
			return _north;
		}
	}

	private Transform _east = null;
	public Transform East
	{
		get
		{
			if (_east == null) _east = transform.GetChild(11);
			return _east;
		}
	}

	private Transform _southWest = null;
	public Transform SouthWest
	{
		get
		{
			if (_southWest == null) _southWest = transform.GetChild(12);
			return _southWest;
		}
	}

	private Transform _northWest = null;
	public Transform NorthWest
	{
		get
		{
			if (_northWest == null) _northWest = transform.GetChild(13);
			return _northWest;
		}
	}

	private Transform _northEast = null;
	public Transform NorthEast
	{
		get
		{
			if (_northEast == null) _northEast = transform.GetChild(14);
			return _northEast;
		}
	}

	private Transform _southEast = null;
	public Transform SouthEast
	{
		get
		{
			if (_southEast == null) _southEast = transform.GetChild(15);
			return _southEast;
		}
	}

	private Transform _ground = null;
	public Transform Ground
	{
		get
		{
			if (_ground == null) _ground = transform.GetChild(16);
			return _ground;
		}
	}

	private BoxCollider _collider = null;
	public BoxCollider Collider
	{
		get
		{
			if (_collider == null) _collider = GetComponent<BoxCollider>();
			return _collider;
		}
	}

	private float scale = 10f;
	public float Scale
	{
		get
		{
			return scale;
		}
		set
		{
			if (value == scale) return;
			scale = value;
			if (scale < 10f) scale = 10f;

			var groundScale = scale / 10f;
			Ground.localScale = new Vector3(groundScale, groundScale, groundScale);
			Collider.size = new Vector3(scale + 2f, 3f, scale + 2f);

			AdjustWalls();
		}
	}

	private Gate opened = Gate.None;
	public Gate Opened
	{
		get
		{
			return opened;
		}
		set
		{
			if (value == opened) return;
			opened = value;
			AdjustGates();
		}
	}

	private int exited = 0;

	public void OnConstruct()
	{
		exited = 0;
		Opened = Gate.None;
		gameObject.SetActive(true);
	}

	public void OnDestruct()
	{
		gameObject.SetActive(false);
	}

	public void AdjustWalls()
	{
		var halfScale = Scale / 2f;
		var innerScale = Constants.SQRT2 * (halfScale - 4.5f);
		var innerPush = Constants.SQRT2 * (halfScale + 0.5f) / 2f;

		LeftEnter     .localPosition = LeftEnter     .localForward() * (halfScale - 1f) + LeftEnter     .localUp() * LeftEnter     .localScale.y / 2f - LeftEnter     .localRight() * 2.5f;
		RightEnter    .localPosition = RightEnter    .localForward() * (halfScale - 1f) + RightEnter    .localUp() * RightEnter    .localScale.y / 2f + RightEnter    .localRight() * 2.5f;
		LeftWestExit  .localPosition = LeftWestExit  .localForward() * (halfScale - 1f) + LeftWestExit  .localUp() * LeftWestExit  .localScale.y / 2f - LeftWestExit  .localRight() * 2.5f;
		RightWestExit .localPosition = RightWestExit .localForward() * (halfScale - 1f) + RightWestExit .localUp() * RightWestExit .localScale.y / 2f + RightWestExit .localRight() * 2.5f;
		LeftNorthExit .localPosition = LeftNorthExit .localForward() * (halfScale - 1f) + LeftNorthExit .localUp() * LeftNorthExit .localScale.y / 2f - LeftNorthExit .localRight() * 2.5f;
		RightNorthExit.localPosition = RightNorthExit.localForward() * (halfScale - 1f) + RightNorthExit.localUp() * RightNorthExit.localScale.y / 2f + RightNorthExit.localRight() * 2.5f;
		LeftEastExit  .localPosition = LeftEastExit  .localForward() * (halfScale - 1f) + LeftEastExit  .localUp() * LeftEastExit  .localScale.y / 2f - LeftEastExit  .localRight() * 2.5f;
		RightEastExit .localPosition = RightEastExit .localForward() * (halfScale - 1f) + RightEastExit .localUp() * RightEastExit .localScale.y / 2f + RightEastExit .localRight() * 2.5f;
		
		Back .localPosition = Back .localForward() * (halfScale + 0.025f) + Back .localUp() * Back .localScale.y / 2f;
		West .localPosition = West .localForward() * (halfScale + 0.025f) + West .localUp() * West .localScale.y / 2f;
		North.localPosition = North.localForward() * (halfScale + 0.025f) + North.localUp() * North.localScale.y / 2f;
		East .localPosition = East .localForward() * (halfScale + 0.025f) + East .localUp() * East .localScale.y / 2f;

		SouthEast.RescaleX(innerScale);
		NorthEast.RescaleX(innerScale);
		NorthWest.RescaleX(innerScale);
		SouthWest.RescaleX(innerScale);

		SouthEast.localPosition = SouthEast.localForward() * innerPush + SouthEast.localUp() * SouthEast.localScale.y / 2f;
		NorthEast.localPosition = NorthEast.localForward() * innerPush + NorthEast.localUp() * NorthEast.localScale.y / 2f;
		NorthWest.localPosition = NorthWest.localForward() * innerPush + NorthWest.localUp() * NorthWest.localScale.y / 2f;
		SouthWest.localPosition = SouthWest.localForward() * innerPush + SouthWest.localUp() * SouthWest.localScale.y / 2f;
	}
	public void AdjustGates()
	{
		West .gameObject.SetActive(!Opened.HasFlag(Gate.West));
		North.gameObject.SetActive(!Opened.HasFlag(Gate.North));
		East .gameObject.SetActive(!Opened.HasFlag(Gate.East));
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Agent"))
		{
			++exited;
			Dependency.Controller.DestructTerrain();
		}
	}

	public bool WaitingOnAgents => exited == 0 || Dependency.Controller.AgentsAlive > exited;
	public IPathway Next { get; private set; } = null;
	public Vector3 ExitPoint
	{
		get
		{
			var direction = Vector3.zero;

			switch (Opened)
			{
				case Gate.West: direction = -transform.right; break;
				case Gate.North: direction = transform.forward; break;
				case Gate.East: direction = transform.right; break;
			}

			return transform.position + Scale * direction / 2f;
		}
	}
	public void ConnectTo(Vector3 position, float rotation)
	{
		Back.gameObject.SetActive(false);
		transform.rotation = Quaternion.Euler(0f, rotation, 0f);
		transform.position = position + Scale * transform.forward / 2f;
	}
	public void ConnectOn(IPathway pathway)
	{
		var direction = Vector3.zero;
		var anglecorrection = 0f;

		switch (Opened)
		{
			case Gate.West:
				direction = -transform.right;
				anglecorrection = -90f;
				break;
			case Gate.North:
				direction = transform.forward;
				anglecorrection = 0f;
				break;
			case Gate.East:
				direction = transform.right;
				anglecorrection = +90f;
				break;
		}

		var position = transform.position + Scale * direction / 2f;
		var rotation = transform.rotation.eulerAngles.y + anglecorrection;
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
		ObjectActivator.Destruct(this);
	}
}
