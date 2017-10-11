using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInstantiater : MonoBehaviour {
    public Text text;

    void OnJoinedRoom() {
        Debug.Log("PlayerInstantiater.OnJoinedRoom");
        if (PhotonNetwork.isMasterClient) {
            Debug.Log("Master Client");
            GameObject go = PhotonNetwork.Instantiate(
                "playerSheep",
                new Vector3(1.9f, 4.2f, -1.4f),
                Quaternion.identity,
                0);
            go.transform.SetParent(this.transform, false);
        } else {
            Debug.Log("Slave Client");
            GameObject go = PhotonNetwork.Instantiate(
                "playerSheep",
                new Vector3(1.9f, 3.2f, 1.4f),
                Quaternion.identity,
                0);
            go.transform.SetParent(this.transform, false);
        }
    }

    void Update() {
        text.text = PhotonNetwork.GetPing().ToString();
    }
}

