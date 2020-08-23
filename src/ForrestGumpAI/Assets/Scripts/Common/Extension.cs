using UnityEngine;

public static class Extension
{
	public static void RescaleX(this Transform transform, float value) => transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
	public static void RescaleY(this Transform transform, float value) => transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z);
	public static void RescaleZ(this Transform transform, float value) => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value);

	public static void MoveX(this Transform transform, float value) => transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z);
	public static void MoveY(this Transform transform, float value) => transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z);
	public static void MoveZ(this Transform transform, float value) => transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value);
}
