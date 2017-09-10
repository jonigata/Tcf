using UnityEngine;
using System.Collections.Generic;

public class PlayerInstantiater : MonoBehaviour {
    void OnJoinedRoom() {
        Debug.Log("PlayerInstantiater.OnJoinedRoom");
        if (PhotonNetwork.isMasterClient) {
            GameObject go = PhotonNetwork.Instantiate(
                "playerSheep",
                new Vector3(1.9f, 4.2f, -1.4f),
                Quaternion.identity,
                0);
            go.transform.SetParent(this.transform, false);
        }
    }

}

