
using UnityEngine;
using System.Collections;

abstract public class GameSingleton<T> : MonoBehaviour where T : GameSingleton<T>
{
	private static T m_instance = null;

	public static T Singleton
	{
		get
		{
			if (m_instance == null)
			{
				GameObject obj = new GameObject (typeof(T).FullName);
				obj.hideFlags = HideFlags.HideAndDontSave;
				Object.DontDestroyOnLoad(obj);
				m_instance = obj.AddComponent(typeof(T)) as T;
			}

			return m_instance;
		}
	}

	public virtual void Initialize() { }

	private void OnApplicationQuit()
	{
		if (m_instance != null)
		{
			Destroy(m_instance);
			Destroy(gameObject);
		}

		m_instance = null;
	}
	
}
