using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Partix {

public class ViewAssigner {
    [MenuItem("Partix/Assign View Object")]
    static void AssignViewObject() {
        foreach (GameObject go in Selection.gameObjects) {
            var t = go.transform;
            while (t.GetComponent<SoftVolume>() == null) {
                t = t.parent;
            }

            var sv = t.GetComponent<SoftVolume>();
            sv.meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
            sv.skinnedMeshRenderers =
                go.GetComponentsInChildren<SkinnedMeshRenderer>();
        }
    }

}

}
