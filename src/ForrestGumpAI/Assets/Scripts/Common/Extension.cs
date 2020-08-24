﻿using UnityEngine;

public static class Extension
{
	public static void RescaleX(this Transform transform, float value) => transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
	public static void RescaleY(this Transform transform, float value) => transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z);
	public static void RescaleZ(this Transform transform, float value) => transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value);

	public static void MoveX(this Transform transform, float value) => transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z);
	public static void MoveY(this Transform transform, float value) => transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z);
	public static void MoveZ(this Transform transform, float value) => transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value);

	public static Vector3 localUp(this Transform transform) => transform.parent.InverseTransformDirection(transform.up);
	public static Vector3 localRight(this Transform transform) => transform.parent.InverseTransformDirection(transform.right);
	public static Vector3 localForward(this Transform transform) => transform.parent.InverseTransformDirection(transform.forward);

	public static int ArgMax(this float[] vals)
	{
		var maxv = float.NegativeInfinity;
		var maxi = -1;

		for (var i = 0; i < vals.Length; ++i)
		{
			if (vals[i] >= maxv)
			{
				maxv = vals[i];
				maxi = i;
			}
		}

		return maxi;
	}
}
