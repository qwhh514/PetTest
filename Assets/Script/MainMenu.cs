
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityStandardAssets.ImageEffects;

public class MainMenu : MonoBehaviour
{
	private Camera m_mainCamera;
	private GameObject m_mainUICamera;

	private GameObject m_logoObj = null;
	private GameObject m_hintObj = null;
	private GameObject m_levelObj = null;

	private GameObject m_upgradeBuild = null;
	private GameObject m_shop = null;

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

		m_mainCamera = Camera.main;
		m_mainUICamera = GameObject.Find("UICamera_Main");

		m_levelObj = GameObject.Find("SelectLevel");

		GameObject btn_start = m_levelObj.transform.FindChild("btn_ok").gameObject;
		UIEventListener.Get(btn_start).onClick += StartLevel;

		GameObject btn_cancel = m_levelObj.transform.FindChild("btn_cancel").gameObject;
		UIEventListener.Get(btn_cancel).onClick += CancelLevel;
		m_levelObj.SetActive(false);

		GameObject item = GameObject.Find ("BattleHall");
		m_menuItem.Add (item);

		m_upgradeBuild = GameObject.Find("UpgradeBuild");
		m_upgradeBuild.SetActive(false);

		for (int i = 0; i < 3; i++)
		{
			string name = "Build_" + i.ToString();
			Transform build = m_upgradeBuild.transform.FindChild(name);
			GameObject button = build.FindChild("Btn_Upgrade").gameObject;
			UIEventListener.Get(button).onClick += UpgradeBuild;
		}

		m_shop = GameObject.Find("Shop");
		btn_cancel = m_shop.transform.FindChild("btn_cancel").gameObject;
		UIEventListener.Get(btn_cancel).onClick += CloseShop;
		m_shop.SetActive(false);

		for (int i = 0; i < 3; i++)
		{
			string name = "Frame_Pet" + i.ToString();
			Transform pet = m_shop.transform.FindChild(name);
//			GameObject button = pet.FindChild("Btn_Buy").gameObject;
//			UIEventListener.Get(button).onClick += BuyPet;
		}

		CameraManager.Singleton.RunMainCamera();
		CameraManager.Singleton.LookTarget(item.transform, new Vector3(0,0,0));
		Vector3 distance = CameraManager.Singleton.GetDistanceFromCamera(item.transform.position);
		CameraManager.Singleton.m_vec3Distancetarget = distance;

		CameraManager.Singleton.RotateCameraToY(-10.0f, 100.0f);
		CameraManager.Singleton.ZoomIn(42.0f, 100.0f, true);

		BlurCamera(false);
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
					TouchObject touch = obj.AddMissingComponent<TouchObject>();
					touch.m_action += BuildingClick;
				}
			}
		}
	}

	private void BuildingClick()
	{
		if (m_levelObj != null)
		{
			m_levelObj.SetActive(true);
		}
	}

	private void StartLevel(GameObject go)
	{
		Application.LoadLevel("GameLevel");
	}

	private void CancelLevel(GameObject go)
	{
		if (m_levelObj != null)
		{
			m_levelObj.SetActive(false);
		}
	}

	private void OpenBuild(GameObject go)
	{
		if (m_upgradeBuild != null)
		{
			m_upgradeBuild.SetActive(true);
		}
	}
	
	private void CloseBuild(GameObject go)
	{
		if (m_upgradeBuild != null)
		{
			m_upgradeBuild.SetActive(false);
		}
	}

	private void UpgradeBuild(GameObject go)
	{
		if (m_upgradeBuild != null)
		{
			m_upgradeBuild.SetActive(false);
		}
	}

	private void OpenShop(GameObject go)
	{
		if (m_shop != null)
		{
			m_shop.SetActive(true);
		}
	}
	
	private void CloseShop(GameObject go)
	{
		if (m_shop != null)
		{
			m_shop.SetActive(false);
		}
	}

	private void BuyPet(GameObject go)
	{
	}

	private void BlurCamera(bool blur)
	{
		BlurOptimized blurScript = null;
		if (m_mainCamera != null)
		{
			blurScript = m_mainCamera.gameObject.GetComponent<BlurOptimized>();
			if (blurScript != null)
			{
				blurScript.enabled = blur;
			}
		}
		if (m_mainUICamera != null)
		{
			blurScript = m_mainUICamera.gameObject.GetComponent<BlurOptimized>();
			if (blurScript != null)
			{
				blurScript.enabled = blur;
			}
		}
	}

}
