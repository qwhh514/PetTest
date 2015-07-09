
using UnityEngine;
using System.Collections;

abstract public class Factory<T> : MonoBehaviour where T : Factory<T>
{	
	public static T Create
	{
		get
		{
			GameObject obj = new GameObject (typeof(T).FullName);
			obj.hideFlags = HideFlags.HideAndDontSave;
			T instance = obj.AddComponent(typeof(T)) as T;
			return instance;
		}
	}

}
