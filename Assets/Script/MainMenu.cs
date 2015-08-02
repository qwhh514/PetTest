
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

	private GameObject m_maskBG = null;
	private GameObject m_screenTap = null;

	private Dictionary<string, GameObject> m_menuItem = new Dictionary<string, GameObject>();
	private Dictionary<string, GameObject> m_buildItem = new Dictionary<string, GameObject>();

	private GameObject Compound = null;
	private GameObject Sprite_Compound_Master = null;
	private GameObject[] Sprite_Compounds = null;
	
	private GameObject Btn_Compound = null;
	private GameObject Btn_Compound_Cancel = null;
	private GameObject[] Btn_Compounds = null;

	private GameObject m_particle = null;

	private GameObject m_shopIns = null;
	private GameObject m_compoundModel = null;
	private GameObject m_petIns = null;

	private bool m_bInit = false;
	private bool m_bInMain = true;

	private GameObject m_NPCModel = null;
	private GameObject m_NPCObj = null;
	private UILabel m_NPCLabel = null;

	// Use this for initialization
	void Start ()
	{
		m_bInit = false;
		m_bInMain = true;

		DataManager.Singleton.Initialize ();
		m_logoObj = GameObject.Find ("logo");
		m_hintObj = GameObject.Find ("hint");
		m_hintObj.SetActive (false);

		m_mainCamera = Camera.main;
		m_mainUICamera = GameObject.Find("UICamera_Main");

		m_maskBG = GameObject.Find ("MaskBG");
		m_maskBG.SetActive (false);

		m_screenTap = GameObject.Find ("ScreenTap");
		m_screenTap.SetActive(false);
		
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

		Compound = GameObject.Find("Compound");

		Sprite_Compound_Master = GameObject.Find("Sprite_Compound_Master");
		Sprite_Compound_Master.transform.localScale = Vector3.zero;
		Sprite_Compound_Master.SetActive(false);
		Sprite_Compounds = new GameObject[5];
		
		for (int i = 0; i < Sprite_Compounds.Length; i++)
		{
			string name = "Sprite_Compound" + i.ToString();
			Sprite_Compounds[i] = GameObject.Find(name);
			Sprite_Compounds[i].transform.localScale = Vector3.zero;
			Sprite_Compounds[i].SetActive(false);
		}
		
		Btn_Compound = GameObject.Find("Btn_Compound");
		Btn_Compound.SetActive(false);
		UIEventListener.Get(Btn_Compound).onClick += CompoundPet;

		Btn_Compound_Cancel = Compound.transform.FindChild ("Btn_cancel").gameObject;
		UIEventListener.Get(Btn_Compound_Cancel).onClick += CloseCompound;

		Btn_Compounds = new GameObject[7];
		for (int i = 0; i < Btn_Compounds.Length; i++)
		{
			string name = "Btn_Compound" + i.ToString();
			Btn_Compounds[i] = GameObject.Find(name);
			UIEventListener.Get(Btn_Compounds[i]).onClick += SelectCompound;
		}

		Compound.SetActive (false);

		m_NPCModel = GameObject.Find ("NPC_Lich");
		m_NPCModel.SetActive (false);

		m_NPCLabel = GameObject.Find ("NPC_Label").GetComponent<UILabel>();
		m_NPCObj = GameObject.Find ("NPC_Talk");
		m_NPCObj.SetActive (false);

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
		if (!m_bInMain)
		{
			return;
		}

		if (go.name == "BattleHall" && m_levelObj != null)
		{
			OpenLevel(go);
		}
		else if (go.name == "Shop" && m_shop != null)
		{
			OpenShop(go);
		}
		else if (go.name == "Compose" && m_levelObj != null)
		{
			OpenCompound(go);
		}
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
		if (!m_bInMain)
		{
			return;
		}

		if (m_upgradeBuild != null)
		{
			m_maskBG.SetActive(true);
			BlurCamera(true);
			m_upgradeBuild.SetActive(true);

			m_NPCModel.SetActive(true);
			Animation anim = m_NPCModel.GetComponent<Animation>();
			anim.Play("talk2");

			m_NPCObj.SetActive(true);
			m_NPCLabel.text = "在这里可以建筑你的要塞,从商店开始吧！";
		}
	}
	
	private void CloseBuild(GameObject go)
	{
		if (m_particle != null)
		{
			m_particle.SetActive(false);
		}
		Destroy(m_particle);

		if (m_shopIns != null)
		{
			m_shopIns.SetActive(false);
		}
		Destroy (m_shopIns);

		m_screenTap.SetActive(false);
		m_maskBG.SetActive(false);
		BlurCamera(false);

		if (m_upgradeBuild != null)
		{
			m_upgradeBuild.SetActive(false);
		}
	}

	private void UpgradeBuild(GameObject go)
	{
		GameObject obj = AssetManager.Singleton.LoadAsset<GameObject> (FilePath.PREFAB_PATH + "shop.prefab");
		m_shopIns = Instantiate (obj);
		m_shopIns.transform.position = new Vector3 (5.0f, -20.0f, 0.0f);
		m_shopIns.transform.localScale = Vector3.zero;

		obj = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PARTICLE_PATH + "2D_Star_01.prefab");
		m_particle = Instantiate(obj);
		m_particle.transform.position = m_mainUICamera.transform.position;

		int layer = LayerMask.NameToLayer ("MenuModel");
		SetGameObjecLayer (m_shopIns, layer);

		layer = LayerMask.NameToLayer ("UI");
		SetGameObjecLayer (m_particle, layer);

		LeanTween.scale (m_shopIns, 2.0f * Vector3.one, 1.0f).setEase(LeanTweenType.easeOutBounce).setOnComplete
		(
			() => {
//				m_maskBG.SetActive(false);
//				BlurCamera(false);
				m_menuItem["Shop"].SetActive(true);
				m_buildItem["Build_Shop"].SetActive(false);

				m_screenTap.SetActive(true);
				UIEventListener.Get(m_screenTap).onClick += CloseBuild;
			}
		);
	
		if (m_upgradeBuild != null)
		{
			m_NPCModel.SetActive(false);
			m_NPCObj.SetActive(false);
			m_maskBG.SetActive(false);
			m_upgradeBuild.SetActive(false);
		}
	}

	private void OpenShop(GameObject go)
	{
		if (m_shop != null)
		{
			m_NPCModel.SetActive(true);
			Animation anim = m_NPCModel.GetComponent<Animation>();
			anim.Play("talk2");
			
			m_NPCObj.SetActive(true);
			m_NPCLabel.text = "在这里可以够买你的宠物,从小龙开始吧！";

			m_maskBG.SetActive(true);
			BlurCamera(true);
			m_shop.SetActive(true);
		}
	}
	
	private void CloseShop(GameObject go)
	{
		if (m_particle != null)
		{
			m_particle.SetActive(false);
		}
		Destroy (m_particle);

		if (m_petIns != null)
		{
			m_petIns.SetActive(false);
		}
		Destroy (m_petIns);

		m_screenTap.SetActive(false);
		m_maskBG.SetActive(false);
		BlurCamera(false);
		if (m_shop != null)
		{
			m_shop.SetActive(false);
		}
	}

	private void BuyPet(GameObject go)
	{
		GameObject obj = AssetManager.Singleton.LoadAsset<GameObject> (FilePath.PREFAB_PATH + "Dragon.prefab");
		m_petIns = Instantiate (obj);
		m_petIns.transform.position = new Vector3 (0.0f, -60.0f, 0.0f);
		m_petIns.transform.localScale = Vector3.one;
		m_petIns.transform.Rotate (new Vector3(-15.0f, -50.0f, 15.0f));
		m_petIns.GetComponent<Animation> ().Play("idle1");

		obj = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PARTICLE_PATH + "2D_Star_01.prefab");
		m_particle = Instantiate(obj);
		m_particle.transform.position = m_mainUICamera.transform.position;

		int layer = LayerMask.NameToLayer ("MenuModel");
		SetGameObjecLayer (m_petIns, layer);

		layer = LayerMask.NameToLayer ("UI");
		SetGameObjecLayer (m_particle, layer);

		LeanTween.scale (m_petIns, 8.0f * Vector3.one, 3.0f).setEase(LeanTweenType.easeOutBounce).setOnComplete
		(
			() => {
//				m_maskBG.SetActive(false);
//				BlurCamera(false);
				m_screenTap.SetActive(true);
				UIEventListener.Get(m_screenTap).onClick += CloseShop;
			}
		);

		if (m_shop != null)
		{
			m_NPCModel.SetActive(false);
			m_NPCObj.SetActive(false);
			m_maskBG.SetActive(false);
			m_shop.SetActive(false);
		}
	}

	private void OpenCompound(GameObject go)
	{
		if (Compound != null)
		{
			m_maskBG.SetActive(true);
			BlurCamera(true);
			Compound.SetActive (true);
		}
	}
	
	public void CloseCompound(GameObject go)
	{
		if (m_particle != null)
		{
			m_particle.SetActive(false);
		}
		Destroy (m_particle);

		if (m_compoundModel != null)
		{
			m_compoundModel.SetActive(false);
		}
		Destroy (m_compoundModel);

		m_screenTap.SetActive(false);
		m_maskBG.SetActive(false);
		BlurCamera(false);

		if (Compound != null)
		{
			Compound.SetActive (false);
		}
	}
	
	private void SelectCompound(GameObject go)
	{
		string btnName = go.name;
		int compoundIdx = -1;
		
		switch(btnName)
		{
			case "Btn_Compound1": compoundIdx = 0; break;
			case "Btn_Compound2": compoundIdx = 1; break;
			case "Btn_Compound4": compoundIdx = 2; break;
			case "Btn_Compound5": compoundIdx = 3; break;
			case "Btn_Compound6": compoundIdx = 4; break;
			default: break;
		}
		
		GameObject compoundItem = (compoundIdx >= 0 && compoundIdx < Sprite_Compounds.Length)? Sprite_Compounds[compoundIdx] : null;
		if (compoundItem != null)
		{
			compoundItem.SetActive(true);
			compoundItem.transform.localScale = 1.2f * Vector3.one;
			
			LeanTween.scale(compoundItem, 0.9f * Vector3.one, 0.6f).setEase(LeanTweenType.easeOutBounce).setOnComplete
			(
				() => {
					bool bCanCompound = true;
					for (int i = 0; i < Sprite_Compounds.Length; i++)
					{
						if (!Sprite_Compounds[i].activeSelf)
						{
							bCanCompound = false;
							break;
						}
					}
					
					if (bCanCompound)
					{
						Sprite_Compound_Master.SetActive(true);
						Sprite_Compound_Master.transform.localScale = 1.5f * Vector3.one;
						LeanTween.scale(Sprite_Compound_Master, 1.2f * Vector3.one, 0.6f).setEase(LeanTweenType.easeOutBounce).setOnComplete
						(
							() => {
								Btn_Compound.SetActive(true);
							}
						);
					}
				}
			);
		}
	}
	
	private void CompoundPet(GameObject go)
	{
		Vector3 destPos = Sprite_Compound_Master.transform.position;
		for (int i = 0; i < Sprite_Compounds.Length; i++)
		{
			GameObject sprite = Sprite_Compounds[i];
			LeanTween.move(sprite, destPos, 0.6f).setEase(LeanTweenType.easeInOutQuad).setOnComplete
			(
				() => {
					sprite.SetActive(false);
				}
			);
		}
		
		LeanTween.delayedCall(0.6f, () => {
			GameObject obj = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PREFAB_PATH + "Boss_E.prefab");
			m_compoundModel = Instantiate(obj);

			m_compoundModel.transform.position = new Vector3(0.0f, -30.0f, 0.0f);
			m_compoundModel.transform.localScale = Vector3.zero;
			m_compoundModel.transform.Rotate (new Vector3(-30.0f, -45.0f, 15.0f));

			GameObject Model_Camera = GameObject.Find("Model_Camera");
			obj = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PARTICLE_PATH + "2D_Star_01.prefab");
			m_particle = Instantiate(obj);
			m_particle.transform.position = m_mainUICamera.transform.position;

			int layer = LayerMask.NameToLayer ("MenuModel");
			SetGameObjecLayer (m_compoundModel, layer);

			layer = LayerMask.NameToLayer("UI");
			SetGameObjecLayer (m_particle, layer);

			if (Compound != null)
			{
				m_maskBG.SetActive(false);
				Compound.SetActive (false);
			}

			LeanTween.scale(m_compoundModel, 16.0f * Vector3.one, 1.6f).setEase(LeanTweenType.easeOutBounce).setOnComplete
			(
				() => {
					m_screenTap.SetActive(true);
					UIEventListener.Get(m_screenTap).onClick += CloseCompound;
				}
			);
		});
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

		m_bInMain = !blur;
	}

}
