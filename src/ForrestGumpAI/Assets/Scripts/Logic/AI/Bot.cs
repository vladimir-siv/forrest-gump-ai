using System;

public class Bot
{
	private static Random r = new Random();

	public Agent Agent { get; set; }

	public void Think()
	{
		var dir = r.Next(0, 2);
		if (dir == 0) dir = -1;
		Agent.Steer(dir);
	}
}
