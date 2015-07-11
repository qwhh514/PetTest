using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StaticDefine;

public class UIDamageNum : MonoBehaviour {

    private GameObject m_damagePrefab;
	
    HUDText m_damageNum;
    Transform m_damageTarget;
    GameObject m_damange;
    private Queue<string> m_queueDamageNum;
	
	// Use this for initialization
	void Start () {

        m_queueDamageNum = new Queue<string>();
        this.m_damagePrefab = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.FONT_PATH + "red.prefab");
//         this.m_damagePrefab = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PREFAB_PATH + "damageNum.prefab");
        this.m_damageTarget = transform.FindChild("pivot");

        if (null == m_damageTarget)
        {
            Debug.LogWarning("The Object used a damageNum Script but with no pivot!!!!!");
			m_damageTarget = transform;
        }

        m_damange = NGUITools.AddChild(GameObject.FindWithTag("UI"), m_damagePrefab);
		UIFont font = m_damange.GetComponent<UIFont>();
        this.m_damageNum = m_damange.AddMissingComponent<HUDText>();
		m_damageNum.bitmapFont = font;
		UIFollowTarget myFollow = m_damange.AddMissingComponent<UIFollowTarget>();
        myFollow.target = m_damageTarget;
	}
	
	// Update is called once per frame
	void Update () {

        if (m_queueDamageNum.Count > 0)
        {
//            m_damageNum.Add(m_queueDamageNum.Dequeue(), Color.yellow, 0.0f, false);
			m_damageNum.Add(m_queueDamageNum.Dequeue(), Color.white, 0.0f);
        }
	}

    void OnDisable()
    {
        if (null == m_queueDamageNum)
        {
            return;
        }

        while (m_queueDamageNum.Count > 0)
        {
//            m_damageNum.Add(m_queueDamageNum.Dequeue(), Color.yellow, 0.0f, false);
            m_damageNum.Add(m_queueDamageNum.Dequeue(), Color.white, 0.0f);
        }
    }

	public void AddDamageNum(int nDamage)
    {
        if (null != this.m_damageNum)
        {
			string s = "-" + nDamage.ToString();
            m_queueDamageNum.Enqueue(s);
//             m_damageNum.Add(-nDamage, Color.yellow, 0.0f, false);
        }
    }

//    public static void Preload()
//    {
//        AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PREFAB_PATH + "damageNum.prefab", OnLoadPrefabEnd(FilePath.PREFAB_PATH + "damageNum.prefab"));
//    }

//    private static IEnumerator OnLoadPrefabEnd(string path)
//    {
//        m_damagePrefab = AssetManager.Singleton.GetAsset<GameObject>(path);
//        yield break;
//    }

//    public static void Unload()
//    {
//        m_damagePrefab = null;
//    }

//     public void resetDamageNum()
//     {
//         if (null != m_damageNum)
//         {
//             m_damageNum.ClearList();
//         }
//     }
}
