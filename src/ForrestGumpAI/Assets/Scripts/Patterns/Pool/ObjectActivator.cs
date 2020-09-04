using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectActivator
{
	private static readonly Dictionary<Type, Pooler> poolers = new Dictionary<Type, Pooler>();
	public static void CreatePool<T>(GameObject prototype) where T : MonoBehaviour, IPoolableObject => poolers.Add(typeof(T), new Pooler(prototype));
	public static T Construct<T>() where T : MonoBehaviour, IPoolableObject => poolers[typeof(T)].Construct<T>();
	public static void Destruct<T>(T obj) where T : MonoBehaviour, IPoolableObject => poolers[obj.GetType()].Destruct(obj);
}
