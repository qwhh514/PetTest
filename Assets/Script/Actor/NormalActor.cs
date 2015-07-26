
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using SimpleJson;
using StaticDefine;

public enum SkillType
{
	ST_NONE,
	ST_DAMAGE,
	ST_HEAL,
}

public enum SkillVisualType
{
	SVT_CASTER,
	SVT_DEFENDER,
	SVT_CASTER_GROUND,
	SVT_DEFENDER_GROUND,
	SVT_CASTER_TO_DEFENDER,
	SVT_DEFENDER_TO_CASTER,
}

public enum ActorState
{
	ACTORSTATE_IDLE = 0,
	ACTORSTATE_MOVE,
	ACTORSTATE_ATTACK1,
	ACTORSTATE_ATTACK2,
	ACTORSTATE_ATTACK3,
	ACTORSTATE_HURT,
	ACTORSTATE_DIE,
}

[Serializable]
public class ActorSkill
{
	public GameObject SkillCaster = null;
	public Vector3 OffsetCaster;
	public Vector3 RotationCaster;
	public Vector3 ScaleCaster;
	public GameObject SkillDefender = null;
	public Vector3 OffsetDefender;
	public Vector3 RotationDefender;
	public Vector3 ScaleDefender;
	public SkillType SkillType = SkillType.ST_NONE;
	public SkillVisualType SkillVisualType = SkillVisualType.SVT_CASTER;
	public Transform SkillBone = null;
	public Transform SkillBoneDefender = null;
}

//[RequireComponent(typeof(Animator))]
public class NormalActor : MonoBehaviour
{

//	private Animator animator;
	private Animation m_animation;
	private string m_curAnim;

	private int state = 0;
	private int m_HP = 0;
	private int m_MaxHP = 0;

	private string name;
	public string Name
	{
		get { return name; }
		set { name = value; }
	}

	private string icon;
	public string Icon
	{
		get { return icon; }
		set { icon = value; }
	}

	private GameObject obj = null;
	private uint Guid = 0;

	private Player m_master;
	private NormalActor m_target;

	private int SkillNum = 3;	
	public ActorSkill[] Skills;

	public static Dictionary<string, GameObject> SkillEffect = new Dictionary<string, GameObject>();
	public Transform SkillBoneHurt = null;

	public event EventHandler onHurtCallBack;

	private int currentSkillIdx;
	private int[] SkillKey;
	private JsonObject[] SkillJson;
	private Vector3 m_originPosition;
	private Quaternion m_originRotation;
	private Vector3 m_targetPosition;
	private Quaternion m_targetRotation;
	private float m_fMoveSpeed;
	private float m_fTurnSpeed;
	private float m_fMoveTime = 0.5f;
	private bool m_bMoving;
	private bool m_bResetRotationAfter;
	private float m_fTurnTimeScale = 0.25f;

	private List<GameObject> m_hurtEff = null;
	private GameObject m_attackEff = null;
	private GameObject m_cageGO = null;

	private bool m_bIsBeCatch = false;
	private bool m_bSwitchBout = false;

	private UIDamageNum m_damageNum;
	private UIHealNum m_healNum;

	public float GetMoveTime()
	{
		return m_fMoveTime;
	}
	public bool Moving()
	{
		return m_bMoving;
	}
	public bool LogicalActive()
	{
		return !Moving() && m_HP > 0;
	}
	public int CurrentSkillId
	{
		set { currentSkillIdx = value; }
	}

	public int HP
	{
		get { return m_HP; }
	}

	public int MaxHP
	{
		get { return m_MaxHP; }
	}

	public NormalActor Target
	{
		get { return m_target; }
		set { m_target = value; }
	}

	public bool BeCatched
	{
		get { return m_bIsBeCatch; }
	}

	protected NormalActor()
	{
		name = "";
		icon = "";

		m_master = null;
		m_target = null;
		
		Skills = null;
		SkillJson = null;

		m_damageNum = null;
		m_healNum = null;

		m_hurtEff = null;
	}

	void Awake ()
	{
//		animator = gameObject.GetComponent<Animator>();
		m_animation = gameObject.GetComponent<Animation> ();
		m_curAnim = "";

//		SkillNum = Skills.Length;
		Guid = Game.GetInstance().actorGuid++;
		string guidStr = Guid.ToString ();

		m_damageNum = gameObject.AddMissingComponent<UIDamageNum>();
		m_healNum = gameObject.AddMissingComponent<UIHealNum>();
		m_originPosition = transform.position;
		m_targetPosition = m_originPosition;
		m_originRotation = transform.rotation;
		m_targetRotation = m_originRotation;
		m_fMoveSpeed = 0;
		m_fTurnSpeed = 0;
		m_bResetRotationAfter = false;
		m_bMoving = false;

		m_bIsBeCatch = false;
		m_hurtEff = new List<GameObject>();
	}

	// Use this for initialization
	void Start ()
	{
	}

	public void Reset()
	{
		m_HP = m_MaxHP;
		m_bIsBeCatch = false;
		gameObject.SetActive (false);
	}

	public void DestroyAllEff ()
	{
		for (int i = 0; i < m_hurtEff.Count; i++)
		{
			Destroy (m_hurtEff[i]);
		}
		m_hurtEff.Clear();
		
		Destroy (m_attackEff);
		m_attackEff = null;
		
		Destroy (m_cageGO);
		m_cageGO = null;
	}

	public void Setup(Player master, int hp, string[] skillIds)
	{
		m_master = master;
		m_HP = hp;
		m_MaxHP = hp;

		if (skillIds.Length <= 0)
		{
			return;
		}

		SkillNum = skillIds.Length;
		SkillKey = new int[SkillNum];
		for (int i = 0; i < SkillNum; i++)
		{
			int.TryParse(skillIds[i], out SkillKey[i]);
		}
		SkillJson = new JsonObject[SkillNum];

		JsonObject skillInfo = null;
		JsonObject skillJson = DataManager.Singleton.GetData ("skill.json");
		JsonObject particleJson = DataManager.Singleton.GetData ("particle.json");
		for (int i = 0; i < SkillNum; i++)
		{
			skillInfo = JsonDataParser.GetJsonObject(skillJson, skillIds[i]);
			SkillJson[i] = skillInfo;

			GameObject obj = null;
			string attackEffId = JsonDataParser.GetString(skillInfo, "SkillsParticle_id");
			if (attackEffId != "" && attackEffId != "0" && !SkillEffect.TryGetValue(attackEffId, out obj))
			{
				JsonObject attackEffJson = JsonDataParser.GetJsonObject(particleJson, attackEffId);
				string attackEffRes = JsonDataParser.GetString(attackEffJson, "SkillsParticle_name");
				obj = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PARTICLE_PATH + attackEffRes);
				obj.SetActive(false);
				SkillEffect.Add(attackEffId, obj);
			}

			obj = null;
			string hurtEffId = JsonDataParser.GetString(skillInfo, "HurtParticle_id");
			if (hurtEffId != "" && hurtEffId != "0" && !SkillEffect.TryGetValue(hurtEffId, out obj))
			{
				JsonObject hurtEffJson = JsonDataParser.GetJsonObject(particleJson, hurtEffId);
				string hurtEffRes = JsonDataParser.GetString(hurtEffJson, "SkillsParticle_name");
				obj = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PARTICLE_PATH + hurtEffRes);
				obj.SetActive(false);
				SkillEffect.Add(hurtEffId, obj);
			}
		}
	}

	public string[] GetSkillIcons()
	{
		if (SkillJson == null || SkillJson.Length == 0)
		{
			return null;
		}

		string[] icons = new string[SkillJson.Length];
		for (int i = 0; i < SkillJson.Length; i++)
		{
			JsonObject obj = SkillJson[i];
			if (obj != null)
			{
				string iconName = JsonDataParser.GetString(obj, "ICON");
				icons[i] = iconName;
			}
		}

		return icons;
	}

	public void CastSkill(GameMessage msg)
	{
//		GameObject target = msg.Target as GameObject;
//		int idx = msg.Value;
//		if (Skills[idx].SkillCaster != null)
//		{
//			if(Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER ||
//			   Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER_GROUND ||
//			   Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER_TO_DEFENDER)
//			{
//				obj = Instantiate(Skills[idx].SkillCaster, Vector3.zero, Quaternion.identity) as GameObject;
//				obj.transform.Rotate(Skills[idx].RotationCaster);
//			}
//		}
//		if (Skills [idx].SkillDefender != null) 
//		{
//			if (Skills [idx].SkillVisualType == SkillVisualType.SVT_DEFENDER ||
//				Skills [idx].SkillVisualType == SkillVisualType.SVT_DEFENDER_GROUND ||
//				Skills [idx].SkillVisualType == SkillVisualType.SVT_DEFENDER_TO_CASTER) {
//				obj = Instantiate (Skills [idx].SkillDefender, Vector3.zero, Quaternion.identity) as GameObject;
//				obj.transform.Rotate (Skills [idx].RotationDefender);
//			}
//		}
//		if (obj != null)
//		{
//			if(Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER ||
//			   Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER_GROUND ||
//			   Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER_TO_DEFENDER)
//			{
//				obj.transform.SetParent(this.transform);
//			}
//			else if(Skills[idx].SkillVisualType == SkillVisualType.SVT_DEFENDER ||
//			        Skills[idx].SkillVisualType == SkillVisualType.SVT_DEFENDER_GROUND ||
//			        Skills[idx].SkillVisualType == SkillVisualType.SVT_DEFENDER_TO_CASTER)
//			{
//				obj.transform.SetParent(m_target.transform);
//			}
//			if(Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER)
//			{
//				obj.transform.position = Skills[idx].SkillBone.position + Skills[idx].OffsetCaster;
//			}
//			else if(Skills[idx].SkillVisualType == SkillVisualType.SVT_DEFENDER)
//			{
//				obj.transform.position = Skills[idx].SkillBoneDefender.position + Skills[idx].OffsetDefender;
//			}
//			else if(Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER_GROUND)
//			{
//				obj.transform.position = this.transform.position + Skills[idx].OffsetCaster;
//			}
//			else if(Skills[idx].SkillVisualType == SkillVisualType.SVT_DEFENDER_GROUND)
//			{
//				obj.transform.position = m_target.transform.position + Skills[idx].OffsetDefender;
//			}
//			else if(Skills[idx].SkillVisualType == SkillVisualType.SVT_CASTER_TO_DEFENDER)
//			{
//				obj.transform.position = this.transform.position + Skills[idx].OffsetCaster;
//				obj.SendMessage("SetTargetPosition", m_target.transform.position + Skills[idx].OffsetCaster);
//			}
//			else if(Skills[idx].SkillVisualType == SkillVisualType.SVT_DEFENDER_TO_CASTER)
//			{
//				obj.transform.position = m_target.transform.position + Skills[idx].OffsetDefender;
//				obj.SendMessage("SetTargetPosition", this.transform.position + Skills[idx].OffsetDefender);
//			}
//			obj.SetActive(true);
//		}
	}

	public void BeHurt(GameMessage msg)
	{
		if (m_HP <= 0 || m_curAnim == "die")
		{
			return;
		}

		state = (int)ActorState.ACTORSTATE_HURT;
//		animator.SetInteger("ActorState", state);

		m_curAnim = "hurt";
		m_animation.Play("hurt");

		string skillId = msg.Value.ToString();
		JsonObject skillJson = DataManager.Singleton.GetData ("skill.json");
		JsonObject skillInfo = JsonDataParser.GetJsonObject(skillJson, skillId);

//		float percent = 1.0f - animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//		percent = Mathf.Clamp (percent, 0.0f, 1.0f);
//		float playedTimes = animator.GetCurrentAnimatorStateInfo(0).length * percent;
		float playedTimes = JsonDataParser.GetFloat(skillInfo, "HurtParticle_PlayTime");
		playedTimes = Mathf.Max (0, playedTimes);
		StartCoroutine (onHurt(playedTimes, skillId));

		//float delay = m_animation ["die"].length;
		//StartCoroutine (SwitchBout(delay));
	}

	public void BeHeal(GameMessage msg)
	{
		int healHP = msg.Value;
		GameObject effect = msg.obj;
		
		if (effect != null)
		{
			obj = Instantiate(effect, Vector3.zero, Quaternion.identity) as GameObject;
		}
		
		if (obj != null)
		{
			obj.transform.SetParent(this.transform);
			obj.transform.position = SkillBoneHurt.position;
			obj.SetActive(true);
		}
		
		m_HP += healHP;
		m_HP = Math.Min (m_HP, m_MaxHP);
		if (onHurtCallBack != null)
		{
			onHurtCallBack(this, EventArgs.Empty);
		}

		m_healNum.AddHealNum(healHP);
		//StartCoroutine (SwitchBout(0.0f));
	}

	public void BeCatch(GameObject cage)
	{
		m_cageGO = cage;
		m_damageNum.AddDamageNum(m_HP);
		m_HP = 0;
		GameLevel.Singleton.RefreshBloodBar(this, EventArgs.Empty);
		StartCoroutine(DelayToInvoke (0.0f, onDead));
	}

//	void OnMouseDown()  
//	{
////		state = 1;
////		animator.SetInteger("ActorState", state);
////		GameLevel.Singleton.SendGameMessage<GameObject>(transform.gameObject, GameActorMessage.GAM_ATTACK, currentSkillIdx, null);
////		currentSkillIdx++;
////		currentSkillIdx = currentSkillIdx % SkillNum;
//	} 
	
	// Update is called once per frame
	void Update ()
	{
//		int lastState = state;
//		float playedTimes = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//		bool name = !animator.GetCurrentAnimatorStateInfo(0).IsName("idle1")
//			&& !animator.GetCurrentAnimatorStateInfo(0).IsName("die");
//		if(name && playedTimes > 1)
//		{
//			state = (int)ActorState.ACTORSTATE_IDLE;
//		}
//		if (lastState != state)
//		{
//			animator.SetInteger("ActorState", state);
//		}
		if (m_bMoving && m_HP > 0) {
			if((!m_animation.isPlaying || m_curAnim != "move") && m_curAnim != "die")
			{
				m_curAnim = "move";
				m_animation.Play("move");
			}

			var rollRotation = transform.rotation;
			if(transform.position != m_targetPosition)
			{
				rollRotation = Quaternion.LookRotation((m_targetPosition - transform.position), Vector3.up);
				transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, Time.deltaTime*m_fMoveSpeed);
			}

			if(transform.position == m_targetPosition && m_bResetRotationAfter){
				rollRotation = m_originRotation; 
			}

			transform.rotation = Quaternion.RotateTowards(transform.rotation, rollRotation, Time.deltaTime*m_fTurnSpeed);

			if(m_fMoveSpeed <= 0)
			{
				transform.position = m_targetPosition;
				transform.rotation = rollRotation;
			}

			if(transform.position == m_targetPosition && transform.rotation == rollRotation)
			{
				if(m_bSwitchBout)
				{
					m_bSwitchBout = false;
					StartCoroutine (SwitchBout (GetMoveTime()));
				}
				m_bMoving = false;
				m_bResetRotationAfter = false;
				m_curAnim = "idle1";
				m_animation.Play("idle1");
			}
		}
		else if (m_curAnim != "idle1" && m_curAnim != "die" && !m_animation.isPlaying)
		{
			m_curAnim = "idle1";
			m_animation.Play("idle1");
		}
	}

	void SkillBeginAfterMove (object sender, EventArgs e)
	{
		string skillName = "attack" + (currentSkillIdx+1).ToString ();
		m_curAnim = skillName;
		m_animation.Play (skillName);

		float playedTimes = JsonDataParser.GetFloat (SkillJson [currentSkillIdx], "SkillsParticle_PlayTime");
		playedTimes = Mathf.Max (0, playedTimes);
		StartCoroutine (DelayToInvoke (playedTimes, SkillBegin));
		currentSkillIdx++;
		currentSkillIdx = currentSkillIdx % SkillNum;

		float animTime = m_animation [skillName].length;
		StartCoroutine (DelayToInvoke (Mathf.Max (animTime, playedTimes), MoveBackToOrigin));
	}

	public void MoveTo (Vector3 targetPosition, float moveTime, bool resetOriginRotationAfter = false, bool switchBout = false)
	{
		m_targetPosition = targetPosition;
		if (moveTime <= 0)
		{
			if(transform.position != m_targetPosition)
			{
				transform.rotation = Quaternion.LookRotation((m_targetPosition - transform.position), Vector3.up);
			}
			if(resetOriginRotationAfter)
			{
				transform.rotation = m_originRotation;
			}
			transform.position = m_targetPosition;
			m_bMoving = false;
			return;
		}

		float distance = (m_targetPosition - transform.position).magnitude;
		m_fMoveSpeed = distance / moveTime;
		m_fTurnSpeed = 180.0f / (moveTime * m_fTurnTimeScale);
		m_bMoving = true;
		m_bResetRotationAfter = resetOriginRotationAfter;
		m_bSwitchBout = switchBout;
		//transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, Time.deltaTime*m_fMoveSpeed);
	}

	public void ReleaseSkill(GameMessage msg)
	{
//		GameLevel.Singleton.SendGameMessage<GameObject>(transform.gameObject, GameActorMessage.GAM_ATTACK, -1, null);
		int skillIdx = msg.Value; 
		currentSkillIdx = (skillIdx >= 0) ? skillIdx : currentSkillIdx;
		state = (int)ActorState.ACTORSTATE_ATTACK1 + currentSkillIdx;
//		animator.SetInteger("ActorState", state);

//		float percent = 1.0f - animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//		percent = Mathf.Clamp (percent, 0.0f, 1.0f);
//		float playedTimes = animator.GetCurrentAnimatorStateInfo(0).length * percent;
		int moveType = JsonDataParser.GetInt(SkillJson [currentSkillIdx], "Move");
		float moveTime = 0;
		if (moveType != 0) {
			moveTime = m_fMoveTime;
			Vector3 targetPosition = m_target.transform.position;
			Vector3 offset = targetPosition - transform.position;
			float distance = offset.magnitude;
			Vector3 targetDirection = offset.normalized;
			float bodySize = 1.0f + (GetBodySizeX(transform) + GetBodySizeX(m_target.transform)) * 0.5f;
			if(bodySize >= distance)
			{
				moveTime = 0;
			}
			else
			{
				targetPosition = transform.position + targetDirection * (distance - bodySize);
				MoveTo(targetPosition, moveTime);
			}
		}
		StartCoroutine (DelayToInvoke (moveTime, SkillBeginAfterMove));

		GameLevel.Singleton.ShowSkillBar (false);
	}

	public static float GetBodySizeX(Transform obj)
	{
		// get the maximum bounds extent of object, including all child renderers,
		// but excluding particles and trails, for FOV zooming effect.
		
		var renderers = obj.GetComponentsInChildren<Renderer>();
		
		Bounds bounds = new Bounds();
		bool initBounds = false;
		foreach (Renderer r in renderers)
		{
			if (!((r is TrailRenderer) || (r is ParticleRenderer) || (r is ParticleSystemRenderer)))
			{
				if (!initBounds)
				{
					initBounds = true;
					bounds = r.bounds;
				}
				else
				{
					bounds.Encapsulate(r.bounds);
				}
			}
		}
		return bounds.extents.x;
	}

	IEnumerator DelayToInvoke(float delaySeconds, EventHandler action)
	{
		yield return new WaitForSeconds (delaySeconds);
		action (this, EventArgs.Empty);
	}

	public IEnumerator SwitchBout(float delaySeconds)
	{
		yield return new WaitForSeconds (delaySeconds);
		GameLevel.Singleton.SwitchBout ();
	}

	void MoveBackToOrigin (object sender, EventArgs e)
	{
		if (transform.position != m_originPosition) {
			MoveTo (m_originPosition, m_fMoveTime, true);
		}
		float delay = 0.0f;
		if (transform.position != m_originPosition) {
			delay += m_fMoveTime + m_fTurnTimeScale * m_fMoveTime;
		} else if (transform.rotation != m_originRotation) {
			delay += m_fTurnTimeScale * m_fMoveTime;
		}
		//m_animation ["die"].length;
		StartCoroutine (SwitchBout (delay));
	}

	void SkillBegin(object sender, EventArgs e)
	{
		int skillId = (currentSkillIdx + SkillNum - 1) % SkillNum;
		string key = JsonDataParser.GetString (SkillJson [skillId], "SkillsParticle_id");

		GameObject attackEff = null;
		if (SkillEffect.TryGetValue(key, out attackEff))
		{
			m_attackEff = Instantiate(attackEff);
			Vector3 position = transform.position;
			string boneName = JsonDataParser.GetString (SkillJson [skillId], "SkillsParticle_Position");
			if(boneName != "")
			{
				Transform bone = transform.Find (boneName);
				if (bone != null)
				{
					m_attackEff.transform.SetParent(bone);
				}
				else
				{
					m_attackEff.transform.SetParent(transform);
				}
				position = m_attackEff.transform.parent.position;
			}
			m_attackEff.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			float yOffset = JsonDataParser.GetFloat (SkillJson [skillId], "Offset");
			m_attackEff.transform.position = position + new Vector3(0.0f, yOffset, 0.0f);
			m_attackEff.SetActive(true);
			m_attackEff.SendMessage("SetTargetPosition", m_target.transform.position);
			m_attackEff.SendMessage("SetOrigin", attackEff);
		}

		int damage = JsonDataParser.GetInt (SkillJson[skillId], "damage");
		int heal = JsonDataParser.GetInt (SkillJson [skillId], "heal");
		if (damage > 0)
		{
			GameLevel.Singleton.SendGameMessage<GameObject>(m_target.gameObject, GameActorMessage.GAM_HURT, SkillKey[skillId], null);
		}
		if (heal > 0)
		{

			GameLevel.Singleton.SendGameMessage<GameObject>(gameObject, GameActorMessage.GAM_HEAL, heal, null);
		}
	}

	IEnumerator onHurt(float delaySeconds, string skillId)
	{
		yield return new WaitForSeconds (delaySeconds);

		JsonObject skillJson = DataManager.Singleton.GetData ("skill.json");
		JsonObject skillInfo = JsonDataParser.GetJsonObject(skillJson, skillId);
		string key = JsonDataParser.GetString (skillInfo, "HurtParticle_id");
		float shake = JsonDataParser.GetFloat (skillInfo, "Shake");

		if (shake > 0f) {
			GameLevel.Singleton.ShakeCamera(shake);
		}

		GameObject hurtEff = null;
		if (SkillEffect.TryGetValue(key, out hurtEff))
		{
			GameObject hurtEffIns = Instantiate(hurtEff);
			Vector3 position = transform.position;
			string boneName = JsonDataParser.GetString (skillInfo, "HurtParticle_Position");
			if(boneName != "")
			{
				Transform bone = transform.Find (boneName);
				if (bone != null)
				{
					hurtEffIns.transform.SetParent(bone);
				}
				else
				{
					hurtEffIns.transform.SetParent(transform);
				}
				position = hurtEffIns.transform.parent.position;
			}

			hurtEffIns.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			float yOffset = JsonDataParser.GetFloat (skillInfo, "HurtOffset");
			hurtEffIns.transform.position = position + new Vector3(0.0f, yOffset, 0.0f);
			hurtEffIns.SetActive(true);
			m_hurtEff.Add(hurtEffIns);
		}

		int hurtHP = JsonDataParser.GetInt (skillInfo, "damage");
		m_HP -= hurtHP;
		m_HP = Math.Max (0, m_HP);
		if (onHurtCallBack != null)
		{
			onHurtCallBack(this, EventArgs.Empty);
		}

		m_damageNum.AddDamageNum(hurtHP);
		if (m_HP <= 0)
		{
//			float percent = 1.0f - animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//			percent = Mathf.Clamp (percent, 0.0f, 1.0f);
//			float playedTimes = animator.GetCurrentAnimatorStateInfo(0).length * percent;
			float percent = 1.0f - m_animation["hurt"].normalizedTime;
			percent = Mathf.Clamp (percent, 0.0f, 1.0f);
			float playedTimes = m_animation["hurt"].time * percent;
			StartCoroutine(DelayToInvoke (playedTimes, onDead));
		}
	}

	void onDead(object sender, EventArgs e)
	{
		state = (int)ActorState.ACTORSTATE_DIE;
//		animator.SetInteger("ActorState", state);

		m_curAnim = "die";
		m_animation.Play ("die");

		float delay = m_animation ["die"].length;
		StartCoroutine (onDeadEnd(delay));
		GameLevel.Singleton.BulletTime (0.2f, 5.0f, gameObject);
	}

	IEnumerator onDeadEnd(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (m_master != null)
		{
			m_master.SwitchPet(true);
			GameLevel.Singleton.m_rainEff.SetActive(true);
			GameLevel.Singleton.m_targetIntensity = 1.0f;
		}
	}

}
