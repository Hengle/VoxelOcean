﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This script creates a mesh for a PlantKelp object using mesh construction functions.
/// Author: Kyle Lowery
/// Last Date Updated: 03/29/2019
/// </summary>
public class PlantKelpMesh : MonoBehaviour
{
    // Ints:
    /// <summary>
    /// Controls the number of iterations the Build function goes through to make the mesh.
    /// </summary>
    [Range(2, 20)] public int iterations = 3;
    /// <summary>
    /// Stores the number of leaves attached to every segment of the mesh.
    /// </summary>
    [Range(2, 10)] public int numberOfLeaves = 5;

    // Scaling Vectors:
    /// <summary>
    /// Stores the scaling of each Stem segment in the mesh.
    /// </summary>
    public Vector3 stemScaling = new Vector3(.25f, 1, .25f);
    /// <summary>
    /// Stores the scaling of each Leaf segment in the mesh.
    /// </summary>
    public Vector3 leafScaling = new Vector3(.25f, 1, .35f);

    // Angle of Instance:
    /// <summary>
    /// Controls the angle of the stem mesh in the X direction.
    /// </summary>
    [Range(0, 45f)] public float angleX = 0;
    /// <summary>
    /// Controls the angle of the stem mesh in the Y direction.
    /// </summary>
    [Range(0, 45f)] public float angleY = 45f;
    /// <summary>
    /// Controls the angle of the stem mesh in the Z direction.
    /// </summary>
    [Range(0, 45f)] public float angleZ = 45f;

    // Offset Angle for leaves:
    /// <summary>
    /// Controls the angle of the leaf mesh in the X direction.
    /// </summary>
    public float leafOffsetAngleX = 90f;
    /// <summary>
    /// Controls the angle of the leaf mesh in the Y direction.
    /// </summary>
    public float leafOffsetAngleY = 90f;
    /// <summary>
    /// Controls the angle of the leaf mesh in the Z direction.
    /// </summary>
    public float leafOffsetAngleZ = 90f;

    // Modifiers for a randomized range of value:
    /// <summary>
    /// Sets the minimum value of a random number.
    /// </summary>
    [Range(0, 45f)] public float minRandom = 0;
    /// <summary>
    /// Sets the maximum value of a random number.
    /// </summary>
    [Range(0, 45f)] public float maxRandom = 30f;

    //Hue modifiers:
    /// <summary>
    /// Sets the minimum value of the hue color.
    /// </summary>
    [Range(.1f, .9f)] public float hueMin = .1f;
    /// <summary>
    /// Sets the maximum value of the hue color.
    /// </summary>
    [Range(.1f, 1f)] public float hueMax = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Check to make sure Unity values don't break the code:
        if (minRandom > maxRandom || minRandom == maxRandom) minRandom = maxRandom - 1;
        if (hueMin > hueMax || hueMin == hueMax) hueMin = hueMax - .1f;

        // Start building the mesh:
        Build();
    }

    /// <summary>
    /// Builds the Mesh for PlantKelp out of coded-in meshes.
    /// </summary>
    public void Build()
    {
        List<CombineInstance> meshes = new List<CombineInstance>();
        Grow(iterations, meshes, Vector3.zero, Quaternion.identity);

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(meshes.ToArray());

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
    /// <summary>
    /// Pieces together a mesh based on a set position, rotation, and iterations.
    /// </summary>
    /// <param name="num">The number of iterations that have passed already.</param>
    /// <param name="meshes">Stores all the meshes this function constructs.</param>
    /// <param name="pos">The position of the endPoint of the previous stem segment/spawner location.</param>
    /// <param name="rot">The rotation of the previous stem segment/spawner location</param>
    private void Grow(int num, List<CombineInstance> meshes, Vector3 pos, Quaternion rot)
    {
        if (num <= 0) return;   //stop recursive function

        // Add Stem Mesh:
        CombineInstance stem = new CombineInstance();
        stem.mesh = MakePentagonalCylinder(num);
        stem.transform = Matrix4x4.TRS(pos, rot, stemScaling);
        meshes.Add(stem);

        // Add Leaves Mesh:
        for (int i = 1; i <= numberOfLeaves; i++)
        {
            CombineInstance leaves = new CombineInstance();
            leaves.mesh = MakeCube(num);
            float rotAmount = 360 / (numberOfLeaves * i);
            Quaternion leafRot = rot * Quaternion.Euler(leafOffsetAngleX, leafOffsetAngleY * rotAmount, leafOffsetAngleZ);
            leaves.transform = Matrix4x4.TRS(pos, leafRot, leafScaling);
            meshes.Add(leaves);
        }

        // Count down number of passes:
        num--;

        // Get the position of the end of this section:
        pos = stem.transform.MultiplyPoint(new Vector3(0, 1, 0));
        
        //Generate random angles based on set angle values:
        float randX = angleX + Random.Range(minRandom, maxRandom);
        float randY = angleY + Random.Range(minRandom, maxRandom);
        float randZ = angleZ + Random.Range(minRandom, maxRandom);

        //Create a Quaternion to change the local transform to the world Vector3.up:
        Quaternion rot1 = Quaternion.FromToRotation(transform.up, Vector3.up);
        //Create a Quaternion using the random angles from earlier:
        Quaternion rot2 = Quaternion.Euler(randX, randY, randZ);

        //Use a slerp to create a Quaternion between rot1 and rot2, leaning more towards randomness in later segments:
        Quaternion finalRot = Quaternion.Slerp(rot1, rot2, (num/(float)iterations));
        
        //Grow another segment:
        Grow(num, meshes, pos, finalRot);
    }
    /// <summary>
    /// Makes a mesh for a 1m-sized cube.
    /// </summary>
    /// <returns>Mesh data for a 1m-sized cube.</returns>
    private Mesh MakeCube(int num)
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> colors = new List<Color>();

        //FRONT
        verts.Add(new Vector3(-0.5f, 0, -0.5f));
        verts.Add(new Vector3(-0.5f, 1, -0.5f));
        verts.Add(new Vector3(+0.5f, 1, -0.5f));
        verts.Add(new Vector3(+0.5f, 0, -0.5f));
        normals.Add(new Vector3(0, 0, -1));
        normals.Add(new Vector3(0, 0, -1));
        normals.Add(new Vector3(0, 0, -1));
        normals.Add(new Vector3(0, 0, -1));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        tris.Add(0);
        tris.Add(1);
        tris.Add(2);
        tris.Add(2);
        tris.Add(3);
        tris.Add(0);

        //BACK
        verts.Add(new Vector3(-0.5f, 0, +0.5f));
        verts.Add(new Vector3(+0.5f, 0, +0.5f));
        verts.Add(new Vector3(+0.5f, 1, +0.5f));
        verts.Add(new Vector3(-0.5f, 1, +0.5f));
        normals.Add(new Vector3(0, 0, +1));
        normals.Add(new Vector3(0, 0, +1));
        normals.Add(new Vector3(0, 0, +1));
        normals.Add(new Vector3(0, 0, +1));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        tris.Add(4);
        tris.Add(5);
        tris.Add(6);
        tris.Add(6);
        tris.Add(7);
        tris.Add(4);

        //LEFT
        verts.Add(new Vector3(-0.5f, 0, -0.5f));
        verts.Add(new Vector3(-0.5f, 0, +0.5f));
        verts.Add(new Vector3(-0.5f, 1, +0.5f));
        verts.Add(new Vector3(-0.5f, 1, -0.5f));
        normals.Add(new Vector3(-1, 0, 0));
        normals.Add(new Vector3(-1, 0, 0));
        normals.Add(new Vector3(-1, 0, 0));
        normals.Add(new Vector3(-1, 0, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        tris.Add(8);
        tris.Add(9);
        tris.Add(10);
        tris.Add(10);
        tris.Add(11);
        tris.Add(8);

        //RIGHT
        verts.Add(new Vector3(+0.5f, 0, -0.5f));
        verts.Add(new Vector3(+0.5f, 1, -0.5f));
        verts.Add(new Vector3(+0.5f, 1, +0.5f));
        verts.Add(new Vector3(+0.5f, 0, +0.5f));
        normals.Add(new Vector3(+1, 0, 0));
        normals.Add(new Vector3(+1, 0, 0));
        normals.Add(new Vector3(+1, 0, 0));
        normals.Add(new Vector3(+1, 0, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        tris.Add(12);
        tris.Add(13);
        tris.Add(14);
        tris.Add(14);
        tris.Add(15);
        tris.Add(12);

        //TOP
        verts.Add(new Vector3(-0.5f, 1, -0.5f));
        verts.Add(new Vector3(-0.5f, 1, +0.5f));
        verts.Add(new Vector3(+0.5f, 1, +0.5f));
        verts.Add(new Vector3(+0.5f, 1, -0.5f));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        tris.Add(16);
        tris.Add(17);
        tris.Add(18);
        tris.Add(18);
        tris.Add(19);
        tris.Add(16);

        //BOTTOM
        verts.Add(new Vector3(-0.5f, 0, -0.5f));
        verts.Add(new Vector3(+0.5f, 0, -0.5f));
        verts.Add(new Vector3(+0.5f, 0, +0.5f));
        verts.Add(new Vector3(-0.5f, 0, +0.5f));
        normals.Add(new Vector3(0, -1, 0));
        normals.Add(new Vector3(0, -1, 0));
        normals.Add(new Vector3(0, -1, 0));
        normals.Add(new Vector3(0, -1, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        tris.Add(20);
        tris.Add(21);
        tris.Add(22);
        tris.Add(22);
        tris.Add(23);
        tris.Add(20);

        float hue = Mathf.Lerp(hueMin, hueMax, (num/(float)iterations));

        foreach (Vector3 pos in verts)
        {
            float tempHue = hue;
            Color tempColor = Color.HSVToRGB(tempHue, 1, 1);
            colors.Add(tempColor);
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        mesh.SetColors(colors);
        return mesh;

    }
    /// <summary>
    /// Makes a mesh for a 1m tall (y-axis) pentagonal cylinder.
    /// </summary>
    /// <returns>Mesh data for a 1m tall (y-axis) cylinder.</returns>
    private Mesh MakePentagonalCylinder(int num)
    {
        List<Vector3> verts = new List<Vector3>();
        //List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> colors = new List<Color>();

        //TOP
        verts.Add(new Vector3(+.5f, 1, 0));
        verts.Add(new Vector3(0, 1, -.5f));
        verts.Add(new Vector3(-.5f, 1, -.25f));
        verts.Add(new Vector3(-.5f, 1, +.25f));
        verts.Add(new Vector3(0, 1, +.5f));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        //uvs?
        tris.Add(0);
        tris.Add(1);
        tris.Add(2);
        tris.Add(0);
        tris.Add(2);
        tris.Add(3);
        tris.Add(0);
        tris.Add(3);
        tris.Add(4);

        //BOTTOM
        verts.Add(new Vector3(+.5f, 0, 0));
        verts.Add(new Vector3(0, 0, -.5f));
        verts.Add(new Vector3(-.5f, 0, -.25f));
        verts.Add(new Vector3(-.5f, 0, +.25f));
        verts.Add(new Vector3(0, 0, +.5f));
        normals.Add(new Vector3(0, -1, 0));
        normals.Add(new Vector3(0, -1, 0));
        normals.Add(new Vector3(0, -1, 0));
        normals.Add(new Vector3(0, -1, 0));
        normals.Add(new Vector3(0, -1, 0));
        //uvs?
        tris.Add(7);
        tris.Add(6);
        tris.Add(5);
        tris.Add(8);
        tris.Add(7);
        tris.Add(5);
        tris.Add(9);
        tris.Add(8);
        tris.Add(5);

        //FRONT
        verts.Add(new Vector3(-.5f, 0, -.25f));
        verts.Add(new Vector3(-.5f, 0, +.25f));
        verts.Add(new Vector3(-.5f, 1, -.25f));
        verts.Add(new Vector3(-.5f, 1, +.25f));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        //uvs?
        tris.Add(10);
        tris.Add(11);
        tris.Add(12);
        tris.Add(11);
        tris.Add(13);
        tris.Add(12);

        //LEFT-FRONT
        verts.Add(new Vector3(0, 0, -.5f));
        verts.Add(new Vector3(-.5f, 0, -.25f));
        verts.Add(new Vector3(0, 1, -.5f));
        verts.Add(new Vector3(-.5f, 1, -.25f));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        //uvs?
        tris.Add(14);
        tris.Add(15);
        tris.Add(16);
        tris.Add(15);
        tris.Add(17);
        tris.Add(16);

        //RIGHT-FRONT
        verts.Add(new Vector3(-.5f, 0, +.25f));
        verts.Add(new Vector3(0, 0, +.5f));
        verts.Add(new Vector3(-.5f, 1, +.25f));
        verts.Add(new Vector3(0, 1, +.5f));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        normals.Add(new Vector3(0, 1, 0));
        //uvs?
        tris.Add(18);
        tris.Add(19);
        tris.Add(20);
        tris.Add(19);
        tris.Add(21);
        tris.Add(20);

        //LEFT-BACK
        verts.Add(new Vector3(0, 0, -.5f));
        verts.Add(new Vector3(+.5f, 0, 0));
        verts.Add(new Vector3(0, 1, -.5f));
        verts.Add(new Vector3(+.5f, 1, 0));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        //uvs?
        tris.Add(24);
        tris.Add(23);
        tris.Add(22);
        tris.Add(24);
        tris.Add(25);
        tris.Add(23);

        //RIGHT-BACK
        verts.Add(new Vector3(+.5f, 0, 0));
        verts.Add(new Vector3(0, 0, +.5f));
        verts.Add(new Vector3(+.5f, 1, 0));
        verts.Add(new Vector3(0, 1, +.5f));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        normals.Add(new Vector3(0, +1, 0));
        //uvs?
        tris.Add(28);
        tris.Add(29);
        tris.Add(26);
        tris.Add(26);
        tris.Add(29);
        tris.Add(27);

        float hue = Mathf.Lerp(hueMin, hueMax, (num / (float)iterations));

        foreach (Vector3 pos in verts)
        {
            float tempHue = hue;
            Color tempColor = Color.HSVToRGB(tempHue, 1, 1);
            colors.Add(tempColor);
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        //mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        mesh.SetColors(colors);
        return mesh;
    }
}

[CustomEditor(typeof(PlantKelpMesh))]
public class PlantKelpMeshEditor : Editor
{
    /// <summary>
    /// Runs the base OnInspectorGUI command and adds a "Grow" button to the inspector in the GUI.
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("GROW!"))                              //if the "Grow" button is pressed...
        {
            PlantKelpMesh k = (target as PlantKelpMesh);                //create a new PlantKelpMesh on the script target.
            k.Build();                                                  //Call the new PlantKelpMesh's Build function.
        }
    }
}
