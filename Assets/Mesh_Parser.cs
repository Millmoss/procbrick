using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_Parser : MonoBehaviour
{
    Texture2D texture;

    Mesh m;
    Dictionary<string, Vector2Int> edges_dic;

    Vector3[] verts;
    Dictionary<Vector2Int,List<int>> faces;
    List<int> corners;
    List<Vector2Int> edges;
    public float resolution = 0.2f;
    public GameObject cube_corn,cube_edge,cube_face;
    

    // Start is called before the first frame update
    void Start()
    {
        cube_corn.transform.localScale = new Vector3(resolution, resolution, resolution);
        cube_edge.transform.localScale = new Vector3(resolution, resolution, resolution);
        cube_face.transform.localScale = new Vector3(resolution, resolution, resolution);
        
        m = GetComponent<MeshFilter>().mesh;
        edges_dic = new Dictionary<string, Vector2Int>();
        edges = new List<Vector2Int>();
        corners = new List<int>();
        faces = new Dictionary<Vector2Int, List<int>>();
        verts = m.vertices;
        Dictionary<Vector2Int, int> edge_garb = new Dictionary<Vector2Int, int>();
        //This isn't hardcoded.
        for (int i = 0; i < m.triangles.Length; i += 3)
        {
            Vector2Int eOne = new Vector2Int(Mathf.Min(m.triangles[i], m.triangles[i + 1]), Mathf.Max(m.triangles[i], m.triangles[i + 1]));
            Vector2Int eTwo = new Vector2Int(Mathf.Min(m.triangles[i], m.triangles[i + 2]), Mathf.Max(m.triangles[i], m.triangles[i + 2]));
            Vector2Int eThree = new Vector2Int(Mathf.Min(m.triangles[i + 2], m.triangles[i + 1]), Mathf.Max(m.triangles[i + 2], m.triangles[i + 1]));
            //Edges.
            { 
            if (edges.Contains(eOne) == false)
            {
                edges.Add(eOne);
                edge_garb[eOne] = m.triangles[i + 2];
            }
            else
            {
                if(SingleDiff(edge_garb[eOne], m.triangles[i + 2]))
                {
                    edges.Remove(eOne);
                }
            }
            if (edges.Contains(eTwo) == false)
            {
                edges.Add(eTwo);
                edge_garb[eTwo] = m.triangles[i + 1];
            }
            else
            {
                if (SingleDiff(edge_garb[eTwo], m.triangles[i + 1]))
                {
                    edges.Remove(eTwo);
                }
            }
            if (edges.Contains(eThree) == false)
            {
                edges.Add(eThree);
                edge_garb[eThree] = m.triangles[i];
            }
            else
            {
                if (SingleDiff(edge_garb[eThree], m.triangles[i]))
                {
                    edges.Remove(eThree);
                }
            }
            }
            //Faces.
            if(SingleDiff(m.triangles[i], m.triangles[i + 1]))
            {
                if (!faces.ContainsKey(eOne))
                    faces[eOne] = new List<int>();
                if (!faces[eOne].Contains(i))
                    faces[eOne].Add(i);
                if (!faces[eOne].Contains(i + 1))
                    faces[eOne].Add(i + 1);
                if (!faces[eOne].Contains(i + 2))
                    faces[eOne].Add(i + 2);
            }
            else if (SingleDiff(m.triangles[i ], m.triangles[i + 2]))
            {
                if (!faces.ContainsKey(eTwo))
                    faces[eTwo] = new List<int>();
                if (!faces[eTwo].Contains(i))
                    faces[eTwo].Add(i);
                if (!faces[eTwo].Contains(i + 1))
                    faces[eTwo].Add(i + 1);
                if (!faces[eTwo].Contains(i + 2))
                    faces[eTwo].Add(i + 2);
            }
            else if (SingleDiff(m.triangles[i + 2], m.triangles[i + 1]))
            {
                if (!faces.ContainsKey(eThree))
                    faces[eThree] = new List<int>();
                if (!faces[eThree].Contains(i))
                    faces[eThree].Add(i);
                if (!faces[eThree].Contains(i + 1))
                    faces[eThree].Add(i + 1);
                if (!faces[eThree].Contains(i + 2))
                    faces[eThree].Add(i + 2);
            }
        }
        
        
        //This is.
        for (int i = 0; i < 8; i++)
            corners.Add(i);


        for(int i=0;i < corners.Count;i++)
        {
            Instantiate(cube_corn, verts[i], Quaternion.identity);
        }

        for(int i=0;i <edges.Count;i++)
        {
            Vector3 dif = verts[edges[i][0]] - verts[edges[i][1]];
            for (float j = resolution; j < 1 ; j += resolution)
                Instantiate(cube_edge, verts[edges[i][0]] - dif * j, Quaternion.identity);
        }
        foreach(Vector2Int i in faces.Keys)
        {
        }

        /*
        for(int i=0;i < m.triangles.Length;i+=3)
        {
            string eOne = Mathf.Min(m.triangles[i], m.triangles[i + 1])+":"+Mathf.Max(m.triangles[i], m.triangles[i + 1]);
            string eTwo = Mathf.Min(m.triangles[i + 2], m.triangles[i + 1]) + ":" + Mathf.Max(m.triangles[i + 2], m.triangles[i + 1]);
            string eThree = Mathf.Min(m.triangles[i], m.triangles[i + 2]) + ":" + Mathf.Max(m.triangles[i], m.triangles[i + 2]);
            if (!edges_dic.ContainsKey(eOne))
            {
                edges_dic[eOne] = new Vector2Int(Mathf.Min(m.triangles[i], m.triangles[i + 1]), Mathf.Max(m.triangles[i], m.triangles[i + 1]));
                edges.Add(edges_dic[eOne]);
            }
            else
            {
                edges.Remove(edges_dic[eOne]);
            }
            if (!edges_dic.ContainsKey(eTwo))
            {
                edges_dic[eTwo] = new Vector2Int(Mathf.Min(m.triangles[i + 2], m.triangles[i + 1]), Mathf.Max(m.triangles[i + 2], m.triangles[i + 1]));
                edges.Add(edges_dic[eTwo]);
            }
            else
                edges.Remove(edges_dic[eTwo]);
            if (!edges_dic.ContainsKey(eThree))
            {
                edges_dic[eThree] = new Vector2Int(Mathf.Min(m.triangles[i], m.triangles[i + 2]), Mathf.Max(m.triangles[i], m.triangles[i + 2]));
                edges.Add(edges_dic[eThree]);
            }
            else
                edges.Remove(edges_dic[eThree]);
        }

        foreach(Vector2Int e in edges)
        {
            foreach(Vector2Int f in edges)
            {
                if (e == f)
                    continue;
                if (float.IsNaN(SharePoint(e, f)) == false)
                {
                    corners.Add((int)SharePoint(e, f));
                }
            }
        }

        foreach(int i in corners)
        {
            foreach(int j in corners)
            {
                if (i == j)
                    continue;
               if(float.IsNaN(SingleDiff(verts[i], verts[j])) == false)
                {
                    if(!faces.ContainsKey(SingleDiff(verts[i],verts[j])))
                    {
                        faces[SingleDiff(verts[i], verts[j])] = new List<int>();
                    }
                    if (faces[SingleDiff(verts[i], verts[j])].Contains(i) == false)
                        faces[SingleDiff(verts[i], verts[j])].Add(i);
                    if (faces[SingleDiff(verts[i], verts[j])].Contains(j) == false)
                        faces[SingleDiff(verts[i], verts[j])].Add(j);
                }
            }
        }
        foreach (int i in corners)
            print(verts[i]);
        print("\n");
        foreach(Vector2Int i in edges)
        {
            print(verts[i[0]] + " " + verts[i[1]]);
        }
        print("\n");*/
    }
    private float SharePoint(Vector2Int a, Vector2Int b)
    {
        if (a[0] == b[0] || a[0] == b[1])
            return a[0];
        if (a[1] == b[0] || a[1] == b[1])
            return a[1];
        return float.NaN;
    }
    
    private bool SingleDiff(int x, int y)
    {
        Vector3 test = verts[x] - verts[y];
        if (test[0] == 0 && test[1] != 0 && test[2] != 0)
            return true;
        if (test[0] != 0 && test[1] != 0 && test[2] == 0)
            return true;
        if (test[0] != 0 && test[1] == 0 && test[2] != 0)
            return true;
        return false;
    }
}
