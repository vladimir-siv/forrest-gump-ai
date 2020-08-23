using UnityEngine;

public class Wall : MonoBehaviour, IPoolableObject
{
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
}
