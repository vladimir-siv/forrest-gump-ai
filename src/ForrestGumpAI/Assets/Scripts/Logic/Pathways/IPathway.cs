using UnityEngine;

public interface IPathway
{
	IPathway Next { get; }
	float Difficulty { get; }
	Vector3 ExitPoint { get; }
	void OnEnter(Runner agent);
	void OnExit(Runner agent);
	void ConnectTo(Vector3 position, float rotation);
	void ConnectOn(IPathway pathway);
	void Disconnect();
	void Destruct();
}
