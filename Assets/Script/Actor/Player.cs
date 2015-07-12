﻿
using UnityEngine;
using System;
using System.Collections;

using SimpleJson;
using StaticDefine;

public enum E_PLAYER_SIDE
{
	E_PLAYER_PLACE_NONE = 0,
	E_PLAYER_PLACE_LEFT,
	E_PLAYER_PLACE_RIGHT,
	E_PLAYER_PLACE_COUNT
}

public class Player : Factory<Player>
{

	private Player m_opponent;
	
	private E_PLAYER_SIDE m_eSide;
	private ArrayList m_pets;
	private NormalActor m_curPet;

	private bool m_bSkilled;
	public bool Skilled
	{
		get { return m_bSkilled; }
		set { m_bSkilled = value; }
	}

	public Player Opponent
	{
		get { return m_opponent; }

		set
		{
			m_opponent = value;
			if (m_opponent != null && m_curPet != null)
			{
				m_curPet.Target = m_opponent.m_curPet;
			}
		}
	}

	public E_PLAYER_SIDE Side
	{
		get { return m_eSide; }
		set { m_eSide = value; }
	}

	public NormalActor CurPet
	{
		get { return m_curPet; }
	}

	protected Player () {}

	void Awake ()
	{
		m_pets = new ArrayList();
		m_curPet = null;
		m_opponent = null;
		m_bSkilled = false;
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void SetPets(string[] petId, EventHandler handle)
	{
		if (petId == null || petId.Length <= 0)
		{
			return;
		}

//		Vector3 origin = GameObject.Find("Scene").transform.position;
		Vector3 origin = Vector3.zero;
		Vector3 position = Vector3.zero;
		Vector3 opposite = Vector3.zero;

		if (m_eSide == E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT)
		{
			position = origin + new Vector3(9, 0, -6);
			opposite = origin + new Vector3(-9, 0, -6);
		}
		else if (m_eSide == E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT)
		{
			position = origin + new Vector3(-9, 0, -6);
			opposite = origin + new Vector3(9, 0, -6);
		}

		Quaternion quat = Quaternion.identity;
		quat.SetLookRotation (opposite - position);
		

		JsonObject petInfo = null;
		JsonObject petJson = DataManager.Singleton.GetData ("pet.json");
		for (int i = 0; i < petId.Length; i++)
		{
			petInfo = JsonDataParser.GetJsonObject (petJson, petId[i].ToString());
			if (petInfo == null)
			{
				continue;
			}

			int hp = JsonDataParser.GetInt(petInfo, "HP");
			string[] skill = JsonDataParser.GetString(petInfo, "Skills_ID").Split("|"[0]);
			string res = JsonDataParser.GetString(petInfo, "Model_ID") + ".prefab";

			GameObject prefab = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PREFAB_PATH + res);
			GameObject ins = Instantiate(prefab, position, quat) as GameObject;
			NormalActor actor = ins.AddMissingComponent<NormalActor>();
			ins.SetActive(false);

			actor.onHurtCallBack += handle;
			actor.Setup(this, hp, skill);
			m_pets.Add(actor);
		}

		if (m_pets.Count > 0)
		{
			m_curPet = m_pets[0] as NormalActor;
			m_curPet.gameObject.SetActive(true);
			GameLevel.Singleton.RefreshBloodBar(m_curPet, EventArgs.Empty);
		}
	}

	public void SwitchPet()
	{
		if (m_curPet != null)
		{
			m_curPet.gameObject.SetActive(false);
		}

		m_curPet = null;
		for (int i = 0; i < m_pets.Count; i++)
		{
			NormalActor pet = m_pets[i] as NormalActor;
			if (pet.HP > 0)
			{
				m_curPet = pet;
				break;
			}
		}

		if (m_curPet != null)
		{
			m_curPet.gameObject.SetActive(true);
			m_curPet.Target = m_opponent.CurPet;
		}
		m_opponent.m_curPet.Target = m_curPet;
		GameLevel.Singleton.SwitchBout ();
	}

	public void ReleaseSkill(GameMessage msg)
	{
		E_PLAYER_SIDE curSide = GameLevel.Singleton.Bout;
		if (curSide != m_eSide || m_bSkilled)
		{
			return;
		}

		if (m_curPet != null)
		{
			m_bSkilled = true;
			m_curPet.ReleaseSkill(msg);
		}
	}

	public void BeHurt(GameMessage msg)
	{
		if (m_curPet != null)
		{
			m_curPet.BeHurt(msg);
		}
	}

	public void BeHeal(GameMessage msg)
	{
		if (m_curPet != null)
		{
			m_curPet.BeHeal(msg);
		}
	}

}
