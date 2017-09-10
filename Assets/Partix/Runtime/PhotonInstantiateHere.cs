using UnityEngine;
using UnityEngine.Events;

namespace Partix {

public class PhotonInstantiateHere : MonoBehaviour {
    public PhotonView photonView;
    public UnityEvent onInstantiateHere;

    void Start() {
        if (photonView.isMine) {
            onInstantiateHere.Invoke();
        }
    }
}

}

