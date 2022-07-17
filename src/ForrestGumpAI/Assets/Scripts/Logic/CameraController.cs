using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private CameraMode Mode = CameraMode.ThirdPerson;

	public Runner Following { get; private set; }
	public int FollowingIndex { get; private set; }

	private void Update()
	{
		var xPressed = Input.GetKeyDown(KeyCode.X);

		if (Input.GetKeyDown(KeyCode.C))
		{
			if (Mode == CameraMode.ThirdPerson) Mode = CameraMode.BirdPerspective;
			else Mode = CameraMode.ThirdPerson;
		}

		if ((Dependency.Controller.AgentsAlive > 0) && (xPressed || Following == null || Following.IsDead))
		{
			var agents = Dependency.Controller.Runners;

			if (xPressed) FollowingIndex = (FollowingIndex + 1) % agents.Length;

			for (var i = 0; i < agents.Length; FollowingIndex = (FollowingIndex + 1) % agents.Length, ++i)
			{
				if (agents[FollowingIndex] == null || agents[FollowingIndex].IsDead) continue;
				Following = agents[FollowingIndex];
				break;
			}
		}
	}

	private void LateUpdate()
	{
		if (Following == null) return;

		switch (Mode)
		{
			case CameraMode.ThirdPerson:
				transform.position = Following.transform.position - 5f * Following.transform.forward + 2.5f * Following.transform.up;
				transform.rotation = Quaternion.Euler(15f, Following.transform.rotation.eulerAngles.y, 0f);
				break;
			case CameraMode.BirdPerspective:
				transform.position = Following.transform.position + 15f * Following.transform.up;
				transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Quaternion.Euler(90f, Following.transform.rotation.eulerAngles.y, 0f);
				break;
		}
	}
}

public enum CameraMode
{
	ThirdPerson = 0,
	BirdPerspective = 1
}
