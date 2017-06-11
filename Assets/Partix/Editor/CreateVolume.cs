using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Partix {

public class VolumeCreator {
    [MenuItem("Assets/Create Volume From Tcf")]
    static void CreateVolume() {
        string path = EditorUtility.OpenFilePanel("Select Tcf", "", "tcf");
        Debug.Log(path);
        Debug.Log(CustomAssetUtil.GetSelectedPathOrFallback());
        var filename = System.IO.Path.GetFileNameWithoutExtension(path);

        var asset = ScriptableObject.CreateInstance<Volume>();
        AssetDatabase.CreateAsset(
            asset,
            AssetDatabase.GenerateUniqueAssetPath(
                CustomAssetUtil.GetSelectedPathOrFallback() +
                "/" + filename + ".asset"));
        AssetDatabase.Refresh();

        FileInfo fi=  new FileInfo(path);
        try {
            using (var sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)) {
                // .node
                int nodeCount = Convert.ToInt32(sr.ReadLine().Split(null)[0]);
                Debug.Log("nodeCount = " + nodeCount.ToString());
                var vertices = new Vector3[nodeCount];
                for (int i = 0 ; i < nodeCount ; i++) {
                    string line = sr.ReadLine();
                    string[] a = Split(line);
                    Vector3 v = new Vector3();
                    v.x = Convert.ToSingle(a[1]);
                    v.y = Convert.ToSingle(a[2]);
                    v.z = Convert.ToSingle(a[3]);
                    vertices[i] = v;
                }

                // .ele
                int tetraCount = Convert.ToInt32(sr.ReadLine().Split(null)[0]);
                Debug.Log("tetraCount = " + tetraCount.ToString());
                var tetrahedra = new Tetrahedron[tetraCount];
                for (int i = 0 ; i < tetraCount ; i++) {
                    string line = sr.ReadLine();
                    string[] a = Split(line);
                    Tetrahedron t = new Tetrahedron();
                    t.i0 = Convert.ToInt32(a[1]);
                    t.i1 = Convert.ToInt32(a[2]);
                    t.i2 = Convert.ToInt32(a[3]);
                    t.i3 = Convert.ToInt32(a[4]);
                    tetrahedra[i] = t;
                }

                // .face
                int faceCount = Convert.ToInt32(sr.ReadLine().Split(null)[0]);
                Debug.Log("faceCount = " + faceCount.ToString());
                var faces = new Triangle[faceCount];
                for (int i = 0 ; i < faceCount ; i++) {
                    string line = sr.ReadLine();
                    string[] a = Split(line);
                    Triangle t = new Triangle();
                    t.i0 = Convert.ToInt32(a[1]);
                    t.i1 = Convert.ToInt32(a[2]);
                    t.i2 = Convert.ToInt32(a[3]);
                    faces[i] = t;
                }
                asset.vertices = vertices;
                asset.tetrahedra = tetrahedra;
                asset.faces = faces;
            }
            AssetDatabase.SaveAssets();
        }
        catch (Exception e) {
            Debug.Log(e);
        }

    }

    public static string[] Split(string s) {
        string[] a = s.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        return a;
    }
}

}
