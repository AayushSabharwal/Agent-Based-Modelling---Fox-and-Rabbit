using UnityEngine;

public static class Noise
{
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight]; //noisemap

		System.Random prng = new System.Random(seed);   //pseudo random number generator with given seed, so we can randomise maps
														//same seed generates same map

		Vector2[] octaveOffsets = new Vector2[octaves]; //we want a random offset for our x and y values for each octave
		for (int i = 0; i < octaves; i++) {
			//changing offset.x and .y parameter allows us to scroll through the map, so we add it to our offset for every octave
			float offsetX = prng.Next(-100000, 100000) + offset.x;  //x offset
			float offsetY = prng.Next(-100000, 100000) + offset.y;  //y offset
			octaveOffsets[i] = new Vector2(offsetX, offsetY);   //assigning
		}

		if (scale <= 0f) {
			scale = 0.0001f;    //clamping scale to prevent errors
		}

		float maxNoiseHeight = float.MinValue;  //keeps track of max height we encounter during generation
		float minNoiseHeight = float.MaxValue;  //likewise, min height

		float halfWidth = mapWidth / 2f;    //half of dimensions
		float halfHeight = mapHeight / 2f;  //these are used to improve zooming with noise scale. Instead of top right, it will zoom into middle

		for (int y = 0; y < mapHeight; y++)  //looping over coordinates
{
			for (int x = 0; x < mapWidth; x++) {
				float amplitude = 1f;   //base amplitude
				float frequency = 1f;   //base frequency
				float noiseHeight = 0f; //noise height for this tile

				for (int i = 0; i < octaves; i++)    //we loop through each octave we want to consider
				{
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;  //sample coordinates denote where in the generated perlin noise we want to sample from
					float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y; //dividing by scale prevents only integer values, since perlin noise is same at integers
																							   //multiplying by frequency means we sample over a larger area so our noise varies more in the same range of x,y

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;    //function returns from 0 to 1, we make it -1 to 1
					noiseHeight += perlinValue * amplitude; //update our noise height

					amplitude *= persistence;   //persistence controls how much effect subsequent octaves have. Details have less effect as we go on
					frequency *= lacunarity;    //lacunarity increases frequency across octaves, so details appear
				}

				if (noiseHeight > maxNoiseHeight) {  //update our maxNoiseHeight as we make the map
					maxNoiseHeight = noiseHeight;
				}

				if (noiseHeight < minNoiseHeight) {  //likewise
					minNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;   //apply what we generated
			}
		}

		for (int y = 0; y < mapHeight; y++) { //looping over coordinates, to reclamp values between 0 and 1 such that they retain relative height
			for (int x = 0; x < mapWidth; x++) {  //aka normalise noise map
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}
}
