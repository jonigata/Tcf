using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Partix {

public class SoftShell : Body {
    World world;
    IntPtr nativePartixSoftShell = IntPtr.Zero;

    void Awake() {
        world = FindObjectOfType<World>();
    }

    IEnumerator Start() {
        yield return new WaitUntil(() => world.Ready());

        var meshFilter = GetComponent<MeshFilter>();
        var mesh = meshFilter.sharedMesh;
        

        Vector3[] vertices = mesh.vertices;
        int vertex_count = mesh.vertices.Length;
        int[] triangles = mesh.triangles;
        int triangle_count = mesh.triangles.Length / 3;

        Stopwatch sw = new Stopwatch();
        sw.Start();
        nativePartixSoftShell = world.CreateSoftShell(
            this,
            vertex_count, vertices,
            triangle_count, triangles,
            32768, transform.position, transform.lossyScale.x, 1.0f);
        sw.Stop();
        UnityEngine.Debug.Log(sw.ElapsedMilliseconds+"ms");
    }


    // Update is called once per frame
    void Update () {
		
    }

}

}
