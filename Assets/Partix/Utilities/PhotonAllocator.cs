using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonAllocator : Photon.MonoBehaviour {
    [SerializeField] PhotonAllocatee[] allocatees;
    [SerializeField] PhotonView photonView;
    
    int allocateeIdSeed = 0;

    void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom()");
        if (PhotonNetwork.isMasterClient) {
        Allocate(allocateeIdSeed);
            DoActivate();
        }
    }

    void OnPhotonPlayerConnected(PhotonPlayer player) {
        Debug.Log("PhotonPlayerConnected");
        if (PhotonNetwork.isMasterClient) {
            Debug.Log("This is master client");
            if (!player.isLocal) {
                photonView.RPC(
                    "Allocate",
                    player,
                    new object[] { allocateeIdSeed });
            } 
            DoActivate();
        } else {
            Debug.Log("This is not master client");
        }
    }

    void DoActivate() {
        photonView.RPC(
            "Activate",
            PhotonTargets.AllBuffered,
            new object[] { allocateeIdSeed });
        allocateeIdSeed++;
    }

    [PunRPC]
    public void Allocate(int allocateeId) {
        Debug.Log("Allocate " + allocateeId.ToString());
        allocatees[allocateeId].OnAllocate.Invoke();
    }

    [PunRPC]
    public void Activate(int allocateeId) {
        Debug.Log("Activate " + allocateeId.ToString());
        allocatees[allocateeId].OnActivate.Invoke();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }    
    
}
