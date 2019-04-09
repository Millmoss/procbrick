using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteNoise : MonoBehaviour
{
	public Material concreteMat;
	public GameObject concreteMesh;
	public int xSize;
	public int ySize;
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

	void Start()
	{
		concreteMesh.GetComponent<MeshRenderer>().material = concreteMat;
		spots = new float[xSize, ySize];

		Texture2D concreteTexture = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);
		Texture2D concreteNormals = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				float zSplotch = (Mathf.Clamp(Mathf.PerlinNoise(x * xSplotching + perlinOffset, y * ySplotching + perlinOffset), 0.3f, 1f) + .5f) / 1.5f;
				float zSpot = Mathf.PerlinNoise(x * xSpotting - perlinOffset, y * ySpotting - perlinOffset);
				float zBlotch = (Mathf.Clamp(Mathf.PerlinNoise(x * xBlotching - perlinOffset, y * yBlotching - perlinOffset), 0.3f, 1f) + .5f) / 1.5f;
				if (zSpot <= spotCutoff)
					zSpot *= 1f / spotCutoff;
				else
					zSpot = 1;
				spots[x, y] = zSpot;
				concreteTexture.SetPixel(x, y, concreteColor * zSplotch * zBlotch * zSpot);
			}
		}
		concreteTexture.Apply();

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				if (x == 0 || y == 0 || y == ySize - 1 || x == xSize - 1 || spots[x, y] >= 0.9f)
				{
					concreteNormals.SetPixel(x, y, new Color(.5f, .5f, 1));
					continue;
				}
				float xDif = spots[x + 1, y] - spots[x, y] + spots[x, y] - spots[x - 1, y];
				xDif = xDif / 2 + spots[x, y] / 2;
				float yDif = spots[x, y + 1] - spots[x, y] + spots[x, y] - spots[x, y - 1];
				yDif = yDif / 2 + (1 - spots[x, y]) / 2;
				Vector3 dir = new Vector3(-xDif, -(xDif + yDif) / 2, -yDif);
				dir += Vector3.one;
				dir /= 2;
				concreteNormals.SetPixel(x, y, new Color(dir.x, dir.y, dir.z));
			}
		}
		concreteNormals.Apply();

		concreteMat.SetTexture("_MainTex", concreteTexture);
		concreteMat.SetTexture("_BumpMap", concreteNormals);
	}

	void Update()
    {
        
    }
}
