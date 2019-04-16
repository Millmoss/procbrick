using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeramicNoise : MonoBehaviour
{
    public Color ceramicColor;
    public float perlin_impact = 0.1f;
    public float perlin_scale = 0.1f;
	public int perlinOffset = 1000;

    public void GenerateTexture(int xSize, int ySize, ref List<Color> colors, ref List<Color> normals)
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                colors.Add(ceramicColor * (1 - perlin_impact + perlin_impact * Mathf.PerlinNoise(x * perlin_scale + perlinOffset, y * perlin_scale + perlinOffset)) + new Color(0, 0, 0, 255));
            }
        }

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                colors.Add(new Color(.5f, .5f, 1) + new Color(0, 0, 0, 255));
            }

        }
    }
}
