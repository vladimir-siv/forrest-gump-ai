using UnityEngine;

public static class Timing
{
	public static readonly WaitForSeconds RagdollTimeout = new WaitForSeconds(3f);
	public static readonly WaitForSeconds TimeRefreshTimeout = new WaitForSeconds(.25f);
	public static readonly WaitForFixedUpdate FixedUpdate = new WaitForFixedUpdate();
}
