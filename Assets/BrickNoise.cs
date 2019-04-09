using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickNoise : MonoBehaviour
{
	public Material brickMat;
	public GameObject brickMesh;
	//public int xSize;
	//public int ySize;
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

	private float[,] holes;

    public void GenerateTexture(int xSize, int ySize, ref List<Color> colors, ref List<Color> normals)
    {/*
		brickMesh.GetComponent<MeshRenderer>().material = brickMat;
        
        Texture2D brickTexture = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);
        Texture2D brickNormals = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);*/
        holes = new float[xSize, ySize];

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				float zSplotch = Mathf.PerlinNoise(x * xSplotching + perlinOffset, y * ySplotching + perlinOffset);
				zSplotch = (zSplotch + .5f) / 1.5f;
				float zHole = Mathf.PerlinNoise(x * xHoling - perlinOffset, y * yHoling - perlinOffset);
				float zLine = Mathf.PerlinNoise(x * xLining + perlinOffset, y * yLining - perlinOffset);
				if (zHole <= holeCutoff)
					zHole *= 1f / holeCutoff;
				else
					zHole = 1;
				holes[x, y] = zHole;
				if (zLine > lineCutoff || zLine < 1 - lineCutoff)
					zLine = 1f;
				else
					zLine /= 1f / lineCutoff * Mathf.PerlinNoise(x * 9.67f - perlinOffset, y * 8.2f + perlinOffset);
				colors.Add(brickColor * zSplotch * zHole * zLine);
			}
		}

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				if (x == 0 || y == 0 || y == ySize - 1 || x == xSize - 1 || holes[x, y] >= 0.9f)
				{
					normals.Add(new Color(.5f, .5f, 1));
					continue;
				}
				float xDif = holes[x + 1, y] - holes[x, y] + holes[x, y] - holes[x - 1, y];
				xDif = xDif / 2 + holes[x, y] / 2;
				float yDif = holes[x, y + 1] - holes[x, y] + holes[x, y] - holes[x, y - 1];
				yDif = yDif / 2 + holes[x, y] / 2;
				Vector3 dir = new Vector3(-xDif, -(xDif + yDif) / 2, -yDif * 2);
				dir += Vector3.one;
				dir /= 2;
                normals.Add(new Color(dir.x, dir.y, dir.z));
			}
		}
        /*
		brickNormals.Apply();

		brickMat.SetTexture("_MainTex", brickTexture);
		brickMat.SetTexture("_BumpMap", brickNormals);*/
	}
	
    void Update()
    {
        
    }
}
