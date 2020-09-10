using System.Collections.Generic;
using UnityEngine;

public class RiggedPathway : Pathway
{
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

	private Transform _leftExit = null;
	public Transform LeftExit
	{
		get
		{
			if (_leftExit == null) _leftExit = transform.GetChild(2);
			return _leftExit;
		}
	}

	private Transform _rightExit = null;
	public Transform RightExit
	{
		get
		{
			if (_rightExit == null) _rightExit = transform.GetChild(3);
			return _rightExit;
		}
	}

	private Transform _back = null;
	public Transform Back
	{
		get
		{
			if (_back == null) _back = transform.GetChild(4);
			return _back;
		}
	}

	private Transform _ground = null;
	public Transform Ground
	{
		get
		{
			if (_ground == null) _ground = transform.GetChild(5);
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

	public float Depth
	{
		get
		{
			return Ground.localScale.z * 10f;
		}
		set
		{
			var val = value / 10f;
			if (val < .6f) val = .6f;

			//if (val == Ground.localScale.z) return;

			Ground.RescaleZ(val);
			Ground.MoveZ((val - 1f) * 5f);

			LeftExit .MoveZ((2f * LeftExit .localScale.z) + ((val - 1f) * 10f));
			RightExit.MoveZ((2f * RightExit.localScale.z) + ((val - 1f) * 10f));

			var size = Collider.size;
			size.z = (val * 10f) + 2f;
			Collider.size = size;

			var center = Collider.center;
			center.z = ((val * 10f) - 8f) / 2f;
			Collider.center = center;

			Adjust();
		}
	}

	private readonly LinkedList<Wall> walls = new LinkedList<Wall>();

	public void ClearWalls()
	{
		foreach (var wall in walls) ObjectActivator.Destruct(wall);
		walls.Clear();
	}

	public void Adjust()
	{
		ClearWalls();
		var count = (int)Depth - 3;
		if (count % 2 != 0) ++count;
		CreateWalls(new Vector3(-3f, 1.5f, -3.5f), -1, count);
		CreateWalls(new Vector3(+3f, 1.5f, -3.5f), +1, count);
	}

	private void CreateWalls(Vector3 initialPosition, int initialAngle, int count)
	{
		for (var i = 0; i < count; ++i)
		{
			var wall = ObjectActivator.Construct<Wall>();
			walls.AddLast(wall);
			wall.transform.SetParent(transform);
			initialPosition.z += 1f;
			wall.transform.localPosition = initialPosition;
			wall.transform.localRotation = Quaternion.Euler(0f, 45f * (i % 2 == 0 ? initialAngle : -initialAngle), 0f);
			wall.transform.localScale = new Vector3(0.025f, 3f, Mathf.Sqrt(2f));
		}
	}

	public override void OnDestruct()
	{
		ClearWalls();
		base.OnDestruct();
	}

	public override Vector3 ExitPoint => transform.position + (Depth - 5f) * transform.forward;
	public override float Difficulty => 6f + Mathf.Min(Depth / 10f, 3f);
	public override void ConnectTo(Vector3 position, float rotation)
	{
		Back.gameObject.SetActive(false);
		transform.rotation = Quaternion.Euler(0f, rotation, 0f);
		transform.position = position + 5f * transform.forward;
	}
	public override void ConnectOn(IPathway pathway)
	{
		var rotation = transform.rotation.eulerAngles.y;
		pathway.ConnectTo(ExitPoint, rotation);
		Next = pathway;
	}
	public override void Disconnect()
	{
		Back.gameObject.SetActive(true);
	}
}
