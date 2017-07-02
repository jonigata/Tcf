using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace Partix {

[ExecuteInEditMode]
public class World : MonoBehaviour {
    public float deltaTime = 0.016f;

    IntPtr nativeWorld = IntPtr.Zero;

    Dictionary<IntPtr, Body> bodies;

    IEnumerator Start() {
        Debug.Log("World.Start");
        if (!Application.isPlaying) {
            Debug.Log("Enable EDITOR");
            Shader.EnableKeyword("EDITOR_MODE");
            Shader.DisableKeyword("PLAY_MODE");
            yield break;
        } else {
            Debug.Log("Disable EDITOR");
            Shader.DisableKeyword("EDITOR_MODE");
            Shader.EnableKeyword("PLAY_MODE");
        }

        yield return new WaitUntil(() => PartixDll.initialized);

        nativeWorld = PartixDll.CreateWorld();
        PartixDll.SetGravity(nativeWorld, new Vector3(0, -9.8f, 0));
        bodies = new Dictionary<IntPtr, Body>();
    }

    public bool Ready() {
        return nativeWorld != IntPtr.Zero;
    }

    void OnDestroy() {
        Debug.Log("World.Destroy");
        if (nativeWorld == IntPtr.Zero) { return; }
        PartixDll.DestroyWorld(nativeWorld);
        nativeWorld = IntPtr.Zero;
    }

    void Update() {
        if (nativeWorld == IntPtr.Zero) { return; }
        PartixDll.UpdateWorld(nativeWorld, deltaTime);
    }

    public IntPtr CreateSoftVolume(
        Body b,
        string tcf, Vector3 p, float scale, float mass) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        IntPtr e = PartixDll.CreateSoftVolume(
            nativeWorld, tcf, p, scale, mass);
        bodies[e] = b;
        return e;
    }

    public IntPtr CreateSoftVolume2(
        Body b,
        int vertexCount, Vector3[] vertices,
        int tetrahedronCount, Tetrahedron[] tetrahedra,
        int faceCount, Triangle[] faces,
        Vector3 p, float scale, float mass) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        IntPtr e = PartixDll.CreateSoftVolume2(
            nativeWorld,
            vertexCount, vertices,
            tetrahedronCount, tetrahedra,
            faceCount, faces,
            p, scale, mass);
        bodies[e] = b;
        return e;
    }

    public IntPtr CreateVehicle(
        Body b,
        string tcf, Vector3 p, float scale, float mass) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        IntPtr e = PartixDll.CreateVehicle(
            nativeWorld, tcf, p, scale, mass);
        bodies[e] = b;
        return e;
    }

    public IntPtr CreateVehicle2(
        Body b,
        int vertexCount, Vector3[] vertices,
        int tetrahedronCount, Tetrahedron[] tetrahedra,
        int faceCount, Triangle[] faces,
        Vector3 p, float scale, float mass) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        IntPtr e = PartixDll.CreateVehicle2(
            nativeWorld,
            vertexCount, vertices,
            tetrahedronCount, tetrahedra,
            faceCount, faces,
            p, scale, mass);
        bodies[e] = b;
        return e;
    }

    public IntPtr CreateSoftShell(
        Body b,
        int vertex_count, [In] Vector3[] vertices,
        int triangle_count, [In] int[] triangles,
        int threshold, Vector3 location, float scale, float mass) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        IntPtr e = PartixDll.CreateSoftShell(
            nativeWorld, 
            vertex_count, vertices,
            triangle_count, triangles, 
            threshold, location, scale, mass);
        bodies[e] = b;
        return e;
    }

    public IntPtr CreatePlane(Vector3 p, Vector3 n) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        return PartixDll.CreatePlane(nativeWorld, p, n);
    }

    public Vector3 GetPosition(IntPtr body) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        Vector3 v;
        PartixDll.GetPosition(nativeWorld, body, out v);
        return v;
    }

    public Matrix4x4 GetOrientation(IntPtr body) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        Matrix4x4 m;
        PartixDll.GetOrientation(nativeWorld, body, out m);
        return m;
    }

    public Vector3[] GetWireFrameVertices(IntPtr body) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        int c = PartixDll.GetWireFrameVertexCount(nativeWorld, body);
        Vector3[] a = new Vector3[c];
        PartixDll.GetWireFrameVertices(nativeWorld, body, a);
        return a;
    }

    public int[] GetWireFrameIndices(IntPtr body) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        int c = PartixDll.GetWireFrameIndexCount(nativeWorld, body);
        int[] a = new int[c];
        PartixDll.GetWireFrameIndices(nativeWorld, body, a);
        return a;
    }

    public VehiclePointLoad[] GetPointLoads(IntPtr body) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        int c = PartixDll.GetWireFrameVertexCount(nativeWorld, body);
        VehiclePointLoad[] a = new VehiclePointLoad[c];
        PartixDll.GetPointLoads(nativeWorld, body, a);
        return a;
    }

    public void SetPointLoads(IntPtr body, VehiclePointLoad[] loads) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        PartixDll.SetPointLoads(nativeWorld, body, loads);
    }

    public void AnalyzeVehicle(
        IntPtr              b,
        ref VehicleParameter vehicleParameter,
        Vector3             prev_velocity,
        Matrix4x4           prev_orientaion,
        Matrix4x4           curr_orientaion,
        float               sensory_balance,
        ref VehicleAnalyzeData ad) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        PartixDll.AnalyzeVehicle(
            nativeWorld,
            b,
            ref vehicleParameter,
            deltaTime,
            prev_velocity,
            prev_orientaion,
            curr_orientaion,
            sensory_balance,
            ref ad);
    }

    public void AccelerateVehicle(
        IntPtr              b,
        ref VehicleParameter vehicleParameter,
        Vector3             accel) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        PartixDll.AccelerateVehicle(
            nativeWorld,
            b,
            ref vehicleParameter,
            accel);
    }

    public void RotateEntity(IntPtr b, Quaternion q) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        PartixDll.RotateEntity(
            nativeWorld,
            b,
            q.w, q.x, q.y, q.z);
    }

    public void RotateEntity(IntPtr b, Quaternion q, Vector3 pivot) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        PartixDll.RotateEntityWithPivot(
            nativeWorld,
            b,
            q.w, q.x, q.y, q.z, pivot);
    }

    public void FixEntity(IntPtr b, Vector3 origin) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        PartixDll.FixEntity(nativeWorld, b, origin);
    }

    public void AddForce(IntPtr b, Vector3 v) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        PartixDll.AddForce(nativeWorld, b, v);
    }

    public void SetEntityFeatures(
        IntPtr b, EntityFeatures ef) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        PartixDll.SetEntityFeatures(nativeWorld, b, ef);
    }

    public Body[] GetContacts(IntPtr b) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        int n = PartixDll.GetContactCount(nativeWorld, b);
        IntPtr[] a = new IntPtr[n];
        PartixDll.GetContacts(nativeWorld, b, a);

        Body[] aa = new Body[n];
        for (int i = 0 ; i < n ; i++) {
            aa[i] = bodies[a[i]];
        }
        return aa;
    }

    public ClassId GetClassId(IntPtr b) {
        Assert.IsTrue(nativeWorld != IntPtr.Zero);
        return PartixDll.GetClassId(nativeWorld, b);
    }

    public void BlendPosition(IntPtr b, Matrix4x4 m, float n, float dn) {
        PartixDll.BlendPosition(nativeWorld, b, m, n, dn);
    }

}

}
