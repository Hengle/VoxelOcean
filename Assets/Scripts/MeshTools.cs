﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for working with meshes.
/// </summary>
public static class MeshTools
{
    /// <summary>
    /// Removes duplicate vertices from a mesh by "welding" together vertices that are EXACTLY the same.
    /// </summary>
    /// <param name="complexMesh">The mesh from which to remove duplicate vertices.</param>
    /// <param name="smoothMesh">Whether or not to smooth out the normals of the new mesh.</param>
    /// <param name="printDebug">Whether or not to output to the console the before/after vertex count.</param>
    /// <returns>A copy of the complexMesh, with vertices removed. NOTE: The copy contains no UVs, vertex colors, or normals.</returns>
    static public Mesh RemoveDuplicates(Mesh complexMesh, bool smoothMesh = true, bool printDebug = false)
    {
        List<Vector3> verts = new List<Vector3>();
        int[] tris = new int[complexMesh.triangles.Length];
        
        Vector3[] oldVerts = complexMesh.vertices; // Calling complexMesh.vertices makes a copy of vertices! Only call this once!

        // this holds remapped index numbers / the key is the old index, the value is the new index
        Dictionary<int, int> remappedVerts = new Dictionary<int, int>();

        int removedCount = 0;
        for (int i = 0; i < oldVerts.Length; i++)
        {
            if (remappedVerts.ContainsKey(i)) // this vertex is a duplicate and has already been "remapped" onto a vertex to its left
            {
                removedCount++; // count this as a vertex that will be removed
                continue;
            }
            verts.Add(oldVerts[i]); // copy vert into new list (this should only run if the vert wasn't a dupe)
            remappedVerts.Add(i, i - removedCount); // remap its index number by shifting it left the number vertices that have been removed

            for (int j = i + 1; j < oldVerts.Length; j++) // find duplicates of this vertex to the right:
            {
                if (remappedVerts.ContainsKey(j)) continue; // once this vert has been remapped, ignore it
                if (oldVerts[i] == oldVerts[j]) remappedVerts.Add(j, verts.Count - 1);
            }
        }
        for (int k = 0; k < complexMesh.triangles.Length; k++) // change indices in tris list:
        {
            if (remappedVerts.ContainsKey(k)) tris[k] = remappedVerts[k];
            else Debug.Log($"index {k} doesn't exist in remappedVerts");
        }
        if (printDebug) Debug.Log($"{oldVerts.Length} reduced to {verts.Count}");
        
        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        if(smoothMesh) mesh.RecalculateNormals();
        return mesh;

    }
    /// <summary>
    /// This function generates and returns a 1m cube mesh. The anchor point is at the bottom of the mesh. This mesh has NO duplicate vertices. So shading will look a little odd due to unrealistic normals.
    /// </summary>
    /// <returns>A mesh with normals set. There are no UVs and no vertex colors on this mesh.</returns>
    public static Mesh MakeSmoothCube()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();

        verts.Add(new Vector3(-0.5f, 0, -0.5f));
        verts.Add(new Vector3(+0.5f, 0, -0.5f));
        verts.Add(new Vector3(+0.5f, 0, +0.5f));
        verts.Add(new Vector3(-0.5f, 0, +0.5f));

        verts.Add(new Vector3(-0.5f, 1, -0.5f));
        verts.Add(new Vector3(+0.5f, 1, -0.5f));
        verts.Add(new Vector3(+0.5f, 1, +0.5f));
        verts.Add(new Vector3(-0.5f, 1, +0.5f));

        tris.Add(0);
        tris.Add(1);
        tris.Add(2);

        tris.Add(0);
        tris.Add(2);
        tris.Add(3);

        tris.Add(1);
        tris.Add(6);
        tris.Add(2);

        tris.Add(1);
        tris.Add(5);
        tris.Add(6);

        tris.Add(0);
        tris.Add(5);
        tris.Add(1);

        tris.Add(0);
        tris.Add(4);
        tris.Add(5);


        tris.Add(3);
        tris.Add(4);
        tris.Add(0);

        tris.Add(3);
        tris.Add(7);
        tris.Add(4);

        tris.Add(7);
        tris.Add(5);
        tris.Add(4);

        tris.Add(7);
        tris.Add(6);
        tris.Add(5);

        tris.Add(2);
        tris.Add(7);
        tris.Add(3);

        tris.Add(2);
        tris.Add(6);
        tris.Add(7);

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        return mesh;
    }
    /// <summary>
    /// This function generates and returns a 1m cube mesh. The anchor point is at the bottom of the mesh.
    /// </summary>
    /// <returns>A mesh object with normals and uvs. No color information has been set.</returns>
    public static Mesh MakeCube()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();

        // Front face
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

        // Back face
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

        // Left face 
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

        // Right face
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

        // Top face
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

        // Bottom face 
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

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        return mesh;
    }

    /// <summary>
    /// Makes a mesh for a 1m tall (y-axis) pentagonal cylinder. Currently has all data except for UVs.
    /// </summary>
    /// <returns>Mesh data for a 1m tall (y-axis) cylinder.</returns>
    public static Mesh MakePentagonalCylinder()
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

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        //mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        return mesh;
    }

    /// <summary>
    /// This function generates and returns a 1m cube mesh that tapers at its end. The anchor point is the back center of the mesh.
    /// </summary>
    /// <returns>A mesh object with normals and uvs. No color information has been set.</returns>
    public static Mesh MakeTaperedCube()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();

        // Front face
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

        // Back face
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

        // Left face 
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

        // Right face
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

        // Top face
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

        // Bottom face 
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

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        return mesh;
    }
}
