using UnityEngine;

namespace Partix {

public class Body: MonoBehaviour {
    public virtual Vector3 GetPosition() { return Vector3.zero; }
    public virtual void AddForce(Vector3 v) {}

    public ClassId GetClassId() { return ClassId.MAX; }

}

}
