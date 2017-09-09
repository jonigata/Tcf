using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhotonAllocatee : Photon.MonoBehaviour {
    public UnityEvent OnAllocate;
    public UnityEvent OnActivate;
}
