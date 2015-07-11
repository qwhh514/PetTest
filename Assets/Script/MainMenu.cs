
using UnityEngine;

using System;
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

		CameraManager.Singleton.RunMainCamera();
		CameraManager.Singleton.LookTarget(item.transform);
		Vector3 distance = CameraManager.Singleton.GetDistanceFromCamera(item.transform.position);
		CameraManager.Singleton.m_vec3Distancetarget = distance;

		CameraManager.Singleton.RotateCameraToY(-10.0f, 100.0f);
		CameraManager.Singleton.ZoomIn(42.0f, 100.0f, true);
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

			CameraManager.Singleton.RotateSpeedX = 100.0f;
			m_bInit = true;
		}
	}

	public void FadeInFinish()
	{
		m_hintObj.SetActive (true);

		LookOverScene();
		CameraManager.Singleton.onRotXComplete += LookOverScene;
	}

	public void LookOverScene()
	{
		if (!m_bInit)
		{
			CameraManager.Singleton.ResetRotateSpeed();
			CameraManager.Singleton.RotateCameraToX(360.0f, 0.08f, true, false);
		}
		else
		{
			for (int i = 0; i < m_menuItem.Count; i++)
			{
				GameObject obj = m_menuItem[i];
				if (obj != null)
				{
					obj.AddMissingComponent<TouchObject>();
				}
			}
		}
	}
}
