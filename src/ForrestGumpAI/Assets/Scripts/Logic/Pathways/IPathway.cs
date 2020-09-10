using UnityEngine;

public interface IPathway
{
	IPathway Next { get; }
	float Difficulty { get; }
	Vector3 ExitPoint { get; }
	void OnEnter(Agent agent);
	void OnExit(Agent agent);
	void ConnectTo(Vector3 position, float rotation);
	void ConnectOn(IPathway pathway);
	void Disconnect();
	void Destruct();
}
