using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAllocator : Photon.MonoBehaviour {
    [SerializeField] GameObject[] players;
    [SerializeField] PhotonView photonView;
    
    int playerId = 1;

    void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom()");
    }

    void OnPhotonPlayerConnected(PhotonPlayer player) {
        Debug.Log("PhotonPlayerConnected");
        if (PhotonNetwork.isMasterClient) {
            Debug.Log("This is master client");
            if (!player.isLocal) {
                photonView.RPC("Allocate", player, new object[] { playerId++ });
            }
        } else {
            Debug.Log("This is not master client");
        }
    }

    [PunRPC]
    public void Allocate(int playerId) {
        Debug.Log("Allocate " + playerId.ToString());
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }    
    
}
