using System;
using UnityEngine;

public class Wall : MonoBehaviour, IPoolableObject
{
	public event Action<Wall> AgentDied;

	public void OnConstruct()
	{
		gameObject.SetActive(true);
	}
	public void OnDestruct()
	{
		transform.SetParent(null);
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Agent")) AgentDied?.Invoke(this);
	}
}
