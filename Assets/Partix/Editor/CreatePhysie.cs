using UnityEngine;
using UnityEditor;

namespace Partix {

public class PhysieCreator {
    [MenuItem("GameObject/Partix/World", false, 1)]
    static void CreateWorld(MenuCommand menuCommand) {
        GameObject go = new GameObject("World");
        go.AddComponent<PartixDll>();
        var world = go.AddComponent<World>();
        world.deltaTime = 0.016f;

        GameObjectUtility.SetParentAndAlign(
            go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Partix/Physie", false, 2)]
    static void CreatePhysie(MenuCommand menuCommand) {
        GameObject go = new GameObject("Physie");
        var softVolume = go.AddComponent<SoftVolume>();
        softVolume.controlTransform = go.transform;
        GameObject wireframe = new GameObject("WireFrame");
        wireframe.transform.SetParent(go.transform, false);
        wireframe.AddComponent<WireFrame>().softVolume = softVolume;

        GameObjectUtility.SetParentAndAlign(
            go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}

}

