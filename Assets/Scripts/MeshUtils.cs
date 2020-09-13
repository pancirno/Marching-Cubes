using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MeshUtils
{
    static public void WeldVertices(List<Vector3> verts, List<int> indices)
    {
        var distinctVerts = verts.Distinct().ToArray();
        Dictionary<Vector3, int> verticeDict = new Dictionary<Vector3, int>();

        for(int vertId = 0; vertId < distinctVerts.Length; vertId++)
        {
            verticeDict[distinctVerts[vertId]] = vertId;
        }

        Parallel.For(0, indices.Count, (i) =>
        {
            Vector3 vert = verts[indices[i]];
            indices[i]   = verticeDict[vert];
        });

        verts.Clear();
        verts.Capacity = distinctVerts.Length;
        verts.AddRange(distinctVerts);
    }
}
