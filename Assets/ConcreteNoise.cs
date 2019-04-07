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

	void Start()
	{
		concreteMesh.GetComponent<MeshRenderer>().material = concreteMat;

		Texture2D concreteTexture = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);

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
				concreteTexture.SetPixel(x, y, concreteColor * zSplotch * zBlotch);
			}
		}
		concreteTexture.Apply();

		concreteMat.mainTexture = concreteTexture;
	}

	void Update()
    {
        
    }
}
