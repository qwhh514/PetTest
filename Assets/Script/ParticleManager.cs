#define USE_POOL

using UnityEngine;
using UnityEngine.Internal;
using System.Collections;
using System.Collections.Generic;
using StaticDefine;
using SimpleJson;

#if USE_POOL
using PathologicalGames;
#endif

public class ParticleManager : GameSingleton<ParticleManager>
{
#if USE_POOL
	private SpawnPool m_particlePool;
#endif
	private JsonObject m_particleInfo;
	private Dictionary<string, GameObject> m_particleGO;

	void Awake()
	{
#if USE_POOL
		//m_particlePool = PoolManager.Pools.Create("Particle", this.gameObject);
		//m_particlePool.dontReparent = true;
		//m_particlePool.dontDestroyOnLoad = true;
#endif
		m_particleInfo = DataManager.Singleton.GetData(FilePath.JSON_PARTICLE_NAME);
		m_particleGO = new Dictionary<string, GameObject>();
	}

	void Start() 
	{
	}

	void Update()
	{
	}
	
	public void Clear()
	{
		m_particleGO.Clear();
	}

	public void Preload(string particleId)
	{
		if (m_particleGO.ContainsKey(particleId))
		{
			return;
		}

		JsonObject config = JsonDataParser.GetJsonObject(m_particleInfo, particleId);
		string file = JsonDataParser.GetString(config, "effect_file");
		string path = FilePath.PARTICLE_PATH + file.ToString() + ".prefab";
		GameObject prefab = AssetManager.Singleton.LoadAsset<GameObject>(path);

		if (prefab != null)
		{
			if (m_particlePool == null)
			{
				m_particlePool = PoolManager.Pools.Create("Particle");
			}

			if (m_particlePool.GetPrefabPool(prefab) == null)
			{
				CreatePool(prefab, JsonDataParser.GetInt(config, "preload_amount"));
			}

			m_particleGO.Add(particleId, prefab);
		}
		else
		{
			Debug.LogError("Particle " + particleId + " cannot be preloaded");
		}
	}

	private void CreatePool(GameObject prefab, int preloadAmount)
	{
#if USE_POOL
		PrefabPool prefabPool = new PrefabPool(prefab.transform);
		prefabPool.preloadAmount = preloadAmount;
		prefabPool.limitInstances = true;
		prefabPool.limitFIFO = true;
		prefabPool.limitAmount = 20;
		prefabPool.cullDespawned = false;
		prefabPool.cullAbove = 6;
		prefabPool.cullDelay = 5;
		prefabPool.cullMaxPerPass = 1;
		m_particlePool.CreatePrefabPool(prefabPool);
#endif
	}

	private GameParticle CreateParticle(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
	{
#if USE_POOL
		if (m_particlePool == null || prefab == null)
		{
			return null;
		}

		Transform instance = m_particlePool.Spawn(prefab, position, rotation, parent);
		if (instance != null)
		{
			GameParticle script = instance.gameObject.GetComponent(typeof(GameParticle)) as GameParticle;;
			if (script == null)
			{
				script = instance.gameObject.AddComponent(typeof(GameParticle)) as GameParticle;
			}

			instance.position = position;
			instance.rotation = rotation;
			instance.gameObject.SetActive(true);
			return script;
		}

		return null;
#else
		if (prefab == null)
		{
			return null;
		}

		prefab.SetActive(false);
		GameObject instance = Instantiate(prefab, position, rotation) as GameObject;
		if (instance != null)
		{
			GameParticle script = instance.AddComponent(typeof(GameParticle)) as GameParticle;
			instance.transform.parent = parent;
			instance.SetActive(true);
			return script;
		}

		return null;
#endif
	}

	public GameParticle Play(string particleId, Vector3 position, Quaternion rotate, Transform parent = null)
	{
		if (!m_particleGO.ContainsKey(particleId))
		{
			Preload(particleId);
		}

		GameObject prefab = null;
		if (m_particleGO.TryGetValue(particleId, out prefab) && prefab != null)
		{
			GameParticle script = CreateParticle(prefab, position, rotate, parent);
			if (script != null)
			{
				JsonObject config = JsonDataParser.GetJsonObject(m_particleInfo, particleId);
				float desTime = JsonDataParser.GetFloat(config, "time_des");
				script.AutoDestory(desTime);
			}

			return script;
		}

		return null;
	}

	public void Despawn(Transform particle)
	{
        if (null == particle)
        {
            return;
        }
#if USE_POOL
		if (m_particlePool != null)
		{
			m_particlePool.Despawn(particle);
		}
#else
		Destroy(bullet.gameObject);
#endif
	}

	public bool ShouldEffectDamage(GGUnit unit)
	{
		return true;
	}
}
