
using UnityEngine;
using System;
using System.Collections;

public class TouchObject : MonoBehaviour
{
	private GameObject m_gameobject = null;
	private Transform m_transform = null;
	public Action<GameObject> m_action;

	void Awake()
	{
		m_action = null;
	}

	// Use this for initialization
	void Start ()
	{
		m_gameobject = gameObject;
		m_transform = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		if(Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit))
			{
//				m_action(m_gameobject);
				if (hit.transform.name == m_transform.name && m_action != null)
				{
					m_action(m_gameobject);
				}
			}
		}
#else
		if (Input.touchCount != 1 )
		{
			return;
		}
		
		if (Input.GetTouch(0).phase == TouchPhase.Began)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.name == m_transform.name && m_action != null)
				{
					m_action();
				}
			}
		}
#endif
	}
	
}
