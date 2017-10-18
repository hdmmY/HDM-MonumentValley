using UnityEngine;

public class OutlineHelper : MonoBehaviour {

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(0, 0, 0, 0.5f);

		for(int x = -100; x <= 100; x += 1)
		{
			Vector3 centre = new Vector3(x, 0, 0);
			Gizmos.DrawLine(centre + Vector3.forward * 100, centre - Vector3.forward * 100);
		}

		for(int z = -100; z <= 100; z += 1)
		{
			Vector3 centre = new Vector3(0, 0, z);
			Gizmos.DrawLine(centre + Vector3.right * 100, centre - Vector3.right * 100);
		}
	}
}
