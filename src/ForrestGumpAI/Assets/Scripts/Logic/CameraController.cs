using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private CameraMode Mode = CameraMode.ThirdPerson;

	public Agent Following { get; set; } = null;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (Mode == CameraMode.ThirdPerson) Mode = CameraMode.BirdPerspective;
			else Mode = CameraMode.ThirdPerson;
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			// Get Random Agent, or some logic which one to view
			Following = Dependency.Controller.Agents[0];
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
				transform.rotation = Quaternion.Euler(90f, Following.transform.rotation.eulerAngles.y, 0f);
				break;
		}
	}
}

public enum CameraMode
{
	ThirdPerson = 0,
	BirdPerspective = 1
}
