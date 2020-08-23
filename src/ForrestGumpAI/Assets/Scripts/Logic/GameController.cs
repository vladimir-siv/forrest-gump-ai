using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[SerializeField] private Agent Player = null;

	private readonly InputCheck WKey = new InputCheck(KeyCode.W);
	private readonly InputCheck DKey = new InputCheck(KeyCode.D);
	private readonly InputCheck RKey = new InputCheck(KeyCode.R);

	private void Start()
	{
		Player.OnConstruct();
	}

	private void Update()
	{
		if (WKey)
		{
			Player.Run();
		}

		if (DKey)
		{
			Player.Die();
		}

		if (RKey)
		{
			Player.OnDestruct();
			Player.OnConstruct();
		}
	}
}
