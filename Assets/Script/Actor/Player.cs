
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
	public ArrayList Pets
	{
		get { return m_pets; }
	}

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

	public void Reset()
	{
		if (m_pets == null || m_pets.Count <= 0)
		{
			return;
		}

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

		m_curPet = null;
		for (int i = 0; i < m_pets.Count; i++)
		{
			NormalActor pet = m_pets[i] as NormalActor;
			if (pet != null)
			{
				pet.transform.position = position;
				pet.transform.rotation = quat;
				pet.Reset();

				if (m_curPet == null && pet.HP > 0)
				{
					m_curPet = pet;
				}
			}
		}
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
			string icon = JsonDataParser.GetString(petInfo, "ICON");
			string[] skill = JsonDataParser.GetString(petInfo, "Skills_ID").Split("|"[0]);
			string res = JsonDataParser.GetString(petInfo, "Model_ID") + ".prefab";

			GameObject prefab = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PREFAB_PATH + res);
			GameObject ins = Instantiate(prefab, position, quat) as GameObject;
			NormalActor actor = ins.AddMissingComponent<NormalActor>();
			ins.SetActive(false);

			actor.Icon = icon;
			actor.onHurtCallBack += handle;
			actor.Setup(this, hp, skill);
			m_pets.Add(actor);
		}

		if (m_pets.Count > 0)
		{
			m_curPet = m_pets[0] as NormalActor;
			m_curPet.gameObject.SetActive(true);
			GoToPlayGround();
			//GameLevel.Singleton.RefreshBloodBar(m_curPet, EventArgs.Empty);
		}
	}

	public void GoToPlayGround (bool switchBout = false)
	{
		Vector3 tar = m_curPet.transform.position;
		if (m_eSide == E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT)
		{
			m_curPet.transform.position = m_curPet.transform.position + new Vector3 (9, 0, 0);
		}
		else if (m_eSide == E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT)
		{
			m_curPet.transform.position = m_curPet.transform.position + new Vector3 (-9, 0, 0);
		}

		m_curPet.gameObject.SetActive (true);
		m_curPet.MoveTo (tar, m_curPet.GetMoveTime(), false, switchBout);

		GameLevel.Singleton.RefreshPetIcon (this);
	}

	public void SwitchPet(bool force = false)
	{
		if (m_curPet != null)
		{
			m_curPet.DestroyAllEff();
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
			GoToPlayGround ();
		}

		m_opponent.CurPet.Target = m_curPet;
		//被干死了不跳过回合
		if (!force)
		{
			GameLevel.Singleton.SwitchBout ();
		}
		GameLevel.Singleton.RefreshBloodBar(m_curPet, EventArgs.Empty);
		GameLevel.Singleton.RefreshSkillIcon ();
	}

	public void SwitchPet(NormalActor pet)
	{
		if (pet == m_curPet)
		{
			return;
		}

		if (m_curPet != null)
		{
			m_curPet.DestroyAllEff();
			m_curPet.gameObject.SetActive(false);
		}
		
		m_curPet = pet;
		
		if (m_curPet != null)
		{
			m_curPet.gameObject.SetActive(true);
			m_curPet.Target = m_opponent.CurPet;
			GoToPlayGround (true);
		}

		m_opponent.m_curPet.Target = m_curPet;
		//StartCoroutine (m_curPet.SwitchBout (m_curPet.GetMoveTime()));
		//GameLevel.Singleton.SwitchBout ();
		GameLevel.Singleton.RefreshBloodBar(m_curPet, EventArgs.Empty);
		GameLevel.Singleton.RefreshSkillIcon ();
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

	public void CatchOpponent(GameObject cage)
	{
		if (cage == null || m_opponent == null || m_opponent.CurPet == null)
		{
			return;
		}

		NormalActor pet = m_opponent.CurPet;

		GameObject cageIns = Instantiate(cage);
		cageIns.transform.SetParent(pet.transform);
		cageIns.SetActive(true);
		cageIns.transform.position = pet.transform.position + 8 * Vector3.up;
		LeanTween.scale(cageIns, 0.5f * Vector3.one, 0.6f).setEase(LeanTweenType.easeInQuad).setOnComplete
		(
			() => {
				LeanTween.moveLocal(cageIns, 0.6f * Vector3.up, 0.3f).setEase(LeanTweenType.easeInQuad).setOnComplete
				(
					() => {
						CatchResult(cageIns);
					}
				);
			}
		);

	}

	private void CatchResult(GameObject cage)
	{
		NormalActor pet = m_opponent.CurPet;
		float percent = (float)pet.HP / (float)pet.MaxHP;

		int num = 0;
		if (percent >= 0.15f && percent < 0.3f)
		{
			num = UnityEngine.Random.Range(0, 2);
		}
		else if (percent < 0.15f)
		{
			num = UnityEngine.Random.Range(0, 5);
		}
		bool result = (num != 0);

		if (result)
		{
			pet.BeCatch(cage);
		}
		else
		{
			LeanTween.alpha(cage, 0.0f, 0.1f).setLoopType(LeanTweenType.easeOutBounce).setLoopCount(3)
				.setDestroyOnComplete(true).setOnComplete
				(
					() => {
						GameLevel.Singleton.SwitchBout();
					}
				);
		}
	}

}
