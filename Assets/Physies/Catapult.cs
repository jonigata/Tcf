using UnityEngine;
using System;
using System.Collections;
using InControl;

public class Catapult : MonoBehaviour {
    Partix.SoftVolume softVolume;

    public Vector3 direction;
    public float power;

    void Awake() {
 softVolume = GetComponent<Partix.SoftVolume>();
    }

    IEnumerator Start() {
        while (!softVolume.Ready()) { yield return null; }

        Misc.Fix(softVolume, 0.2f, 0.2f);

        var ef = new Partix.EntityFeatures();
        ef.stretch_factor = 0.6f;
        ef.restore_factor = 0.6f;
        ef.alive = 1;
        ef.positive = 1;
        ef.influential = 1;
        ef.auto_freezing = 0;
        ef.frozen = 0;
        softVolume.SetFeatures(ef);
    }

    void Update() {
        if (!softVolume.Ready()) { return; }
        softVolume.Fix();

        Partix.Body[] a = softVolume.GetContacts();

        Vector3 v0 = softVolume.GetPosition();
        foreach (Partix.Body b in a) {
            if (b as Partix.SoftVolume == null) { continue; }
            b.AddForce(direction.normalized * power);
        }
    }

}

