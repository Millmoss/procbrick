using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateTexture : MonoBehaviour
{
    //TODO: Move all these to a seperate class to seperate the Unity side of things from the rest of the code.

    Texture2D texture;
	Texture2D normal;
    public Material mat;
    public ImageNoise im;

    public int num_brick_x, num_brick_y;
    public int resolution_x, resolution_y;
    public int mortar_width, mortar_height;
    private int x_dif, y_dif;
    public BrickNoise bn;
    public ConcreteNoise cn;
    public CeramicNoise rn;
    public Color between_color = Color.gray;
    public int erode_height = 5;
    public bool gen_brick, gen_con, gen_cera, stagger_tiles, use_image_noise;

    private int brick_count_x, brick_count_y;
    private Point[] points;
    private int perlin_offset = 0;


    // Start is called before the first frame update
    //This is just the general render function, should prob push it in its own spot.
    void Start()
    {
        print("E");
        brick_count_x = num_brick_x;
        brick_count_y = num_brick_y;
        x_dif = resolution_x / (brick_count_x);
        y_dif = resolution_y / (brick_count_y);
        brick_count_x++;
        points = new Point[brick_count_x * brick_count_y];

        //Instantate points
        for (int y = 0;y < brick_count_y;y++)
        {
            int mod_y = 1;
            if (stagger_tiles)
                mod_y = y % 2;
            for (int x = 0; x < brick_count_x;x++)
            {
                Point new_point = new Point(x_dif * (x) + (mod_y * x_dif / 2), y_dif * y + y_dif / 2);
                points[x + y * brick_count_x] = new_point;
            }
        }

        //First generate a blank layer of mortar.
        texture = new Texture2D(resolution_x, resolution_y);
        normal = new Texture2D(resolution_x, resolution_y);
        for(int y = 0;y  <resolution_y;y++)
        {
            for(int x=0;x < resolution_x;x++)
            {
                float pn;
                pn = Mathf.PerlinNoise(x * .05f, y * .05f);

                texture.SetPixel(x, y, between_color * (0.95f + pn * .05f) + new Color(0, 0, 0, 255));
				normal.SetPixel(x, y, new Color(.5f, .5f, 1));
				//texture.SetPixel(x, y, between_color * pn + new Color(0, 0, 0, 255));
				perlin_offset++;
            }
        }
        //Set positions of point's colors.
        for (int p_x = 0; p_x < brick_count_x - 0; p_x++)
        {
            for (int p_y = 0; p_y < brick_count_y - 0; p_y++)
            {
                Point p = getPoint(p_x, p_y);
                Point r = getPoint(p_x + 1, p_y);
                Point d = getPoint(p_x, p_y + 1);


                List<int> ign_x_top = generateRandomWalk((y_dif - mortar_width) / 2, 
                    (x_dif - mortar_height) / 2 - (-x_dif + mortar_height) / 2,
                    p_x + p_y * 10);
                List<int> ign_x_bottom = generateRandomWalk((-y_dif + mortar_width) / 2, 
                    (x_dif - mortar_height) / 2 - (-x_dif + mortar_height) / 2,
                    p_x + p_y * 10);

                List<int> ign_y_right = generateRandomWalk((-x_dif + mortar_height) / 2,
                    (y_dif - mortar_width) / 2 - (-y_dif + mortar_width) / 2,
                    p_x + p_y * 10);
                List<int> ign_y_left = generateRandomWalk((x_dif - mortar_height) / 2,
                    (y_dif - mortar_width) / 2 - (-y_dif + mortar_width) / 2,
                    p_x + p_y * 10);

                int mid_x = (p.x + x_dif / 2);
                int mid_y = (p.y + y_dif / 2);

                if (r != null)
                {
                    mid_x = (p.x + r.x) / 2;
                }
                if (d != null)
                {
                    mid_y = (p.y + d.y) / 2;
                }

                //Generate the brick texture at each brick.
                List<Color> colors = new List<Color>();
                List<Color> normals = new List<Color>();
                if(gen_brick)
                { 
                    bn.GenerateTexture(x_dif - mortar_width, y_dif - mortar_height, ref colors, ref normals, (int)(Random.value * 10000f));
                    bn.perlinOffset -= (int)(1000 * Random.value);
                }
                else if (gen_con)
                {
                    cn.GenerateTexture(x_dif - mortar_width, y_dif - mortar_height, ref colors, ref normals);
                    cn.perlinOffset -= (int)(1000 * Random.value);
                }
                else if (gen_cera)
                {
                    rn.GenerateTexture(x_dif - mortar_width, y_dif - mortar_height, ref colors, ref normals);
                    rn.perlinOffset -= (int)(1000 * Random.value);
                }
                int x_count = 0, y_count = 0;
                for (int y = (-y_dif + mortar_width) / 2; y < (y_dif - mortar_width) / 2; y++)
                {
                    for (int x = (-x_dif + mortar_height) / 2; x < (x_dif - mortar_height) / 2; x++)
                    {
                        
                        if (ign_x_top[x_count]> y && ign_x_bottom[x_count] < y &&
                            ign_y_right[y_count] < x && ign_y_left[y_count] > x)
                        { 
                            if (p.x + x >= 0 && p.x + x < resolution_x &&
                                p.y + y >= 0 && p.y + y < resolution_y)
                            {
								normal.SetPixel(p.x + x, p.y + y, normals[x_count + y_count * (x_dif - mortar_width / 1)]);
                                if (use_image_noise)
                                    texture.SetPixel(p.x + x, p.y + y, im.GetNoise(p.x + x, p.y + y));
                                else
                                    texture.SetPixel(p.x + x, p.y + y, colors[x_count + y_count * (x_dif - mortar_width / 1)]);
                            }
                        }
                        x_count++;
                    }
                    y_count++;
                    x_count = 0;
                }

            }
        }

        for (int p_x = 0; p_x < brick_count_x; p_x++)
            for (int p_y = 0; p_y < brick_count_y; p_y++)
            {
                Point p = getPoint(p_x,p_y);
                //texture.SetPixel(p.x, p.y, Color.red);
            }

        texture.Apply();
        mat.mainTexture = texture;

		byte[] bytes = ImageConversion.EncodeToPNG(normal);
		FileStream file = File.Open(Application.dataPath + "/n.png", FileMode.Create);
		BinaryWriter binary = new BinaryWriter(file);
		binary.Write(bytes);
		file.Close();

        bytes = ImageConversion.EncodeToPNG(texture);
        file = File.Open(Application.dataPath + "/texture.png", FileMode.Create);
        binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
    }

    private List<int> generateRandomWalk(int init_pos, int length, int _rand)
    {
        List<int> ret = new List<int>();
        Random.InitState(init_pos + Mathf.CeilToInt(Time.time) + _rand);
        ret.Add(init_pos);
        for(int i=0;i < length - 1;i++)
        {
            int rand = Random.Range(-1, 2);
            if (Mathf.Abs(ret[i] + rand - init_pos) < erode_height)
                ret.Add(ret[i] + rand);
            else
                ret.Add(ret[i]);
        }
        return ret;
    }

    //Returns null if the point is invalid
    private Point getPoint(int x, int y)
    {
        if (x >= brick_count_x || y >= brick_count_y)
            return null;
        return points[x + y * brick_count_x];
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            Start();
    }
}
