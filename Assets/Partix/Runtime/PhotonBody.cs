using UnityEngine;

namespace Partix {

// TODO: delay�v���𕪂���

public class PhotonBody : Photon.MonoBehaviour {
    PhotonWorld photonWorld;
    PhotonView photonView;

    float ping = 0;
    int count = 0;

    public float blendFactor;
    public float velocityBlendFactor;

    public SoftVolume softVolume;
    public float receiveTime;
    public Vector3 position;
    public Vector3 prevPosition;
    public Quaternion orientation;
    public Quaternion prevOrientation;

    public Matrix4x4 matrix;


    void Awake() {
        photonWorld = FindObjectOfType<PhotonWorld>();
        photonView = GetComponent<PhotonView>();

        receiveTime = 0;
        position = Vector3.zero;
        prevPosition = Vector3.zero;
        orientation = Quaternion.identity;
        prevOrientation = Quaternion.identity;
    }

    void Update() {
        if (!softVolume.Ready()) { return; }

        if (Input.GetKeyDown(KeyCode.X)) {
            StartPing();
        }

        if (PhotonNetwork.isMasterClient) {
            position = softVolume.currOrientation.GetColumn(3);
            prevPosition = softVolume.prevOrientation.GetColumn(3);
            orientation = GetOrientation(softVolume.currOrientation);
            prevOrientation = GetOrientation(softVolume.prevOrientation);
        } else {
            var t = Time.time - receiveTime;
            var z = t / softVolume.world.deltaTime;

            Vector3 v = position + (position - prevPosition) * z;
            Quaternion q =
                Quaternion.SlerpUnclamped(prevOrientation, orientation, z);
            Matrix4x4 m = Matrix4x4.TRS(v, q, Vector3.one);
            matrix = m;
            softVolume.BlendPosition(m, blendFactor, velocityBlendFactor);
        }
    }

    Quaternion GetOrientation(Matrix4x4 m) {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
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

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        stream.Serialize(ref position);
        stream.Serialize(ref prevPosition);
        stream.Serialize(ref orientation);
        stream.Serialize(ref prevOrientation);
        if (!stream.isWriting) {
            receiveTime = Time.time;
        }
    }
}

}

