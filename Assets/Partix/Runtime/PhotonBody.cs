using UnityEngine;

namespace Partix {

public class PhotonBody : Photon.MonoBehaviour {
    PhotonView photonView;

    float ping = 0;
    int count = 0;

    public float blendFactor = 0.1f;
    public float velocityBlendFactor = 0.5f;

    public SoftVolume softVolume;
    double sendTime;
    public Vector3 position;
    public Vector3 prevPosition;
    public Quaternion orientation;
    public Quaternion prevOrientation;

    public Matrix4x4 matrix;

    void Awake() {
        photonView = GetComponent<PhotonView>();

        sendTime = 0;
        position = Vector3.zero;
        prevPosition = Vector3.zero;
        orientation = Quaternion.identity;
        prevOrientation = Quaternion.identity;
    }

    void Update() {
        if (!softVolume.Ready()) { return; }

        if (photonView.isMine) {
            position = softVolume.currOrientation.GetColumn(3);
            prevPosition = softVolume.prevOrientation.GetColumn(3);
            orientation = GetOrientation(softVolume.currOrientation);
            prevOrientation = GetOrientation(softVolume.prevOrientation);
        } else {
            var t = PhotonNetwork.time - sendTime;
            var z = (float)(t / softVolume.world.deltaTime);

            Vector3 v = position - prevPosition;
            Vector3 p = position + v * z;
            Quaternion q =
                Quaternion.SlerpUnclamped(prevOrientation, orientation, z);
            Matrix4x4 m = Matrix4x4.TRS(p, q, Vector3.one);
            matrix = m;
            softVolume.BlendPosition(m, blendFactor, velocityBlendFactor);
        }
    }

    Quaternion GetOrientation(Matrix4x4 m) {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        stream.Serialize(ref position);
        stream.Serialize(ref prevPosition);
        stream.Serialize(ref orientation);
        stream.Serialize(ref prevOrientation);
        if (!stream.isWriting) {
            sendTime = info.timestamp;
        }
    }
}

}

