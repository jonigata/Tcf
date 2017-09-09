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
        Allocate(allocateeIdSeed);
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
                allocateeIdSeed++;
            } 
        } else {
            Debug.Log("This is not master client");
        }
    }

    [PunRPC]
    public void Allocate(int allocateeId) {
        Debug.Log("Allocate " + allocateeId.ToString());
        allocatees[allocateeId].OnAllocate.Invoke();

        photonView.RPC(
            "Activate",
            PhotonTargets.Others,
            new object[] { allocateeIdSeed });
    }

    [PunRPC]
    public void Activate(int allocateeId) {
        Debug.Log("Activate " + allocateeId.ToString());
        allocatees[allocateeId].OnActivate.Invoke();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }    
    
}
