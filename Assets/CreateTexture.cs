using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTexture : MonoBehaviour
{
    //TODO: Move all these to a seperate class to seperate the Unity side of things from the rest of the code.

    Texture2D texture;
    public Material mat;

    public int brick_count_x, brick_count_y;
    public int resolution_x, resolution_y;
    private int x_dif, y_dif;
    public BrickNoise bn;
    private Point[] points;

    // Start is called before the first frame update
    void Start()
    {


        x_dif = resolution_x / (brick_count_x);
        y_dif = resolution_y / (brick_count_y);
        brick_count_x++;
        points = new Point[brick_count_x * brick_count_y];

        //Instantate points
        for (int y = 0;y < brick_count_y;y++)
        {
            for(int x = 0; x < brick_count_x;x++)
            {
                Point new_point = new Point(x_dif * (x) + ((y % 2) * x_dif / 2), y_dif * y + y_dif / 2);
                points[x + y * brick_count_x] = new_point;
            }
        }

        texture = new Texture2D(resolution_x, resolution_y);
        
        //Set positions of point's colors.
        for(int p_x = 0; p_x < brick_count_x - 0;p_x++)
            for (int p_y = 0; p_y < brick_count_y - 0; p_y++)
            {
                Point p = getPoint(p_x, p_y);
                Point r = getPoint(p_x + 1, p_y);
                Point d = getPoint(p_x, p_y + 1);

                int mid_x = (p.x + x_dif/2);
                int mid_y = (p.y + y_dif / 2);

                if ( r != null)
                {
                    mid_x = (p.x + r.x) / 2;
                }
                if (d != null)
                {
                    mid_y = (p.y + d.y) / 2;
                }
                /*
                for (int x = 1; x < x_dif * 2; x++)
                {
                    for (int y = 1; y < y_dif; y++)
                    {
                        texture.SetPixel(mid_x + x - x_dif, mid_y + y - y_dif, colors[x + y * (x_dif * 2 - 1)]);
                    }
                }*/

                

                //Generate the borders.
                for (int x = -x_dif/1 ; x < x_dif / 2 ;x++)
                {
                    if (mid_x + x >= 0 && mid_x + x < resolution_x)
                        texture.SetPixel(mid_x + x, mid_y, Color.blue);
                }
                for (int y = -y_dif; y <  0; y++)
                {
                    //Make sure that the set pixel fits the bounds.
                   if(mid_y + y >= 0 && mid_y + y < resolution_y)
                        texture.SetPixel(mid_x, mid_y + y, Color.green);
                }

                List<Color> colors = new List<Color>();
                List<Color> normals = new List<Color>();

                bn.GenerateTexture(x_dif - 1, y_dif - 1, ref colors, ref normals);
                bn.perlinOffset++;
                int x_count = 0, y_count = 0;
                for(int y = -y_dif / 2 + 1; y < y_dif / 2 ; y++)
                {
                    for (int x = -x_dif/2 + 1; x < x_dif / 2; x++)
                    {
                        if (p.x + x >= 0 && p.x + x < resolution_x &&
                            p.y + y >= 0 && p.y + y < resolution_y)
                            texture.SetPixel(p.x + x , p.y + y, colors[x_count + y_count * (x_dif - 1)]);
                        x_count++;
                    }
                    y_count++;
                    x_count = 0;
                }

            }


        for (int p_x = 0; p_x < brick_count_x; p_x++)
            for (int p_y = 0; p_y < brick_count_y; p_y++)
            {
                Point p = getPoint(p_x,p_y);
                texture.SetPixel(p.x, p.y, Color.red);
            }

        texture.Apply();
        mat.mainTexture = texture;
    }

    //Returns null if the point is invalid
    private Point getPoint(int x, int y)
    {
        if (x >= brick_count_x || y >= brick_count_y)
            return null;
        return points[x + y * brick_count_x];
    }
}
