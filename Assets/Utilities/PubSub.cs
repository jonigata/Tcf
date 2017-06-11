using UnityEngine;
using System.Collections.Generic;

public abstract class ASub : MonoBehaviour {
}

public abstract class APubSub : MonoBehaviour {
    public abstract void AddSubscriber(ASub s);
    public abstract void RemoveSubscriber(ASub s);
}

public abstract class Sub<T> : ASub {
    [SerializeField] protected APubSub pubSub;

    protected void Awake() {
        Debug.Log("Sub<T>.Awake");
        if (pubSub == null) {
            pubSub = FindObjectOfType<PubSub<T>>();
        }
    }

    void OnEnable() {
        Debug.Log("Sub<T>.OnEnable");
        pubSub.AddSubscriber(this);
        Debug.Log("OnEnable");
    }

    void OnDisable() {
        pubSub.RemoveSubscriber(this);
        Debug.Log("OnDisable");
    }

    public abstract void Subscribe(T x);
}

public class PubSub<T> : APubSub {
    List<ASub> z = new List<ASub>();

    public void Publish(T x) {
        foreach (ASub zz in z) {
            var zzz = (Sub<T>)zz;
            zzz.Subscribe(x);
        }
    }

    public override void AddSubscriber(ASub s) {
        z.Add(s);
    }

    public override void RemoveSubscriber(ASub s) {
        z.Remove(s);
    }
}