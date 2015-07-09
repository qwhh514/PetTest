
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	private GameObject m_logoObj = null;
	private GameObject m_hintObj = null;

	private List<GameObject> m_menuItem = new List<GameObject>();

	private bool m_bInit = false;

	// Use this for initialization
	void Start ()
	{
		m_bInit = false;

		DataManager.Singleton.Initialize ();
		m_logoObj = GameObject.Find ("logo");
		m_hintObj = GameObject.Find ("hint");
		m_hintObj.SetActive (false);

		GameObject item = GameObject.Find ("BattleHall");
		m_menuItem.Add (item);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!m_bInit && Input.anyKey)
		{
			if (m_logoObj != null)
			{
				m_logoObj.SetActive(false);
			}
			if (m_hintObj != null)
			{
				m_hintObj.SetActive(false);
			}

			for (int i = 0; i < m_menuItem.Count; i++)
			{
				GameObject obj = m_menuItem[i];
				if (obj != null)
				{
					obj.AddMissingComponent<TouchObject>();
				}
			}

			m_bInit = true;
		}
	}

	public void FadeInFinish()
	{
		m_hintObj.SetActive (true);
	}
}
