using UnityEngine;
using System;
using System.Collections;
using InControl;

public class Mushroom : MonoBehaviour {
    Partix.SoftVolume softVolume;

    void Awake() {
        softVolume = GetComponent<Partix.SoftVolume>();
    }

    IEnumerator Start() {
        while (!softVolume.Ready()) { yield return null; }

        Misc.Fix(softVolume, 0.2f, 0.2f);

        var ef = new Partix.EntityFeatures();
        ef.stretch_factor = 0.9f;
        ef.restore_factor = 0.4f;
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
            
            Vector3 v1 = b.GetPosition();
            Vector3 diff = v1 - v0;
            diff.y = 0;
            if (diff.sqrMagnitude == 0) { continue; }
            Vector3 d = diff.normalized;
            b.AddForce(d * 320.0f);
            softVolume.AddForce(-d * 15.0f);
            // b.AddForce(Vector3.up * 10.0f);
        }
    }

}

