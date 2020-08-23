using UnityEngine;

public class InputCheck
{
	public KeyCode Key { get; set; }
	public bool IsRequested { get; private set; }
	public InputCheck(KeyCode key) { Key = key; IsRequested = false; }

	public static implicit operator bool(InputCheck check)
	{
		if (check.IsRequested ^ Input.GetKeyDown(check.Key))
		{
			check.IsRequested = !check.IsRequested;
			return check.IsRequested;
		}

		return false;
	}
}
