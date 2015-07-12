﻿
using UnityEngine;
using System;
using System.Collections;

using UnityStandardAssets.ImageEffects;

public enum GameActorMessage
{
	GAM_NONE = 0,
	GAM_SPAWN,
	GAM_IDLE,
	GAM_MOVE,
	GAM_ATTACK,
	GAM_HEAL,
	GAM_HURT,
	GAM_DIE,
	GAM_COUNT
}

public enum BattleResult
{
	BATTLE_RESULT_NONE = 0,
	BATTLE_RESULT_WIN,
	BATTLE_RESULT_LOSR,
}

public struct GameMessage
{
	public int Value;
	public GameObject obj;
}

public class GameLevel : MonoBehaviour {

	private static GameLevel m_instance = null;
	
	private Hashtable ActorHash;
	private Player m_leftPlayer;
	private Player m_rightPlayer;

	private BattleResult m_battleResult;

	private E_PLAYER_SIDE m_curBout;
	public E_PLAYER_SIDE Bout
	{
		get { return m_curBout; }
	}

	private Camera m_mainCamera;
	private GameObject m_mainUICamera;

	private GameObject m_leftHud;
	private GameObject m_rightHud;

	private UIProgressBar m_leftBloodBar;
	private UILabel m_leftBlood;

	private UIProgressBar m_rightBloodBar;
	private UILabel m_rightBlood;

	private GameObject m_readygo;
	private GameObject m_reslut;
	private GameObject m_changePet;
	private GameObject m_skillBtn;

	public static GameLevel Singleton
	{
		get
		{
			if (m_instance == null)
			{
				GameObject obj = new GameObject (typeof(GameLevel).FullName);
				obj.hideFlags = HideFlags.HideAndDontSave;
				UnityEngine.Object.DontDestroyOnLoad(obj);
				m_instance = obj.AddComponent(typeof(GameLevel)) as GameLevel;
			}
			
			return m_instance;
		}
	}

	protected GameLevel()
	{
		m_instance = this;
	}

	private void Awake()
	{
		m_leftHud = GameObject.Find("Left_Hud");
		m_rightHud = GameObject.Find("Right_Hud");

		GameObject obj = GameObject.Find ("BloodBG_Left");
		m_leftBloodBar = obj.GetComponent<UIProgressBar>();
		m_leftBlood = obj.GetComponentInChildren<UILabel>();
		
		obj = GameObject.Find ("BloodBG_Right");
		m_rightBloodBar = obj.GetComponent<UIProgressBar>();
		m_rightBlood = obj.GetComponentInChildren<UILabel>();
		
		m_readygo = GameObject.Find("ReadyGo");
		m_skillBtn = GameObject.Find ("SkillBtn");

		m_reslut = GameObject.Find("LevelResult");
		m_reslut.SetActive(false);

		m_changePet = GameObject.Find("ChangePetPlane");
		m_changePet.SetActive(false);
		
		m_leftPlayer = Player.Create;
		m_leftPlayer.Side = E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT;
		m_leftPlayer.SetPets (new string[]{"101001", "101002", "101003"}, new EventHandler(RefreshBloodBar));

		m_rightPlayer = Player.Create;
		m_rightPlayer.Side = E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT;
		m_rightPlayer.SetPets (new string[]{"101000"}, new EventHandler(RefreshBloodBar));

		m_leftPlayer.Opponent = m_rightPlayer;
		m_rightPlayer.Opponent = m_leftPlayer;

		m_battleResult = BattleResult.BATTLE_RESULT_NONE;
		m_curBout = E_PLAYER_SIDE.E_PLAYER_PLACE_NONE;

		m_mainCamera = Camera.main;
		m_mainUICamera = GameObject.Find("UICamera_Main");

//		GameObject skill = GameObject.Find ("Skill0");
//		UIButton button = skill.GetComponent<UIButton> ();
//		button.enabled = true;
//
//		skill = GameObject.Find ("Skill1");
//		button = skill.GetComponent<UIButton> ();
//		button.enabled = true;
//
//		skill = GameObject.Find ("Skill2");
//		button = skill.GetComponent<UIButton> ();
//		button.enabled = true;
	}

	private void Start()
	{
		UIButton btn = GameObject.Find("Skip").GetComponent<UIButton>();
		btn.isEnabled = false;

		btn = GameObject.Find("GiveUp").GetComponent<UIButton>();
		btn.isEnabled = false;

		btn = GameObject.Find("Bag").GetComponent<UIButton>();
		btn.isEnabled = false;

		btn = GameObject.Find("ChangePet").GetComponent<UIButton>();
		UIEventListener.Get(btn.gameObject).onClick += OpenChangePet;
		btn.isEnabled = true;

		GameObject btn_start = m_changePet.transform.FindChild("Btn_Change_OK").gameObject;
		UIEventListener.Get(btn_start).onClick += CloseChangePet;
		GameObject btn_cancel = m_changePet.transform.FindChild("Btn_Change_Cancel").gameObject;
		UIEventListener.Get(btn_cancel).onClick += CloseChangePet;

		BlurCamera(true);

		LeanTween.scale(m_readygo, new Vector3(1.0f, 1.0f, 1.0f), 1.0f).setEase(LeanTweenType.easeInQuad).setOnComplete(
			() => {
				LeanTween.alpha(m_readygo, 0.0f, 1.0f).setDestroyOnComplete(true).setOnComplete(StartLevel);
			}
		);
	}

	private void BlurCamera(bool blur)
	{
		m_mainCamera.gameObject.GetComponent<BlurOptimized>().enabled = blur;
		m_mainUICamera.GetComponent<BlurOptimized>().enabled = blur;
	}

	public void OnSpawnActor(uint key, GameObject obj)
	{
		ActorHash.Add (key, obj);
	}
	
	public void OnRemoveActor(uint key)
	{
		ActorHash.Remove (key);
	}
	
	public void SendGameMessage<T>(GameObject obj, GameActorMessage type, int value, GameObject hookObj)  
	{  
		GameMessage msg;
		msg.Value = value;
		msg.obj = hookObj;

		switch (type)
		{  
			case GameActorMessage.GAM_ATTACK:  
			{
				obj.SendMessage ("ReleaseSkill", msg);
			}
			break;

			case GameActorMessage.GAM_HURT:  
			{
				obj.SendMessage ("BeHurt", msg);
			}
			break;

			case GameActorMessage.GAM_HEAL:
			{
				obj.SendMessage ("BeHeal", msg);
			}
			break;

			default: break;
		}
	}

	public void PetSkill(GameObject button)
	{
		if (button.name == "Skill0")
		{
			this.SendGameMessage<GameObject> (m_leftPlayer.gameObject, GameActorMessage.GAM_ATTACK, 0, null);
		}
		else if (button.name == "Skill1")
		{
			this.SendGameMessage<GameObject> (m_leftPlayer.gameObject, GameActorMessage.GAM_ATTACK, 1, null);
		}
		else if (button.name == "Skill2")
		{
			this.SendGameMessage<GameObject> (m_leftPlayer.gameObject, GameActorMessage.GAM_ATTACK, 2, null);
		}
	}

	private void StartLevel()
	{
		BlurCamera(false);

		SwitchBout();
	}

	public void SwitchBout()
	{
		E_PLAYER_SIDE preBout = m_curBout;
		m_curBout = (m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_NONE || m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT) ? E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT : E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT;
		if (m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT && m_leftPlayer != null)
		{
//			StartCoroutine(AutoSkill(m_leftPlayer.gameObject, 3.0f));
		}
		else if (m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT && m_rightPlayer != null)
		{
			StartCoroutine(AutoSkill(m_rightPlayer.gameObject, 0.0f));
		}

		if (m_skillBtn != null)
		{
			Transform transform = GameObject.Find("SkillBtn").transform;
			int count = transform.childCount;
			for (int i = 0; i < count; i++)
			{
				GameObject obj = transform.GetChild(i).gameObject;
				float offset = (m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT)? 0.42f : -0.42f;
				offset = (preBout == E_PLAYER_SIDE.E_PLAYER_PLACE_NONE)? 0.0f : offset;
				Vector3 position = obj.transform.position;
				Vector3 target = position + new Vector3(0.0f, offset, 0.0f);

				TweenPosition tc = TweenPosition.Begin(obj, 0.3f, target, true);
				tc.method = UITweener.Method.Linear;
			}
		}

		m_leftPlayer.Skilled = false;
		m_rightPlayer.Skilled = false;
	}

	private IEnumerator AutoSkill(GameObject player, float delay)
	{
		yield return new WaitForSeconds (delay);
		this.SendGameMessage<GameObject> (player, GameActorMessage.GAM_ATTACK, -1, null);
	}

	void Update()
	{
		if (m_leftPlayer.CurPet == null)
		{
			m_battleResult = BattleResult.BATTLE_RESULT_LOSR;
		}
		else if (m_rightPlayer.CurPet == null)
		{
			m_battleResult = BattleResult.BATTLE_RESULT_WIN;
		}

		if (m_battleResult != BattleResult.BATTLE_RESULT_NONE)
		{
			m_reslut.SetActive(true);
			BlurCamera(true);
		}
	}

	public void RefreshBloodBar(object obj, EventArgs args)
	{
		int curHp = 0;
		int maxHp = 0;
		float value = 0.0f;

		NormalActor actor = null;
		if (m_leftPlayer != null && m_leftPlayer.CurPet != null)
		{
			actor = m_leftPlayer.CurPet;

			if (actor != null)
			{
				curHp = actor.HP;
				maxHp = actor.MaxHP;
				value = (float)(curHp) / (float)(maxHp);
			}
			m_leftBloodBar.value = value;
			m_leftBlood.text = curHp.ToString() + "/" + maxHp.ToString();
		}

		if (m_rightPlayer != null && m_rightPlayer.CurPet != null)
		{
			actor = m_rightPlayer.CurPet;

			if (actor != null)
			{
				curHp = actor.HP;
				maxHp = actor.MaxHP;
				value = (float)(curHp) / (float)(maxHp);
			}
			m_rightBloodBar.value = value;
			m_rightBlood.text = curHp.ToString() + "/" + maxHp.ToString();
		}
	}

	public void RefreshPetIcon(Player player)
	{
		if (player == null)
		{
			return;
		}

		E_PLAYER_SIDE side = player.Side;
		GameObject hudObj = (side == E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT)? m_leftHud : m_rightHud;
		if (hudObj != null)
		{
			ArrayList pets = player.Pets;
			int count = (pets == null)? 0 : pets.Count;

			for (int i = 0; i < 3; i++)
			{
				string iconName = "Icon" + i.ToString();
				GameObject icon = hudObj.transform.FindChild(iconName).gameObject;
				if (icon == null)
				{
					continue;
				}

				if (i < count)
				{
					icon.SetActive(true);

					NormalActor pet = pets[i] as NormalActor;
					string spriteName = "head_" + pet.Icon;
					UISprite sprite = icon.GetComponent<UISprite>();
					if (sprite != null)
					{
						sprite.spriteName = spriteName;
					}
				}
				else
				{
					icon.SetActive(false);
				}
			}
		}
	}

	private void OpenChangePet(GameObject go)
	{
		m_changePet.SetActive(true);
	}

	private void CloseChangePet(GameObject go)
	{
		m_changePet.SetActive(false);
	}

}
