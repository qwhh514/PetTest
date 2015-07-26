
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityStandardAssets.ImageEffects;

using StaticDefine;

public class MainMenu : MonoBehaviour
{
	private Camera m_mainCamera;
	private GameObject m_mainUICamera;

	private GameObject m_logoObj = null;
	private GameObject m_hintObj = null;
	private GameObject m_levelObj = null;

	private GameObject m_upgradeBuild = null;
	private GameObject m_shop = null;

	private Dictionary<string, GameObject> m_menuItem = new Dictionary<string, GameObject>();
	private Dictionary<string, GameObject> m_buildItem = new Dictionary<string, GameObject>();

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
		m_menuItem["BattleHall"] = item;

		item = GameObject.Find ("Shop");
		item.SetActive (false);
		m_menuItem["Shop"] = item;

		item = GameObject.Find ("Compose");
		m_menuItem["Compose"] = item;

		m_upgradeBuild = GameObject.Find("UpgradeBuild");
		m_upgradeBuild.SetActive(false);

		item = GameObject.Find("Build_Shop");
		if (item != null)
		{
			m_buildItem["Build_Shop"] = item;
		}

		item = GameObject.Find("Build_Compose");
		if (item != null)
		{
			m_buildItem["Build_Compose"] = item;
		}

		for (int i = 0; i < 3; i++)
		{
			string name = "Build_" + i.ToString();
			Transform build = m_upgradeBuild.transform.FindChild(name);
			GameObject button = build.FindChild("Btn_Upgrade").gameObject;
			UIEventListener.Get(button).onClick += UpgradeBuild;
		}

		m_shop = GameObject.Find("ShopFrame");
		Transform button1 = m_shop.transform.FindChild ("btn_cancel");
		btn_cancel = m_shop.transform.FindChild("btn_cancel").gameObject;
		UIEventListener.Get(btn_cancel).onClick += CloseShop;
		m_shop.SetActive(false);

		for (int i = 0; i < 3; i++)
		{
			string name = "Frame_Pet" + i.ToString();
			GameObject petButton = m_shop.transform.FindChild(name).gameObject;
//			GameObject button = petButton.FindChild("Btn_Buy").gameObject;
			UIEventListener.Get(petButton).onClick += BuyPet;
		}

		item = GameObject.Find ("BattleHall");
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
			foreach (KeyValuePair<string, GameObject> item in m_menuItem)
			{
				GameObject obj = item.Value;
				if (obj != null)
				{
					TouchObject touch = obj.AddMissingComponent<TouchObject>();
					touch.m_action += BuildingClick;
				}
			}

			foreach (KeyValuePair<string, GameObject> item in m_buildItem)
			{
				GameObject obj = item.Value;
				if (obj != null)
				{
					TouchObject touch = obj.AddMissingComponent<TouchObject>();
					touch.m_action += OpenBuild;
				}
			}
		}
	}

	private void BuildingClick(GameObject go)
	{
		if (go.name == "BattleHall" && m_levelObj != null)
		{
			OpenLevel(go);
		}
		else if (go.name == "Shop" && m_shop != null)
		{
			OpenShop(go);
		}
//		else if (go.name == "Compose" && m_levelObj != null)
//		{
//		}
	}

	private void OpenLevel(GameObject go)
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
			BlurCamera(true);
			m_upgradeBuild.SetActive(true);
		}
	}
	
	private void CloseBuild(GameObject go)
	{
		if (m_upgradeBuild != null)
		{
			BlurCamera(false);
			m_upgradeBuild.SetActive(false);
		}
	}

	private void UpgradeBuild(GameObject go)
	{
		GameObject shopGO = AssetManager.Singleton.LoadAsset<GameObject> (FilePath.PREFAB_PATH + "shop.prefab");
		GameObject shopIns = Instantiate (shopGO);
		shopIns.layer = LayerMask.NameToLayer ("MenuModel");
		shopIns.transform.position = new Vector3 (0.0f, -20.0f, 0.0f);
		shopIns.transform.localScale = Vector3.zero;

		int layer = LayerMask.NameToLayer ("MenuModel");
		SetGameObjecLayer (shopIns, layer);

		LeanTween.scale (shopIns, 2.0f * Vector3.one, 1.0f).setEase(LeanTweenType.easeOutBounce).setDestroyOnComplete (true).setOnComplete(
			() => {
				BlurCamera(false);
				m_menuItem["Shop"].SetActive(true);
				m_buildItem["Build_Shop"].SetActive(false);
			}
		);
	
		if (m_upgradeBuild != null)
		{
			m_upgradeBuild.SetActive(false);
		}
	}

	private void OpenShop(GameObject go)
	{
		if (m_shop != null)
		{
			BlurCamera(true);
			m_shop.SetActive(true);
		}
	}
	
	private void CloseShop(GameObject go)
	{
		BlurCamera(false);
		if (m_shop != null)
		{
			m_shop.SetActive(false);
		}
	}

	private void BuyPet(GameObject go)
	{
		GameObject petGO = AssetManager.Singleton.LoadAsset<GameObject> (FilePath.PREFAB_PATH + "Dragon.prefab");
		GameObject petIns = Instantiate (petGO);
		petIns.transform.position = new Vector3 (0.0f, -40.0f, 0.0f);
		petIns.transform.localScale = Vector3.one;
		petIns.transform.Rotate (new Vector3(0.0f, -30.0f, 0.0f));
		petIns.GetComponent<Animation> ().Play("idle1");

		int layer = LayerMask.NameToLayer ("MenuModel");
		SetGameObjecLayer (petIns, layer);

		LeanTween.scale (petIns, 8.0f * Vector3.one, 3.0f).setEase(LeanTweenType.easeOutBounce).setDestroyOnComplete (true).setOnComplete(
			() => {
				BlurCamera(false);
			}
		);

		if (m_shop != null)
		{
			m_shop.SetActive(false);
		}
	}

	private void SetGameObjecLayer(GameObject go, int layer)
	{
		go.layer = layer;

		Transform goTran = go.transform;
		int count = goTran.childCount;
		for (int i = 0; i < count; i++)
		{
			GameObject child = goTran.GetChild(i).gameObject;
			child.layer = layer;
			SetGameObjecLayer(child, layer);
		}
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
