using UnityEngine;
using System.Collections.Generic;

namespace Partix {

public class PhotonWorld : Photon.MonoBehaviour {
    class ChildEntry {
        public float ping;
        int count;

        public ChildEntry() { ping = 0; count = 0; }
        public void Update(float m) {
            ping = (ping * count) + m;
            count++;
            ping /= count;
            if (5 < count) { count = 5; }
            Debug.LogFormat("avg: {0}sec", ping);
        }
    }

    Dictionary<int, ChildEntry> entries;
    

    void OnJoinedRoom() {
        if (PhotonNetwork.isMasterClient) {
            entries = new Dictionary<int, ChildEntry>();
            StartPing();
        }
    }

    public void StartPing() {
        photonView.RPC(
            "Ping",
            PhotonTargets.Others,
            Time.time);
    }

    [PunRPC]
    public void Ping(float time) {
        Debug.Log("ping");
        photonView.RPC(
            "Pong",
            PhotonTargets.MasterClient,
            time);
    }

    [PunRPC]
    public void Pong(float time, PhotonMessageInfo info) {
        ChildEntry e = null;
        if (!entries.ContainsKey(info.sender.ID)) {
            e = entries[info.sender.ID];
        } else {
            e = new ChildEntry();
            entries[info.sender.ID] = e;
        }

        float m = Time.time - time;
        Debug.LogFormat("sender: {0}, {1}sec", info.sender.ID, m);
        e.Update(m);
    }

    public float GetDelay(int playerId) {
        return entries[playerId].ping;
    }

}

}

