
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;

using SimpleJson;

public class AssetManager : GameSingleton<AssetManager>
{
	private Dictionary<string, UnityEngine.Object> m_assetDict = null;

	private bool m_bInit;

	public bool Init
	{
		get { return m_bInit; }
	}

	void Awake()
	{
		m_assetDict = new Dictionary<string, UnityEngine.Object>();
		m_bInit = false;
	}

	public void ClearAsset()
	{
		if (m_assetDict.Count <= 0)
		{
			return;
		}

		m_assetDict.Clear();
	}

	public T LoadAsset<T>(string path) where T : UnityEngine.Object
	{
		T obj = null;
		UnityEngine.Object assetObj = null;
		if (m_assetDict.TryGetValue (path, out assetObj) && assetObj != null)
		{
			obj = assetObj as T;
		} else
		{
			string[] name = path.Split("."[0]);
			assetObj = Resources.Load<T>(name[0]);
			m_assetDict[path] = assetObj;
			obj = assetObj as T;
		}

		return obj;
	}
}
