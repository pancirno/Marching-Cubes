using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MeshUtils
{
    static public List<Vector3> WeldVertices(List<Vector3> verts, List<int> indicesToUpdate)
    {
        var distinctVerts = verts.Distinct().ToList();
        Dictionary<Vector3, int> verticeDict = new Dictionary<Vector3, int>();

        for(int vertId = 0; vertId < distinctVerts.Count; vertId++)
        {
            verticeDict[distinctVerts[vertId]] = vertId;
        }

        for(int i = 0; i < indicesToUpdate.Count; i++)
        {
            Vector3 vert = verts[indicesToUpdate[i]];
            indicesToUpdate[i] = verticeDict[vert];
        }

        return distinctVerts;
    }
}
