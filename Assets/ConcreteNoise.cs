using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteNoise : MonoBehaviour
{
	public Material concreteMat;
	public GameObject concreteMesh;
	public Color concreteColor;
	public int perlinOffset;
	public float xSplotching = 0.006f;
	public float ySplotching = 0.006f;
	public float xSpotting = .015f;
	public float ySpotting = .015f;
	public float spotCutoff = 0.15f;
	public float xBlotching = 0.01f;
	public float yBlotching = 0.006f;

	private float[,] spots;

    public void GenerateTexture(int xSize, int ySize, ref List<Color> colors, ref List<Color> normals)
    {

		spots = new float[xSize, ySize];

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				float zSplotch = (Mathf.Clamp(Mathf.PerlinNoise(x * xSplotching + perlinOffset, y * ySplotching + perlinOffset), 0.3f, 1f) + 2f) / 3f;
				float zSpot = Mathf.PerlinNoise(x * xSpotting + 2.7f * perlinOffset, y * ySpotting + 2.7f * perlinOffset);
				float zBlotch = (Mathf.Clamp(Mathf.PerlinNoise(x * xBlotching + 10f * perlinOffset, y * yBlotching +  10f * perlinOffset), 0.3f, 1f) + 2f) / 3f;
				if (zSpot <= spotCutoff)
					zSpot *= 1f / spotCutoff;
				else
					zSpot = 1;
				zSpot = (zSpot + 3f) / 4f;
				spots[x, y] = zSpot;
				colors.Add(concreteColor * zSplotch * zBlotch * zSpot + new Color(0, 0, 0, 255));
			}
		}

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				if (x == 0 || y == 0 || y == ySize - 1 || x == xSize - 1 || spots[x, y] >= 0.9f)
				{
                    colors.Add(new Color(.5f, .5f, 1) + new Color(0, 0, 0, 255));
					continue;
				}
				float xDif = spots[x + 1, y] - spots[x, y] + spots[x, y] - spots[x - 1, y];
				xDif = xDif / 2 + spots[x, y] / 2;
				float yDif = spots[x, y + 1] - spots[x, y] + spots[x, y] - spots[x, y - 1];
				yDif = yDif / 2 + (1 - spots[x, y]) / 2;
				Vector3 dir = new Vector3(-xDif, -(xDif + yDif) / 2, -yDif);
				dir += Vector3.one;
				dir /= 2;
                colors.Add(new Color(dir.x, dir.y, dir.z, 255));
			}
		}
	}

	void Update()
    {
        
    }
}
