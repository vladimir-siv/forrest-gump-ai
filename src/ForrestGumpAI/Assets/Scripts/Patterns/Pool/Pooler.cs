using System.Collections.Generic;
using UnityEngine;

public sealed class Pooler
{
	private readonly Stack<IPoolableObject> pool = new Stack<IPoolableObject>();

	public GameObject Prototype { get; private set; }

	public Pooler(GameObject prototype)
	{
		Prototype = prototype;
	}

	public T Construct<T>() where T : MonoBehaviour, IPoolableObject
	{
		T obj;
		if (pool.Count == 0) obj = Object.Instantiate(Prototype).GetComponent<T>();
		else obj = (T)pool.Pop();
		obj.OnConstruct();
		return obj;
	}
	public void Destruct<T>(T obj) where T : MonoBehaviour, IPoolableObject
	{
		obj.OnDestruct();
		pool.Push(obj);
	}
}
