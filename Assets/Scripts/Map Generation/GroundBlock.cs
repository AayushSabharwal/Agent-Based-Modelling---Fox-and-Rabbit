using UnityEngine;

public class GroundBlock : MonoBehaviour
{

	private BoxCollider boxCollider;
	
	public GroundType groundType;
	public bool isOccupied = false;

	private void Start()
	{
		boxCollider = gameObject.GetComponent<BoxCollider>();
		Init();
	}

	public void Init()
	{
		Color colour;
		if (groundType == GroundType.Grass) {
			colour = Color.green;
			gameObject.tag = "Grass";
			boxCollider.center = new Vector3(0f, 0f, 0f);

		}
		else {
			colour = Color.blue;
			gameObject.tag = "Water";
			boxCollider.center = new Vector3(0f, 1f, 0f);
		}

		gameObject.GetComponent<MeshRenderer>().material.color = colour;
	}

	private void OnTriggerEnter(Collider other)
	{
		isOccupied = true;
	}

	private void OnTriggerExit(Collider other)
	{
		isOccupied = false;
	}
}

[System.Serializable]
public enum GroundType
{
	Water,
	Grass
}