using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[SerializeField]
	int width = 256;
	[SerializeField]
	int length = 256;
	[SerializeField]
	GameObject blockPrefab;
	[SerializeField]
	[Range(0f, 1f)]
	float surfaceLevel;
	[SerializeField]
	int seed;
	[SerializeField]
	float scale;
	[SerializeField]
	Vector2 offset;
	[SerializeField]
	Transform groundCubesParent;

	public bool autoUpdate = false;

	float[,] heightMap;

	public void GenerateMap()
	{
		heightMap = Noise.GenerateNoiseMap(width, length, seed, scale, 1, 1f, 1f, offset);

		float topLeftX = -width / 2;
		float topLeftZ = -length / 2;

		if (groundCubesParent.childCount != width * length) {
			foreach (Transform child in groundCubesParent) {
				if (!Application.isPlaying) {
					DestroyImmediate(child.gameObject);
				}
				else {
					Destroy(child.gameObject);
				}
			}

			for (int i = 0; i < width; i++) {
				for (int j = 0; j < length; j++) {
					GameObject g;
					//if (Application.isPlaying) {
					g = Instantiate(blockPrefab, new Vector3(topLeftX + i, 0f, topLeftZ + j), Quaternion.identity);
					g.transform.parent = groundCubesParent;
					//}
					//else {
					//	g = (GameObject) PrefabUtility.InstantiatePrefab(blockPrefab, groundCubesParent);
					//	g.transform.position = new Vector3(topLeftX + i, 0f, topLeftZ + j);
					//	g.transform.rotation = Quaternion.identity;
					//}

					GroundBlock gb = g.GetComponent<GroundBlock>();
					if (heightMap[i, j] < surfaceLevel) {
						gb.groundType = GroundType.Water;
					}
					else {
						gb.groundType = GroundType.Grass;
					}
					gb.Init();
				}
			}
		}
		else {
			foreach (Transform child in groundCubesParent) {
				GroundBlock gb = child.gameObject.GetComponent<GroundBlock>();
				if (heightMap[(int) (child.position.x - topLeftX), (int) (child.position.z - topLeftZ)] < surfaceLevel) {
					gb.groundType = GroundType.Water;
				}
				else {
					gb.groundType = GroundType.Grass;
				}
				gb.Init();
			}
		}


	}
}
