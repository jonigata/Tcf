using UnityEngine;
using System;
using System.Collections;

public class Flower : MonoBehaviour {
    Partix.SoftVolume softVolume;

    void Awake() {
        softVolume = GetComponent<Partix.SoftVolume>();

        string[] colors = {"Red", "Violet", "Yellow", "White"};
        var color = colors[UnityEngine.Random.Range(0, 4)];

        var prefab = string.Format(
            "Poppy/{0}/Poppy_{0}_0{1}", color,
            UnityEngine.Random.Range(0, 9));

        GameObject view =
            GameObject.Instantiate(
                Resources.Load<GameObject>(prefab), transform, false);
        softVolume.meshRenderers =
            view.GetComponentsInChildren<MeshRenderer>();

    }

    IEnumerator Start() {
        while (!softVolume.Ready()) { yield return null; }

        Vector3[] vertices = softVolume.GetWireFrameVertices();
        Partix.VehiclePointLoad[] loads = softVolume.GetPointLoads();

        // boundingbox
        BoundingBox bb = new BoundingBox(vertices);

        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 v = vertices[i];
            Vector3 np = bb.CalculateNormalizedPosition(v);
            loads[i].friction = 0.2f;
            loads[i].fix_target = np.y < 0.1f ? 1 : 0;
            // loads[i].fix_target = 0;
        }
        softVolume.SetPointLoads(loads);

        Partix.EntityFeatures ef = new Partix.EntityFeatures();
        ef.stretch_factor = 0.1f;
        ef.restore_factor = 0.8f;
        ef.alive = 1;
        ef.positive = 1;
        ef.influential = 1;
        ef.auto_freezing = 1;
        ef.frozen = 1;
        // ef.auto_freezing = 0;
        // ef.frozen = 0;
        softVolume.SetFeatures(ef);
    }

    void Update() {
        if (!softVolume.Ready()) { return; }
        softVolume.Fix();
    }

}

