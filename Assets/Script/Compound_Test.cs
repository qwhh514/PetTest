
using UnityEngine;
using System.Collections;

using StaticDefine;

public class Compound_Test : MonoBehaviour {

	private GameObject Compound = null;
	private GameObject Sprite_Compound_Master = null;
	private GameObject[] Sprite_Compounds = null;

	private GameObject Btn_Compound = null;
	private GameObject[] Btn_Compounds = null;

	// Use this for initialization
	void Start () {

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

		Btn_Compounds = new GameObject[7];
		for (int i = 0; i < Btn_Compounds.Length; i++)
		{
			string name = "Btn_Compound" + i.ToString();
			Btn_Compounds[i] = GameObject.Find(name);
			UIEventListener.Get(Btn_Compounds[i]).onClick += SelectCompound;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void OpenCompound(GameObject go)
	{
		Compound.SetActive(true);
	}

	public void CloseCompound(GameObject go)
	{
		Compound.SetActive(false);
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
			
			LeanTween.scale(compoundItem, 0.8f * Vector3.one, 0.6f).setEase(LeanTweenType.easeOutBounce).setOnComplete
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
						LeanTween.scale(Sprite_Compound_Master, Vector3.one, 0.6f).setEase(LeanTweenType.easeOutBounce).setOnComplete
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
			CloseCompound(null);

			GameObject obj = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PREFAB_PATH + "Boss_E.prefab");
			GameObject model = Instantiate(obj);
			model.transform.position = new Vector3(0.0f, -2.0f, 0.0f);
			model.transform.localScale = Vector3.zero;
			model.transform.LookAt(Camera.main.transform.position + 6.0f * Vector3.right);

			LeanTween.scale(model, 2.4f * Vector3.one, 0.6f).setEase(LeanTweenType.easeOutBounce).setOnComplete(
				() => {
					obj = null;
					obj = AssetManager.Singleton.LoadAsset<GameObject>(FilePath.PARTICLE_PATH + "2D_Star_01.prefab");
					GameObject particle = Instantiate(obj);
					particle.transform.position = new Vector3(0.0f, 0.0f, 3.0f);
				}
			);
		});
	}

}
