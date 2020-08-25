using UnityEngine;

public interface IPathway
{
	bool WaitingOnAgents { get; }
	IPathway Next { get; }
	void ConnectTo(Vector3 position, float rotation);
	void ConnectOn(IPathway pathway);
	void Disconnect();
	void Destruct();
}
