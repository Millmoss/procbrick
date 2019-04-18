using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretExperimentalScript : MonoBehaviour
{
	public Texture2D img;
	public Material imgmat;
	public float perlinMult = .05f;
	public float perlinOffset = 100f;
	public int xSize = 1024;
	public int ySize = 1024;
	private float[,] pixelAvgs;

    void Start()
    {
		pixelAvgs = new float[img.width, img.height];
		xSize = img.width;
		ySize = img.height;
		//pixelAvgs = new float[xSize, ySize];
		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				Color c = img.GetPixel(x, y);
				pixelAvgs[x, y] = (c.r + c.g + c.b) / 3f;
				//pixelAvgs[y, x] = Mathf.PerlinNoise(((float)x) * perlinMult + perlinOffset, ((float)y) * perlinMult + perlinOffset);
			}
		}

		float xft = 0;
		for (int y = 0; y < ySize; y++)
		{
			float xf = 0;
			for (int x = 1; x < xSize; x++)
			{
				xf += Mathf.Abs(pixelAvgs[x, y] - pixelAvgs[x - 1, y]) * (((float)x) / xSize);
			}
			xft += xf * (((float)y) / ySize);
		}
		print(xft);

		float yft = 0;
		for (int x = 0; x < xSize; x++)
		{
			float yf = 0;
			for (int y = 1; y < ySize; y++)
			{
				yf += Mathf.Abs(pixelAvgs[x, y] - pixelAvgs[x, y - 1]) * (((float)y) / ySize);
			}
			yft += yf * (((float)x) / xSize);
		}
		print(yft);

		float ft = (xft + yft) / (.2f * xSize * ySize);
		print(ft);

		Texture2D outTexture = new Texture2D(xSize, ySize);

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				float p = Mathf.PerlinNoise(((float)x) * ft + perlinOffset, ((float)y) * ft + perlinOffset);
				Color c = new Color(p, p, p);
				outTexture.SetPixel(x, y, c);
			}
		}
		outTexture.Apply();

		imgmat.mainTexture = outTexture;
	}
	
    void Update()
    {

    }
}
