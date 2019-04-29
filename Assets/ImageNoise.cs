using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralNoiseProject;

public class ImageNoise : MonoBehaviour
{
	public Texture2D img;
	public Material imgmat;
	public float perlinMult = .05f;
	public float perlinOffset = 100f;
	public int xSize = 1024;
	public int ySize = 1024;
	private float[,] pixelAvgs;
	private float hi;
	private float lo;
	private Color avgColor;
	private Color hiColor;
	private Color loColor;
    private Texture2D outTexture;

    void Awake()
    {
		pixelAvgs = new float[img.width, img.height];
		xSize = img.width;
		ySize = img.height;
		//pixelAvgs = new float[xSize, ySize];
		Color c = img.GetPixel(0, 0);
		hi = (c.r + c.g + c.b) / 3f;
		lo = (c.r + c.g + c.b) / 3f;
		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				c = img.GetPixel(x, y);
				pixelAvgs[x, y] = (c.r + c.g + c.b) / 3f;
				if (pixelAvgs[x, y] < lo)
					lo = pixelAvgs[x, y];
				if (pixelAvgs[x, y] > hi)
					hi = pixelAvgs[x, y];
				avgColor += new Color(c.r / (float)(xSize * ySize), c.g / (float)(xSize * ySize), c.b / (float)(xSize * ySize));
				//pixelAvgs[y, x] = Mathf.PerlinNoise(((float)x) * perlinMult + perlinOffset, ((float)y) * perlinMult + perlinOffset);
			}
		}

		float centColor = (avgColor.r + avgColor.g + avgColor.b) / 3f;
		hiColor = avgColor * hi;
		loColor = (avgColor * lo + avgColor * 2f) / 2f;

		float xft = 0;
		for (int y = 0; y < ySize; y++)
		{
			float xf = 0;
			for (int x = 1; x < xSize; x++)
			{
				xf += Mathf.Abs(pixelAvgs[x, y] - pixelAvgs[x - 1, y]) * ((xSize / 2.0f) / xSize);
			}
			xft += xf * ((ySize / 2.0f) / ySize);
		}
		print(xft);

		float yft = 0;
		for (int x = 0; x < xSize; x++)
		{
			float yf = 0;
			for (int y = 1; y < ySize; y++)
			{
				yf += Mathf.Abs(pixelAvgs[x, y] - pixelAvgs[x, y - 1]) * ((ySize / 2.0f) / ySize);
			}
			yft += yf * ((xSize / 2.0f) / xSize);
		}
		print(yft);

		float ft = (xft + yft) / (.2f * xSize * ySize);
		print(ft);

		outTexture = new Texture2D(xSize, ySize);

		Noise sn = new SimplexNoise(1, ft);
		Noise pn = new PerlinNoise(1, ft);
		Noise vn = new ValueNoise(1, 1);

		for (int y = 0; y < ySize; y++)
		{
			for (int x = 0; x < xSize; x++)
			{
				float p = (sn.Sample2D(((float)x) + perlinOffset, ((float)y) * 2f + perlinOffset) + sn.Sample2D(((float)x) * 2f + perlinOffset, ((float)y) * 4f + perlinOffset) + sn.Sample2D(((float)x) * 4f + perlinOffset, ((float)y) * 8f + perlinOffset)) / 3f;
				if (p > .5f)
					c = Color.Lerp(avgColor, hiColor, (p - .5f) * 2f);
				else
					c = Color.Lerp(loColor, avgColor, p * 2f);
				outTexture.SetPixel(x, y, c);
			}
		}
		outTexture.Apply();
        
        imgmat.mainTexture = outTexture;
    }

    public Color GetNoise(int x, int y)
    {
        Color tmp = outTexture.GetPixel(x, y);
        return tmp;
    }
	
    void Update()
    {

    }
}
