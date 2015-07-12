using UnityEngine;
using System.Collections;

public class StraightMultiEffect : MonoBehaviour
{
	public float destroyTime = 0.5f;
	public float MoveTime = 1;
	public float spawnTime = 0.3f;
	private float timeCount = 0;
	private Vector3 StartPosition;
	private Vector3 CurrentPosition;
	public Vector3 TargetPosition;
	private float x;
	private float y;
	private float z;
	private bool mama = false;
	private GameObject tmpObj = null;
	private float moveSpeed = 0;
	public void SetTargetPosition(Vector3 pos)
	{
		mama = true; 
		StartPosition = transform.position;
		CurrentPosition = StartPosition; 
		TargetPosition = pos;
		TargetPosition.y = StartPosition.y;
		moveSpeed = (TargetPosition - CurrentPosition).magnitude;
	}

	public void SetOrigin(GameObject obj)
	{
		tmpObj = obj;
	}
	
	void Start () {
		if (!mama) {
			Destroy (gameObject, destroyTime);
		}
	}
	
	void Update () {
		if (mama) {
			if (CurrentPosition == TargetPosition) {
				Destroy (gameObject, destroyTime);
				mama = false;
			} else {
				CurrentPosition = Vector3.MoveTowards(CurrentPosition, TargetPosition, Time.deltaTime * moveSpeed);
				timeCount += Time.deltaTime;
				if(timeCount >= spawnTime)
				{
					timeCount = 0;
					GameObject obj = Instantiate (tmpObj);
					obj.transform.SetParent(this.transform.parent);
					obj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
					obj.transform.position = CurrentPosition;
					obj.SetActive(true);
				}
			}
		}
	}
}

