using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickNoise : MonoBehaviour
{
	public Material brickMat;
	public GameObject brickMesh;
	public int xSize;
	public int ySize;
	public Color brickColor;
	public int perlinOffset;
	public float xSplotching = 0.006f;
	public float ySplotching = 0.006f;
	public float xHoling = .015f;
	public float yHoling = .015f;
	public float holeCutoff = 0.15f;
	public float xLining = 0.002f;
	public float yLining = 0.006f;
	public float lineCutoff = 0.05f;
	//add big holes with normals

    void Start()
    {
		brickMesh.GetComponent<MeshRenderer>().material = brickMat;

		Texture2D brickTexture = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				float zSplotch = Mathf.PerlinNoise(x * xSplotching + perlinOffset, y * ySplotching + perlinOffset);
				float zHole = Mathf.PerlinNoise(x * xHoling - perlinOffset, y * yHoling - perlinOffset);
				float zLine = Mathf.PerlinNoise(x * xLining + perlinOffset, y * yLining - perlinOffset);
				if (zHole <= holeCutoff)
					zHole *= 1f / holeCutoff;
				else
					zHole = 1;
				if (zLine > lineCutoff || zLine < 1 - lineCutoff)
					zLine = 1f;
				else
					zLine /= 1f / lineCutoff * Mathf.PerlinNoise(x * 9.67f - perlinOffset, y * 8.2f + perlinOffset);
				brickTexture.SetPixel(x, y, brickColor * zSplotch * zHole * zLine);
			}
		}
		brickTexture.Apply();

		brickMat.mainTexture = brickTexture;
	}
	
    void Update()
    {
        
    }
}
