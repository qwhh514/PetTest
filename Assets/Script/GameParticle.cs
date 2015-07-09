
#define USE_POOL

using UnityEngine;
using System.Collections;

#if USE_POOL
using PathologicalGames;
#endif

public class GameParticle: MonoBehaviour 
{
	private ParticleSystem[] m_particles;
	private float m_fLifetime = 0.0f;
	private float m_delta = 0.0f;	

	void Awake ()
	{
		m_particles = transform.GetComponentsInChildren<ParticleSystem>();
	}

	void Start () 
	{
	}

	void OnEnable()
	{
		m_delta = 0.0f;
	}

	public void AutoDestory(float time)
	{
		m_fLifetime = time;
	}

	void Update ()
	{
		m_delta += Time.deltaTime;
		if (m_fLifetime > 0.0f && m_delta > m_fLifetime)
		{
			ParticleManager.Singleton.Despawn(transform);
			return;
		}

		bool isAlive = false;
		for(int i = 0; i < m_particles.Length; i++)
		{
			if (m_particles[i].IsAlive(false))
			{
				isAlive = true;
				break;
			}
		}
		
		if (!isAlive)
		{
			ParticleManager.Singleton.Despawn(transform);
		}
	}
}
