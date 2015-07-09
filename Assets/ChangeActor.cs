using UnityEngine;
using System.Collections;

public class ChangeActor : MonoBehaviour {
	private GameObject obj = null;
	private GameObject obj2 = null;
	private GameObject objLoad = null;
	public int CurrentActor = 0;
	private Vector3 SpawnPosition;
	private Vector3 SpawnPosition2;
	private string[] actorNames = new string[4]
	{
		"Prefabs/Beast",
		"Prefabs/Dragon",
		"Prefabs/Flower",
		"Prefabs/Ghost"
	};
	// Use this for initialization
	void Start () {
		SpawnPosition.Set (0, 1, -8);
		SpawnPosition2.Set (0, 1, 0);
		obj = CreateActor (actorNames [0].ToString (), SpawnPosition, SpawnPosition2);
		obj2 = CreateActor (actorNames [1].ToString (), SpawnPosition2, SpawnPosition);

		obj.SendMessage ("SetTarget", obj2);
		obj2.SendMessage ("SetTarget", obj);
	}

	GameObject CreateActor (string src, Vector3 position, Vector3 lookAt)
	{
		Quaternion quat = Quaternion.identity;
		quat.SetLookRotation (lookAt - position);
		objLoad = Resources.Load (src) as GameObject;
		return Instantiate (objLoad, position, quat) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp (KeyCode.Alpha1))
		{
			Destroy (obj);
			Destroy (obj2);

			CurrentActor = 0;
			int current2 = CurrentActor + 1;
			if (current2 >= 4) {
				current2 = 0;
			}

			obj = CreateActor (actorNames [CurrentActor].ToString (), SpawnPosition, SpawnPosition2);
			obj2 = CreateActor (actorNames [current2].ToString (), SpawnPosition2, SpawnPosition);

			obj.SendMessage ("SetTarget", obj2);
			obj2.SendMessage ("SetTarget", obj);
		}
		else if(Input.GetKeyUp (KeyCode.Alpha2))
		{
			Destroy (obj);
			Destroy (obj2);
			
			CurrentActor = 1;
			int current2 = CurrentActor + 1;
			if (current2 >= 4) {
				current2 = 0;
			}
			
			obj = CreateActor (actorNames [CurrentActor].ToString (), SpawnPosition, SpawnPosition2);
			obj2 = CreateActor (actorNames [current2].ToString (), SpawnPosition2, SpawnPosition);

			obj.SendMessage ("SetTarget", obj2);
			obj2.SendMessage ("SetTarget", obj);
		}
		else if(Input.GetKeyUp (KeyCode.Alpha3))
		{
			Destroy (obj);
			Destroy (obj2);
			
			CurrentActor = 2;
			int current2 = CurrentActor + 1;
			if (current2 >= 4) {
				current2 = 0;
			}
			
			obj = CreateActor (actorNames [CurrentActor].ToString (), SpawnPosition, SpawnPosition2);
			obj2 = CreateActor (actorNames [current2].ToString (), SpawnPosition2, SpawnPosition);

			obj.SendMessage ("SetTarget", obj2);
			obj2.SendMessage ("SetTarget", obj);
		}
		else if(Input.GetKeyUp (KeyCode.Alpha4))
		{
			Destroy (obj);
			Destroy (obj2);
			
			CurrentActor = 3;
			int current2 = CurrentActor + 1;
			if (current2 >= 4) {
				current2 = 0;
			}
			
			obj = CreateActor (actorNames [CurrentActor].ToString (), SpawnPosition, SpawnPosition2);
			obj2 = CreateActor (actorNames [current2].ToString (), SpawnPosition2, SpawnPosition);

			obj.SendMessage ("SetTarget", obj2);
			obj2.SendMessage ("SetTarget", obj);
		}
	}
}
