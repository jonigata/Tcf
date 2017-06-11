using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Partix {

[ExecuteInEditMode]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
public class VolumeWireFrame : MonoBehaviour {
    public Material material;
    public Volume volume; 

    Volume volume_;

    MeshFilter meshFilter;
    Mesh mesh;

    void Awake() {
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start() {
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
        GetComponent<MeshRenderer>().material = material;

        SetUp();
    }

    void Update() {
        if (volume_ == volume) { return; }
        volume_ = volume;
        if (volume_ != null) {
            SetUp();
        }
    }

    void SetUp() {
        mesh.vertices = volume.vertices;

        int[] indices = new int[volume.faces.Length * 6];
        int i = 0;
        foreach (Triangle t in volume.faces) {
            indices[i++] = t.i0;
            indices[i++] = t.i1;
            indices[i++] = t.i1;
            indices[i++] = t.i2;
            indices[i++] = t.i2;
            indices[i++] = t.i0;
        }
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
    }

}

}
