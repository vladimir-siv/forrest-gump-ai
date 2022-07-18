using UnityEngine;

public class Agent
{
	private Runner runner = null;

	public void AssignAgent(Runner runner)
	{
		this.runner = runner;
	}

	public void Think()
	{
		var decision = Random.Range(0, 3) - 1;
		if (decision != 0) runner.Steer(decision);
	}
}
