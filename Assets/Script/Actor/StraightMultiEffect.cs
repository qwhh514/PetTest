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
	public void SetTargetPosition(Vector3 pos)
	{
		mama = true; 
		StartPosition = transform.position;
		CurrentPosition = StartPosition;
		TargetPosition = pos;
		TargetPosition.y = StartPosition.y;
		if (Mathf.Abs (TargetPosition.x) > Mathf.Abs (StartPosition.x)) {
			x = Mathf.Lerp (Mathf.Abs (StartPosition.x), Mathf.Abs (TargetPosition.x), Time.deltaTime);
			if (TargetPosition.x < StartPosition.x)
			{
				x = -x;
			}
		} else {
			x = Mathf.Lerp (Mathf.Abs (TargetPosition.x), Mathf.Abs (StartPosition.x), Time.deltaTime);
			if (TargetPosition.x < StartPosition.x) {
				x = -x;
			}
		}
		if (Mathf.Abs (TargetPosition.y) > Mathf.Abs (StartPosition.y)) {
			y = Mathf.Lerp (Mathf.Abs (StartPosition.y), Mathf.Abs (TargetPosition.y), Time.deltaTime);
			if (TargetPosition.y < StartPosition.y)
			{
				y = -y;
			}
		} else {
			y = Mathf.Lerp (Mathf.Abs (TargetPosition.y), Mathf.Abs (StartPosition.y), Time.deltaTime);
			if (TargetPosition.y < StartPosition.y) {
				y = -y;
			}
		}
		if (Mathf.Abs (TargetPosition.z) > Mathf.Abs (StartPosition.z)) {
			z = Mathf.Lerp (Mathf.Abs (StartPosition.z), Mathf.Abs (TargetPosition.z), Time.deltaTime);
			if (TargetPosition.z < StartPosition.z)
			{
				z = -z;
			}
		} else {
			z = Mathf.Lerp (Mathf.Abs (TargetPosition.z), Mathf.Abs (StartPosition.z), Time.deltaTime);
			if (TargetPosition.z < StartPosition.z)
			{
				z = -z;
			}
		}
	}
	
	void Start () {
	}
	
	void Update () {
		if (mama) {
			if (CurrentPosition == TargetPosition) {
				Destroy (gameObject, destroyTime);
				mama = false;
			} else {
				Vector3 newPos;
				Vector3 diff = TargetPosition - CurrentPosition;
				if(Mathf.Abs(diff.x) <= Mathf.Abs(x))
				{
					newPos.x = TargetPosition.x;
				}
				else
				{
					newPos.x = CurrentPosition.x + x;
				}
				if(Mathf.Abs(diff.y) <= Mathf.Abs(y))
				{
					newPos.y = TargetPosition.y;
				}
				else
				{
					newPos.y = CurrentPosition.y + y;
				}
				if(Mathf.Abs(diff.z) <= Mathf.Abs(z))
				{
					newPos.z = TargetPosition.z;
				}
				else
				{
					newPos.z = CurrentPosition.z + z;
				}

				CurrentPosition = newPos;
				timeCount += Time.deltaTime;
				if(timeCount >= spawnTime)
				{
					timeCount = 0;
					GameObject obj = Instantiate (gameObject, CurrentPosition, transform.rotation) as GameObject;
					obj.transform.SetParent(this.transform);
					//obj.transform.position = CurrentPosition;
					//obj.transform.localPosition = Vector3.zero;
					obj.SetActive(true);
				}
			}
		}
	}
}

