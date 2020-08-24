using UnityEngine;

public interface IPathway
{
	int AgentsInside { get; }
	IPathway Next { get; }
	void ConnectTo(Vector3 position, float rotation);
	void ConnectOn(IPathway pathway);
	void Disconnect();
	void Destruct();
}
