using UnityEngine;

public class Misc {
    public static void Fix(
        Partix.SoftVolume sv, float friction, float threshold) {
        Vector3[] vertices = sv.GetWireFrameVertices();
        Partix.VehiclePointLoad[] loads = sv.GetPointLoads();

        // boundingbox
        var bb = new BoundingBox(vertices);

        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 v = vertices[i];
            Vector3 np = bb.CalculateNormalizedPosition(v);
            loads[i].friction = 0.2f;
            loads[i].fix_target = np.y < threshold ? 1 : 0;
            // loads[i].fix_target = 0;
        }
        sv.SetPointLoads(loads);
    }

}