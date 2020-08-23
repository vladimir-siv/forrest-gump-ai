using UnityEngine;

public class StraightPathway : MonoBehaviour, IPathway, IPoolableObject
{
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

	public void ConnectTo(Vector3 position, float rotation)
	{
		transform.rotation = Quaternion.Euler(0f, rotation, 0f);
		transform.position = position + Depth * transform.forward / 2f;
	}
	public void ConnectOn(IPathway pathway)
	{
		var position = transform.position + Depth * transform.forward / 2f;
		var rotation = transform.rotation.eulerAngles.y;
		pathway.ConnectTo(position, rotation);
	}
	public void Destruct() => ObjectActivator.Destruct(this);
}
