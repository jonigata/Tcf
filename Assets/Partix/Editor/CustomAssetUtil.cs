using UnityEngine;
using UnityEditor;
using System.IO;

namespace Partix {

public static class CustomAssetUtil {
    public static string GetSelectedPathOrFallback() {
        string path = "Assets";

        foreach(UnityEngine.Object obj
                in Selection.GetFiltered(
                    typeof(UnityEngine.Object),
                    SelectionMode.Assets)) {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path)&& File.Exists(path)) {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}

}
