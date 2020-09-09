using UnityEngine;

public class Wall : MonoBehaviour, IPoolableObject
{
	private static int mask = -1;
	public static int Mask
	{
		get
		{
			if (mask < 0) mask = LayerMask.GetMask("Walls");
			return mask;
		}
	}

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
