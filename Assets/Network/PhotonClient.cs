using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonClient : MonoBehaviour {
    void Start () {
        PhotonNetwork.ConnectUsingSettings(null);
    }
	
    void OnJoinedLobby () {
        Debug.Log("OnJoinedLobby()");
        RoomOptions roomOptions = new RoomOptions() {
            isVisible = false,
            maxPlayers = 6
        };
        PhotonNetwork.JoinOrCreateRoom("TCF", roomOptions, TypedLobby.Default);
    }
}
