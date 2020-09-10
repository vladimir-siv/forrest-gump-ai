using UnityEngine;

public class StraightPathway : Pathway
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

	private Transform _ground = null;
	public Transform Ground
	{
		get
		{
			if (_ground == null) _ground = transform.GetChild(3);
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

			Left .RescaleZ(val * 10f);
			Right.RescaleZ(val * 10f);
			Left .MoveZ((val - 1f) * 5f);
			Right.MoveZ((val - 1f) * 5f);

			var size = Collider.size;
			size.z = (val * 10f) + 2f;
			Collider.size = size;

			var center = Collider.center;
			center.z = ((val * 10f) - 8f) / 2f;
			Collider.center = center;
		}
	}

	public override Vector3 ExitPoint => transform.position + (Depth - 5f) * transform.forward;
	public override float Difficulty => 4f + Mathf.Min(Depth / 10f, 3f);
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
