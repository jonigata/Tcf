using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Partix {

public class Plane : MonoBehaviour {
    World world;
    IntPtr nativePartixPlane;

    void Awake() {
        world = FindObjectOfType<World>();
    }

    IEnumerator Start() {
        yield return new WaitUntil(() => world.Ready());

        nativePartixPlane = world.CreatePlane(transform.position, transform.up);
    }

}

}
