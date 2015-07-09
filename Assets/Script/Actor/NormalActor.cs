
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

	protected NormalActor()
	{
		m_master = null;
		m_target = null;
		
		Skills = null;
		SkillJson = null;
	}

	void Awake ()
	{
//		animator = gameObject.GetComponent<Animator>();
		m_animation = gameObject.GetComponent<Animation> ();
		m_curAnim = "";

//		SkillNum = Skills.Length;
		Guid = Game.GetInstance().actorGuid++;
		string guidStr = Guid.ToString ();
	}

	// Use this for initialization
	void Start ()
	{
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

		float delay = m_animation ["die"].length;
		StartCoroutine (SwitchBout(delay));
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
		StartCoroutine (SwitchBout(0.0f));
	}

	void OnMouseDown()  
	{
//		state = 1;
//		animator.SetInteger("ActorState", state);
//		GameLevel.Singleton.SendGameMessage<GameObject>(transform.gameObject, GameActorMessage.GAM_ATTACK, currentSkillIdx, null);
//		currentSkillIdx++;
//		currentSkillIdx = currentSkillIdx % SkillNum;
	} 
	
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

		if (m_curAnim != "idle1" && m_curAnim != "die" && !m_animation.isPlaying)
		{
			m_curAnim = "idle1";
			m_animation.Play("idle1");
		}
	}

	public void ReleaseSkill(GameMessage msg)
	{
//		GameLevel.Singleton.SendGameMessage<GameObject>(transform.gameObject, GameActorMessage.GAM_ATTACK, -1, null);
		int skillIdx = msg.Value;
		currentSkillIdx = (skillIdx >= 0) ? skillIdx : currentSkillIdx;
		state = (int)ActorState.ACTORSTATE_ATTACK1 + currentSkillIdx;
//		animator.SetInteger("ActorState", state);

		string skillName = "attack" + (currentSkillIdx+1).ToString ();
		m_curAnim = skillName;
		m_animation.Play (skillName);

//		float percent = 1.0f - animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//		percent = Mathf.Clamp (percent, 0.0f, 1.0f);
//		float playedTimes = animator.GetCurrentAnimatorStateInfo(0).length * percent;
		float playedTimes = JsonDataParser.GetFloat(SkillJson [currentSkillIdx], "SkillsParticle_PlayTime");
		playedTimes = Mathf.Max (0, playedTimes);
		StartCoroutine(DelayToInvoke (playedTimes, SkillBegin));

		currentSkillIdx++;
		currentSkillIdx = currentSkillIdx % SkillNum;
	}

	IEnumerator DelayToInvoke(float delaySeconds, EventHandler action)
	{
		yield return new WaitForSeconds (delaySeconds);
		action (this, EventArgs.Empty);
	}

	IEnumerator SwitchBout(float delaySeconds)
	{
		yield return new WaitForSeconds (delaySeconds);
		GameLevel.Singleton.SwitchBout ();
	}

	void SkillBegin(object sender, EventArgs e)
	{
		int skillId = (currentSkillIdx + SkillNum - 1) % SkillNum;
		string key = JsonDataParser.GetString (SkillJson [skillId], "SkillsParticle_id");

		GameObject attackEff = null;
		if (SkillEffect.TryGetValue(key, out attackEff))
		{
			GameObject eff = Instantiate(attackEff);
			string boneName = JsonDataParser.GetString (SkillJson [skillId], "SkillsParticle_Position");
			Transform bone = transform.Find (boneName);
			if (bone != null)
			{
				eff.transform.parent = bone;
			}
			else
			{
				eff.transform.parent = transform;
			}
			eff.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			eff.SetActive(true);
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

		GameObject hurtEff = null;
		if (SkillEffect.TryGetValue(key, out hurtEff))
		{
			GameObject eff = Instantiate(hurtEff);
			string boneName = JsonDataParser.GetString (skillInfo, "HurtParticle_Position");
			Transform bone = transform.Find (boneName);
			if (bone != null)
			{
				eff.transform.parent = bone;
			}
			else
			{
				eff.transform.parent = transform;
			}
			eff.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			eff.SetActive(true);
		}
		int hurtHP = JsonDataParser.GetInt (skillInfo, "damage");
		m_HP -= hurtHP;
		m_HP = Math.Max (0, m_HP);
		if (onHurtCallBack != null)
		{
			onHurtCallBack(this, EventArgs.Empty);
		}
		
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
	}

	IEnumerator onDeadEnd(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (m_master != null)
		{
			m_master.SwitchPet();
		}
	}

}
