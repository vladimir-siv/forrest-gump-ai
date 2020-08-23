using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[SerializeField] private Animator PlayerAnimator = null;

	private readonly InputCheck SKey = new InputCheck(KeyCode.S);
	
	void Update()
	{
		if (SKey)
		{
			PlayerAnimator.SetTrigger("Die");
		}
	}
}
