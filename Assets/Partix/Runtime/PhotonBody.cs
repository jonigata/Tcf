using UnityEngine;

namespace Partix {

public class PhotonBody : Photon.MonoBehaviour {
    PhotonWorld photonWorld;
    PhotonView photonView;

    void Awake() {
        photonWorld = FindObjectOfType<PhotonWorld>();
        photonView = GetComponent<PhotonView>();
    }

    void Start() {
        if (PhotonNetwork.isMasterClient) {
            photonView.RPC(
                "Ping",
                PhotonTargets.AllViaServer,
                new object[] { Time.time });
        }
    }

    [PunRPC]
    public void Ping(float time) {
        Debug.Log("ping");
        photonView.RPC(
            "Pong",
            PhotonTargets.MasterClient,
            new object[] { time });
    }

    [PunRPC]
    public void Pong(float time) {
        Debug.LogFormat("{0}ms", Time.time - time);
    }
}

}

