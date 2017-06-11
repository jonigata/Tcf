using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace Partix {

public class SoftVolume : Body {
    public MeshRenderer[] meshRenderers;
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public Transform controlTransform;
    public Volume volume;
    public bool vehicle;
    public VehicleParameter vehicleParameter;
    public VehicleAnalyzeData vehicleAnalyzeData;
    public float partixScale = 1.0f;
    public float pointMass = 1.0f;

    public Matrix4x4 forDebug;

    World world;
    IntPtr nativePartixSoftVolume = IntPtr.Zero;
    public Matrix4x4 prevOrientation;
    public Matrix4x4 currOrientation;
    float sensoryBalance = 0;
    Vector3 sourcePosition;

    void Awake() {
        world = FindObjectOfType<World>();
        sourcePosition = controlTransform.position;
    }

    IEnumerator Start() {
        Debug.Log("A");
        yield return new WaitUntil(() => world.Ready());

        Debug.Log("B");
        volume.DumpStats(gameObject.name);
        if (vehicle) {
            nativePartixSoftVolume = world.CreateVehicle2(
                this,
                volume.vertices.Length,
                volume.vertices,
                volume.tetrahedra.Length,
                volume.tetrahedra,
                volume.faces.Length,
                volume.faces,
                controlTransform.position, 
                partixScale,
                pointMass);
        } else {
            nativePartixSoftVolume = world.CreateSoftVolume2(
                this,
                volume.vertices.Length,
                volume.vertices,
                volume.tetrahedra.Length,
                volume.tetrahedra,
                volume.faces.Length,
                volume.faces,
                controlTransform.position, 
                partixScale,
                pointMass);
        }
    }

    void Update() {
        if (nativePartixSoftVolume == IntPtr.Zero) { return; }

        Matrix4x4 m = world.GetOrientation(nativePartixSoftVolume);
        controlTransform.position = new Vector3(m[0,3], m[1,3], m[2,3]);

        prevOrientation = currOrientation;
        currOrientation = m;
        m[0,3] = 0;
        m[1,3] = 0;
        m[2,3] = 0;
        m[3,0] = 0;
        m[3,1] = 0;
        m[3,2] = 0;
        AttachDeform(m);


        if (vehicle) {
            AnalyzeVehicle();
        }
    }

    public bool Ready() { return nativePartixSoftVolume != IntPtr.Zero; }

    public Vector3[] GetWireFrameVertices() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        return world.GetWireFrameVertices(nativePartixSoftVolume);
    }

    public int[] GetWireFrameIndices() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        return world.GetWireFrameIndices(nativePartixSoftVolume);
    }

    public VehiclePointLoad[] GetPointLoads() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        return world.GetPointLoads(nativePartixSoftVolume);
    }

    public void SetPointLoads(VehiclePointLoad[] loads) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.SetPointLoads(nativePartixSoftVolume, loads);
    }

    public void AnalyzeVehicle() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.AnalyzeVehicle(
            nativePartixSoftVolume,
            ref vehicleParameter,
            Vector3.zero,
            prevOrientation,
            currOrientation,
            sensoryBalance,
            ref vehicleAnalyzeData);
        sensoryBalance = vehicleAnalyzeData.sensory_balance;
    }

    public void AccelerateVehicle(Vector3 accel) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.AccelerateVehicle(
            nativePartixSoftVolume,
            ref vehicleParameter,
            accel);
    }

    public void Rotate(Quaternion q) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.RotateEntity(nativePartixSoftVolume, q);
    }

    public void Rotate(Quaternion q, Vector3 pivot) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.RotateEntity(nativePartixSoftVolume, q, pivot);
    }

    public void Fix() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.FixEntity(nativePartixSoftVolume, sourcePosition);
    }

    public void SetFeatures(EntityFeatures ef) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.SetEntityFeatures(nativePartixSoftVolume, ef);
    }

    public void AttachDeform(Matrix4x4 partixOrientation) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        foreach (MeshRenderer r in meshRenderers) {
            Transform t = r.transform;
            List<Matrix4x4> matrices = new List<Matrix4x4>();
            while (t.gameObject != controlTransform.gameObject) {
                Matrix4x4 mm = Matrix4x4.TRS(
                    t.localPosition, t.localRotation, t.localScale);
                matrices.Add(mm);
                t = t.parent;
            }

            Matrix4x4 m = controlTransform.localToWorldMatrix;
            m *= partixOrientation;

            for (int i = 0 ; i < matrices.Count ; i++) {
                m *= matrices[matrices.Count - 1 - i];
            }
            Material material = r.sharedMaterial;

            // TODO: 毎回作ってる
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetMatrix("_Deform", m);
            props.SetMatrix("_ShadowDeform", m);
            r.SetPropertyBlock(props);
        }
        foreach (SkinnedMeshRenderer r in skinnedMeshRenderers) {
            List<Matrix4x4> matrices = new List<Matrix4x4>();

            Transform t = r.rootBone;
            {
                Matrix4x4 mm = Matrix4x4.TRS(
                    t.localPosition, t.localRotation, new Vector3(1,1,1));
                matrices.Add(mm);
            }

            t = r.transform.parent;
            while (t.gameObject != controlTransform.gameObject) {
                // scaleは自動でかかるらしい
                Matrix4x4 mm = Matrix4x4.TRS(
                    t.localPosition, t.localRotation, new Vector3(1,1,1));
                matrices.Add(mm);
                t = t.parent;
            }

            Matrix4x4 m = controlTransform.localToWorldMatrix;
            m *= partixOrientation;

            for (int i = 0 ; i < matrices.Count ; i++) {
                m *= matrices[matrices.Count - 1 - i];
            }
            Material material = r.sharedMaterial;

            forDebug = m;

            // TODO: 毎回作ってる
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetMatrix("_Deform", m);
            props.SetMatrix("_ShadowDeform", m);
            r.SetPropertyBlock(props);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        if (controlTransform != null) {
            Gizmos.DrawWireSphere(controlTransform.position, 0.25f);
        }
    }

    public Body[] GetContacts() {
        // TODO: Bodyに置きたいがWorldをどうにかする必要がある
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        return world.GetContacts(nativePartixSoftVolume);
    }

    public override Vector3 GetPosition() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        return world.GetPosition(nativePartixSoftVolume);
    }
    
    public override void AddForce(Vector3 v) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.AddForce(nativePartixSoftVolume, v);
    }

}

}
