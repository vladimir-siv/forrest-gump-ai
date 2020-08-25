using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private CameraMode Mode = CameraMode.ThirdPerson;

	public Agent Following { get; set; }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (Mode == CameraMode.ThirdPerson) Mode = CameraMode.BirdPerspective;
			else Mode = CameraMode.ThirdPerson;
		}

		if (Input.GetKeyDown(KeyCode.X) || Following == null || Following.IsDead)
		{
			var agents = Dependency.Controller.Agents;

			for (var i = 0; i < agents.Length; ++i)
			{
				if (agents[i] == null || agents[i].IsDead) continue;
				Following = agents[i];
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
