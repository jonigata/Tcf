using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Partix {

[Serializable]
public struct Tetrahedron {
    public int i0;
    public int i1;
    public int i2;
    public int i3;
}

[Serializable]
public struct Triangle {
    public int i0;
    public int i1;
    public int i2;
}

public class Volume : ScriptableObject {
    public Vector3[] vertices;
    public Tetrahedron[] tetrahedra;
    public Triangle[] faces;

    public void DumpStats(string objectName) {
        Debug.LogFormat("{3}: vertices = {0} tetrahedra = {1} faces = {2}",
                        vertices.Length,
                        tetrahedra.Length,
                        faces.Length,
                        objectName);
    }
}

}
