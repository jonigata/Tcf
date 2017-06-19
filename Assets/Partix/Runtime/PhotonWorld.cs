using UnityEngine;

namespace Partix {

public class PhotonWorld : Photon.MonoBehaviour {
    [PunRPC]
    public void Ping() {
        Debug.Log("ping");
    }

    [PunRPC]
    public void Pong() {
        Debug.Log("pong");
    }
}

}

