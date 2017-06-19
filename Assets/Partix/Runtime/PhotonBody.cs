using UnityEngine;

namespace Partix {

public class PhotonBody : Photon.MonoBehaviour {
    PhotonWorld photonWorld;
    PhotonView photonView;

    float ping = 0;
    int count = 0;

    void Awake() {
        photonWorld = FindObjectOfType<PhotonWorld>();
        photonView = GetComponent<PhotonView>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.X)) {
            StartPing();
        }
    }

    public void StartPing() {
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
        if (!PhotonNetwork.isMasterClient) {
            photonView.RPC(
                "Pong",
                PhotonTargets.MasterClient,
                new object[] { time });
        }
    }

    [PunRPC]
    public void Pong(float time) {
        float m = Time.time - time;
        Debug.LogFormat("{0}sec", Time.time - time);

        ping = (ping * count) + m;
        count++;
        ping /= count;
        if (5 < count) { count = 5; }
        Debug.LogFormat("avg: {0}sec", ping);
    }
}

}

