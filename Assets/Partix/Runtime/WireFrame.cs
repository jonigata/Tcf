using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Partix {

[ExecuteInEditMode]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
public class WireFrame : MonoBehaviour {
    public bool renderWeight;
    public bool renderAccel;
    public bool renderFriction;
    public SoftVolume softVolume;

    World world;
    MeshFilter meshFilter;
    Mesh mesh;
    float usedScale = 1.0f;
    Volume usedVolume;

    Color[] colors;

    void Awake() {
        world = FindObjectOfType<World>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
    }

    void Update() {
        if (!Application.isPlaying) {
            if (usedVolume == softVolume.volume &&
                usedScale == softVolume.partixScale) { return; }
            usedVolume = softVolume.volume;
            usedScale = softVolume.partixScale;
            if (usedVolume != null) {
                SetUpFromStaticData();
            }
        } else {
            if (!softVolume.Ready()) { return; }
        
            int[] indices = softVolume.GetWireFrameIndices();
            mesh.vertices = softVolume.GetWireFrameVertices();
            mesh.SetIndices(indices, MeshTopology.Lines, 0);

            VehiclePointLoad[] vpl = softVolume.GetPointLoads();
            colors = new Color[vpl.Length];
            for (int i = 0 ; i < colors.Length ; i++) {
                var c = new Color(0, 0, 0);
                if (renderWeight) c.r = vpl[i].weight;
                if (renderFriction) c.g = vpl[i].friction;
                if (renderAccel) c.b = vpl[i].accel;
                colors[i] = c;
            }
            mesh.colors = colors;
        }
    }

    void SetUpFromStaticData() {
        mesh.SetIndices(null, MeshTopology.Lines, 0);

        Vector3[] vertices = new Vector3[usedVolume.vertices.Length];
        for (int j = 0 ; j < usedVolume.vertices.Length ; j++) {
            vertices[j] = usedVolume.vertices[j] * softVolume.partixScale;
        }
        mesh.vertices = vertices;

        int[] indices = new int[usedVolume.faces.Length * 6];
        int i = 0;
        foreach (Triangle t in usedVolume.faces) {
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
