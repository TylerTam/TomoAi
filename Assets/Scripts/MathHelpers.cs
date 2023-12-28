using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MH
{
    public static Vector3 VectorOnYPlane(Vector3 pos, float yCord)
    {
        return new Vector3(pos.x, yCord, pos.z);
    }
    public static Vector3 FlattenVector(Vector3 pos)
    {
        return new Vector3(pos.x, 0, pos.z);
    }
}
