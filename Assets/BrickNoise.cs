﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralNoiseProject;

public class BrickNoise : MonoBehaviour
{
	public Material brickMat;
	public GameObject brickMesh;
	public int seed = 0;
	//public int xSize;
	//public int ySize;
	public Color brickColor;
	public int perlinOffset;
	public float xSplotching = 0.006f;
	public float ySplotching = 0.006f;
	public float xHoling = .015f;
	public float yHoling = .015f;
	public float holeCutoff = 0.15f;
	public float crackChance = .1f;
	public float crackIncrement = 1.5f;
	public int crackWidth = 2;
	public float crackShift = .1f;

	private float[,] pixels;
	private float[,] cracks;
	private float[,] holes;

    public void GenerateTexture(int xSize, int ySize, ref List<Color> colors, ref List<Color> normals, int r)
    {/*
		brickMesh.GetComponent<MeshRenderer>().material = brickMat;
        
        Texture2D brickTexture = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);
        Texture2D brickNormals = new Texture2D(xSize, ySize, TextureFormat.ARGB32, false);*/
        holes = new float[xSize, ySize];
        pixels = new float[xSize, ySize];
		cracks = new float[xSize, ySize];

		Noise pn = new SimplexNoise(seed, .2f);

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				float zSplotch = pn.Sample2D(x * xSplotching + perlinOffset, y * ySplotching + perlinOffset);
				zSplotch = (zSplotch + 3.5f) / 4.5f;
				float zHole = Mathf.PerlinNoise(x * xHoling - perlinOffset, y * yHoling - perlinOffset);
				if (zHole <= holeCutoff)
					zHole *= 1f / holeCutoff;
				else
					zHole = 1;
				holes[x, y] = zHole;
				cracks[x, y] = 1;
				pixels[x, y] = zSplotch * zHole;
			}
		}

		Random.InitState(r);
		if (Random.value <= crackChance)
		{
			float xCase = Random.value;
			float yCase = Random.value;
			Vector2 position = Vector2.zero;
			Vector2 direction = Vector2.zero;
			if (xCase < .5f)
			{
				if (yCase < .5f)
				{
					position = new Vector2(0, Random.value * (ySize - 1));
					direction = new Vector2(1, 0);
				}
				else
				{
					position = new Vector2(xSize - 1, Random.value * (ySize - 1));
					direction = new Vector2(-1, 0);
				}
			}
			else
			{
				if (yCase < .5f)
				{
					position = new Vector2(Random.value * (xSize - 1), 0);
					direction = new Vector2(0, 1);
				}
				else
				{
					position = new Vector2(Random.value * (xSize - 1), ySize - 1);
					direction = new Vector2(0, -1);
				}
			}

			cracks[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)] = 0;
			for (int y = -crackWidth; y <= crackWidth; y++)
			{
				for (int x = -crackWidth; x <= crackWidth; x++)
				{
					if (Mathf.RoundToInt(position.x) + x < 0 || Mathf.RoundToInt(position.y) + y < 0 || 
						Mathf.RoundToInt(position.y) + y > ySize - 1 || Mathf.RoundToInt(position.x) + x > xSize - 1 || 
						(x == 0 && y == 0))
					{
						continue;
					}

					cracks[Mathf.RoundToInt(position.x) + x, Mathf.RoundToInt(position.y) + y] = 0;// (Mathf.Abs(x) + Mathf.Abs(y)) / (2 * crackWidth);
				}
			}
			position += direction * crackIncrement;

			while (position.x >= 0 && position.y >= 0 && position.y <= ySize - 1 && position.x <= xSize - 1)
			{
				cracks[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)] = 0;
				for (int y = -crackWidth; y <= crackWidth; y++)
				{
					for (int x = -crackWidth; x <= crackWidth; x++)
					{
						if (Mathf.RoundToInt(position.x) + x < 0 || Mathf.RoundToInt(position.y) + y < 0 ||
							Mathf.RoundToInt(position.y) + y > ySize - 1 || Mathf.RoundToInt(position.x) + x > xSize - 1 ||
							(x == 0 && y == 0))
						{
							continue;
						}

						cracks[Mathf.RoundToInt(position.x) + x, Mathf.RoundToInt(position.y) + y] = 0;// Mathf.Pow((Mathf.Abs(x) + Mathf.Abs(y)) / (2 * crackWidth), 2);
					}
				}
				float xShift = 2 * (Random.value - .5f) * crackShift;
				float yShift = 2 * (Random.value - .5f) * crackShift;
				direction += new Vector2(xShift, yShift);
				direction.Normalize();
				position += direction * crackIncrement;
			}
		}

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				if (x == 0 || y == 0 || y == ySize - 1 || x == xSize - 1 || holes[x, y] >= 0.9f)
				{
					colors.Add(brickColor * pixels[x, y] * cracks[x, y]);
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
				colors.Add(brickColor * pixels[x, y] * cracks[x, y]);
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
