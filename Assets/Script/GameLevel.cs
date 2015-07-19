
using UnityEngine;
using System;
using System.Collections;
using StaticDefine;
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

class SortPet : IComparer 
{
	int IComparer.Compare( object a, object b ) 
	{
		return ((NormalActor)a).HP - ((NormalActor)b).HP;
	}
}

public class GameLevel : MonoBehaviour {

	private static GameLevel m_instance = null;
	
	private Hashtable ActorHash;
	private Player m_leftPlayer;
	private Player m_rightPlayer;

	private bool m_bShowSkill;

	private BattleResult m_battleResult;

	private E_PLAYER_SIDE m_preBout;
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

//	private UIButton[] m_skillButons;

	private GameObject m_readygo;
	private GameObject m_reslut;
	private GameObject m_changePet;
	private GameObject m_skillBtn;

	///public CameraController m_cameraController;
	private CameraShake m_cameraShake;

	private NormalActor m_replacePet;

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

	public GameObject m_rainEff = null;

	public float m_originIntensity;

	public float m_targetIntensity;

	private void Awake()
	{
		//Light light = GameObject.Find("Light")
		m_originIntensity = GameObject.Find("Light").GetComponent<Light> ().intensity;
		m_targetIntensity = m_originIntensity;
		m_bShowSkill = true;

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

		//m_cameraController = CameraController.Create;
		//m_cameraController.SetCamera(Camera.main.transform);

		m_cameraShake = CameraShake.Create;
		m_cameraShake.SetTargetObj(Camera.main.transform);


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

//		for (int i = 0; i < 3; i++)
//		{
//			string name = "Skill" + i.ToString();
//			m_skillButons[0] = GameObject.Find(name).GetComponent<UIButton>();
//		}

		m_battleResult = BattleResult.BATTLE_RESULT_NONE;
		m_preBout = E_PLAYER_SIDE.E_PLAYER_PLACE_NONE;
		m_curBout = E_PLAYER_SIDE.E_PLAYER_PLACE_NONE;

		m_mainCamera = Camera.main;

		GameObject eff = null;
		eff = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PARTICLE_PATH + "3D_Rain_02.prefab");
		m_rainEff = Instantiate(eff);	
		m_rainEff.transform.SetParent(m_mainCamera.transform);
		m_rainEff.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		//float yOffset = JsonDataParser.GetFloat (skillInfo, "HurtOffset");
		//m_hurtEff.transform.position = position + new Vector3(0.0f, yOffset, 0.0f);
		m_rainEff.SetActive(false);

		m_mainUICamera = GameObject.Find("UICamera_Main");

		m_replacePet = null;

		RefreshSkillIcon ();

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

		LeanTween.scale(m_readygo, new Vector3(0.8f, 0.8f, 0.8f), 1.0f).setEase(LeanTweenType.easeInQuad).setOnComplete(
			() => {
				LeanTween.alpha(m_readygo, 0.0f, 1.0f).setDestroyOnComplete(true).setOnComplete(StartLevel);
			}
		);
	}

	public void ShakeCamera (float shake)
	{
		m_cameraShake.Shake (shake);
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
		if (!m_leftPlayer.CurPet.LogicalActive ()) {
			return;
		}

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
		E_PLAYER_SIDE m_preBout = m_curBout;
		m_curBout = (m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_NONE || m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT) ? E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT : E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT;
		if (m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT && m_leftPlayer != null)
		{
//			StartCoroutine(AutoSkill(m_leftPlayer.gameObject, 3.0f));
		}
		else if (m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_RIGHT && m_rightPlayer != null)
		{
			StartCoroutine(AutoSkill(m_rightPlayer.gameObject, 0.0f));
		}

		ShowSkillBar (m_curBout == E_PLAYER_SIDE.E_PLAYER_PLACE_LEFT);

		m_leftPlayer.Skilled = false;
		m_rightPlayer.Skilled = false;
	}

	public void ShowSkillBar(bool bShow)
	{
		if (m_bShowSkill != bShow && m_skillBtn != null)
		{
			Transform transform = GameObject.Find("SkillBtn").transform;
			int count = transform.childCount;
			for (int i = 0; i < count; i++)
			{
				GameObject obj = transform.GetChild(i).gameObject;
				float offset = bShow? 0.5f : -0.5f;
//				offset = (m_preBout == E_PLAYER_SIDE.E_PLAYER_PLACE_NONE)? 0.0f : offset;
				Vector3 position = obj.transform.position;
				Vector3 target = position + new Vector3(0.0f, offset, 0.0f);
				
				TweenPosition tc = TweenPosition.Begin(obj, 0.1f, target, true);
				tc.method = UITweener.Method.Linear;
			}
		}

		m_bShowSkill = bShow;
	}

	private IEnumerator AutoSkill(GameObject player, float delay)
	{
		yield return new WaitForSeconds (delay);
		this.SendGameMessage<GameObject> (player, GameActorMessage.GAM_ATTACK, -1, null);
	}

	void Update()
	{
		GameObject.Find ("Light").GetComponent<Light>().intensity = Mathf.Lerp(GameObject.Find ("Light").GetComponent<Light>().intensity, m_targetIntensity, Time.deltaTime);
		if (m_fBulletTime > 0) {
			m_fBulletTime -= Time.fixedDeltaTime;
			Time.timeScale = m_fBulletTimeScale;
			if(m_fBulletTime <= 0)
			{
				Time.timeScale = 1.0f;
				CameraManager.Singleton.StopMainCamera();
				CameraManager.Singleton.RestoreCameraSettings ();
			}
		}
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
			OpenResult();
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
			ArrayList temp = new ArrayList();
			for(int i = 0; i < pets.Count; i++)
			{
				temp.Add(pets[i]);
			}

			NormalActor pet = player.CurPet;
			temp.Remove(pet);
			temp.Sort(new SortPet());
			int count = temp.Count;

			ArrayList allPets = new ArrayList();
			allPets.Add(pet);
			for (int i = 0; i < count; i++)
			{
				allPets.Add(temp[i]);
			}
			count = allPets.Count;

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

					pet = allPets[i] as NormalActor;
					bool isAlive = pet.HP > 0;
					string spriteName = "head_" + pet.Icon;
					UIButton button = icon.GetComponent<UIButton>();
					if (button != null)
					{
						button.normalSprite = spriteName;
						button.hoverSprite = spriteName;
						button.pressedSprite = spriteName;
						button.disabledSprite = spriteName;
						button.isEnabled = isAlive;
					}
				}
				else
				{
					icon.SetActive(false);
				}
			}
		}
	}

	private void RefreshChangePet()
	{
		ArrayList pets = m_leftPlayer.Pets;
		for (int i = 0; i < pets.Count; i++)
		{
			string contentName = "btn_change" + i.ToString();
			NormalActor pet = pets[i] as NormalActor;
			GameObject content = m_changePet.transform.FindChild(contentName).gameObject;
			
			float percent = (float)pet.HP / (float)pet.MaxHP;
			content.transform.FindChild("BloodContent").GetComponent<UIProgressBar>().value = percent;
			content.transform.FindChild("BloodContent").FindChild("Blood").GetComponent<UILabel>().text = pet.HP.ToString() + "/" + pet.MaxHP.ToString();
			
			string state = "";
			if (pet.HP <= 0)
			{
				state = "icon_no";
			}
			else if (pet == m_replacePet)
			{
				state = "icon_change";
			}

			GameObject petState = content.transform.FindChild("PetState").gameObject;
			if (state == "")
			{
				petState.SetActive(false);
			}
			else
			{
				petState.GetComponent<UISprite>().spriteName = state;
				petState.SetActive(true);
			}
			
			state = "";
			bool isAlive = true;
			if (pet.HP <= 0)
			{
				state = "_die";
				isAlive = false;
			}
			else if (pet == m_replacePet)
			{
				state = "_sel";
			}
			
			string spriteName = "icon_" + pet.Icon + state;
			UIButton button = content.GetComponent<UIButton>();
			if (button != null)
			{
				button.normalSprite = spriteName;
				button.hoverSprite = spriteName;
				button.pressedSprite = spriteName;
				button.disabledSprite = spriteName;
				
				if (isAlive)
				{
					UIEventListener.Get(button.gameObject).onClick += SelectPet;
				}
			}
		}
	}

	private void OpenChangePet(GameObject go)
	{
		BlurCamera (true);

		m_changePet.SetActive(true);
		m_replacePet = m_leftPlayer.CurPet;
		RefreshChangePet ();
	}

	private void SelectPet(GameObject go)
	{
		int index = 0;
		if (go.name == "btn_change0")
		{
			index = 0;
		}
		else if (go.name == "btn_change1")
		{
			index = 1;
		}
		else if (go.name == "btn_change2")
		{
			index = 2;
		}
		
		NormalActor replacePet = m_leftPlayer.Pets[index] as NormalActor;
		if (replacePet.HP > 0)
		{
			m_replacePet = replacePet;
			RefreshChangePet ();
		}
	}

	private void CloseChangePet(GameObject go)
	{
		if (go.name == "Btn_Change_OK" && m_replacePet != m_leftPlayer.CurPet)
		{
			m_leftPlayer.SwitchPet (m_replacePet);
		}

		m_changePet.SetActive(false);
		BlurCamera (false);
		m_replacePet = null;
	}

	float m_fBulletTime = 0f;
	float m_fBulletTimeScale = 0f;

	public void BulletTime(float timeScale, float time, GameObject focusObj = null)
	{
		m_fBulletTime = time;
		m_fBulletTimeScale = timeScale;
		if (!focusObj)
			return;
		//m_cameraController.Focus (time, focusObj);
		CameraManager.Singleton.RunMainCamera();
		CameraManager.Singleton.BackupCameraSettings ();
		Vector3 offset= new Vector3(0f, 2.0f, 0f);
		CameraManager.Singleton.LookTarget(focusObj.transform, offset);
		Vector3 distance = CameraManager.Singleton.GetDistanceFromCamera(focusObj.transform.position);
		CameraManager.Singleton.m_vec3Distancetarget = distance;
		
		CameraManager.Singleton.RotateCameraToY(-10.0f, 100.0f);
		CameraManager.Singleton.RotateSpeedX = 50.0f;
		CameraManager.Singleton.ZoomIn(30.0f, 100.0f, true);
	}

	private void OpenResult()
	{
		m_reslut.SetActive(true);
		BlurCamera(true);

		GameObject button = GameObject.Find ("BackToMenu");
		UIEventListener.Get (button).onClick += BackMainMenu;

		ArrayList pets = m_leftPlayer.Pets;
		for (int i = 0; i < pets.Count; i++)
		{
			string contentName = "reslut_pet" + i.ToString();
			NormalActor pet = pets[i] as NormalActor;
			GameObject content = m_reslut.transform.FindChild(contentName).gameObject;

			string state = "";
			if (pet.HP <= 0)
			{
				state = "_die";
			}
			
			string spriteName = "icon_" + pet.Icon + state;
			UISprite sprite = content.GetComponent<UISprite>();
			if (sprite != null)
			{
				sprite.spriteName = spriteName;
			}
		}
	}

	public void RefreshSkillIcon()
	{
		if (m_leftPlayer == null || m_leftPlayer.CurPet == null)
		{
			return;
		}

		string[] iconName = m_leftPlayer.CurPet.GetSkillIcons ();
		if (iconName == null)
		{
			return;
		}

		for (int i = 0; i < iconName.Length; i++)
		{
			string name = "Skill" + i.ToString();
			UIButton button = GameObject.Find(name).GetComponent<UIButton>();
			if (button == null)
			{
				continue;
			}

			button.normalSprite = iconName[i];
			button.hoverSprite = iconName[i];
			button.pressedSprite = iconName[i];
			button.disabledSprite = iconName[i];
		}
	}

	private void BackMainMenu(GameObject go)
	{
		Application.LoadLevel("MainScene");
	}

}
