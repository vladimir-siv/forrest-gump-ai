using UnityEngine;

public interface IPathway
{
	void ConnectTo(Vector3 position, float rotation);
	void ConnectOn(IPathway pathway);
	void Destruct();
}
