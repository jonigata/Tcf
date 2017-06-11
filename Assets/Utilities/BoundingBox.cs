using UnityEngine;
using System;

public class BoundingBox {
    public Vector3 min;
    public Vector3 max;
    public Vector3 w;

    public BoundingBox(Vector3[] vertices) {
        var mv = Single.MaxValue;
        min = new Vector3(mv, mv, mv);
        max = new Vector3(-mv, -mv, -mv);

        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 v = vertices[i];
            if (v.x < min.x) { min.x = v.x; }
            if (v.y < min.y) { min.y = v.y; }
            if (v.z < min.z) { min.z = v.z; }
            if (max.x < v.x) { max.x = v.x; }
            if (max.y < v.y) { max.y = v.y; }
            if (max.z < v.z) { max.z = v.z; }
        }

        w = max - min;
    }

    public Vector3 CalculateNormalizedPosition(Vector3 v) {
        Vector3 np = new Vector3(
            (v.x - min.x) / w.x,
            (v.y - min.y) / w.y,
            (v.z - min.z) / w.z);
        return np;
    }

}
