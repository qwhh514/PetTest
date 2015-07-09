
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StaticDefine;

using SimpleJson;

//TODO:有Loading后，在Loading里初始化
public class DataManager : GameSingleton<DataManager>  
{
	private Dictionary<string, JsonObject> m_data;

	void Awake() 
	{
	}

	public virtual void Initialize()
	{
		m_data = new Dictionary<string, JsonObject>();
		TextAsset textAsset = AssetManager.Singleton.LoadAsset<TextAsset>(StaticDefine.FilePath.JSON_ALLJSON_PATH);
		JsonArray allJson = SimpleJson.SimpleJson.DeserializeObject<SimpleJson.JsonArray>(textAsset.text);
		for (int index = 0; index < allJson.Count; index++)
		{
			JsonObject config = allJson[index] as JsonObject;
			string path = config["path"].ToString();
			TextAsset jsonText = AssetManager.Singleton.LoadAsset<TextAsset>(path);
			if (jsonText != null)
			{
				JsonObject json = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(jsonText.text);
				m_data[config["key"].ToString()] = json;
			}
		}
	}

	public JsonObject GetData(string key)
	{
		if (m_data.ContainsKey(key))
		{
			return m_data[key];
		}

		return null;
	}

	public bool HasData(string key)
	{
		return m_data.ContainsKey(key);
	}
}