using UnityEngine;
using System.Collections;

namespace PartixUtil {

public class AutoRewind {
    public Partix.SoftVolume softVolume;
    public float bottom;

    Vector3 initialPosition;

    IEnumerator Start() {
        yield return softVolume.Ready();
        initialPosition = softVolume.GetInitialPosition();
    }

    void Update() {
        var v = softVolume.GetPosition();
        if (v.y < bottom) {
            softVolume.Teleport(initialPosition);
        }
    }
    
}

}

