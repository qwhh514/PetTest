using UnityEngine;
using System.Collections;

public class StraightEffect : MonoBehaviour
{
	public float destroyTime = 0.5f;
	public float MoveTime = 1;
	private Vector3 StartPosition;
	public Vector3 TargetPosition;
	private float x;
	private float y;
	private float z;
	public void SetTargetPosition(Vector3 pos)
	{
		StartPosition = transform.position;
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
		if (transform.position == TargetPosition) {
			Destroy (gameObject, destroyTime);
		} else {
			Vector3 newPos;
			Vector3 diff = TargetPosition - transform.position;
			if(Mathf.Abs(diff.x) <= Mathf.Abs(x))
			{
				newPos.x = TargetPosition.x;
			}
			else
			{
				newPos.x = transform.position.x + x;
			}
			if(Mathf.Abs(diff.y) <= Mathf.Abs(y))
			{
				newPos.y = TargetPosition.y;
			}
			else
			{
				newPos.y = transform.position.y + y;
			}
			if(Mathf.Abs(diff.z) <= Mathf.Abs(z))
			{
				newPos.z = TargetPosition.z;
			}
			else
			{
				newPos.z = transform.position.z + z;
			}
			transform.position = newPos;
		}
	}
}

