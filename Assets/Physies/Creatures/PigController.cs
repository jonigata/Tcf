using UnityEngine;
using System;
using System.Collections;
using InControl;

public class PigController : MonoBehaviour {
    [SerializeField] Vector3 liftUp;

    const float epsilon = 1.0e-6f;

    Vector3 accel;
    Vector3 velocity;
    Partix.World world;
    Partix.SoftVolume sv;

    void Awake() {
        world = FindObjectOfType<Partix.World>();
        sv = GetComponent<Partix.SoftVolume>();

        accel = Vector3.zero;
        velocity = Vector3.zero;
    }

    IEnumerator Start() {
        while (!sv.Ready()) { yield return null; }

        Vector3[] vertices = sv.GetWireFrameVertices();
        Partix.VehiclePointLoad[] loads = sv.GetPointLoads();

        // boundingbox
        BoundingBox bb = new BoundingBox(vertices);

        // weightê›íË
        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 np = bb.CalculateNormalizedPosition(vertices[i]);
            
            float w = 1.0f;
            float z = 1.0f - np.z;
            float y = 1.0f - np.y;
            w += z * z * y * 6.0f;
            loads[i].weight = w * 0.2f;
            loads[i].friction = w * 0.2f;
            loads[i].fix_target = 0;
        }
        sv.SetPointLoads(loads);
    }

    void Update() {
        if (!sv.Ready()) { return; }
        
        float grip = 
            (sv.vehicleAnalyzeData.front_grip + 
             sv.vehicleAnalyzeData.rear_grip) * 0.5f;

        sv.AccelerateVehicle(
            accel * sv.vehicleParameter.accel_factor * grip +
            accel.magnitude * liftUp);

        sv.Rotate(GetBalanceRotation());
        // sv.Rotate(GetTurningRotation());
    }

    public void UpdateAccel(Vector3 input) {
/*
        // ã}ê˘âÒÇÕÉuÉåÅ[ÉL 
        {
            Vector3 v0 = velocity.normalized;
            Vector3 v1 = v.normalized;
            float breakAngle = Mathf.Deg2Rad * sv.vehicleParameter.brake_angle;
            float angle = Mathf.Acos(Vector3.Dot(v0, v1));
            if (breakAngle < Mathf.Abs(angle) && 3.0f < velocity.magnitude) {
                Debug.LogFormat("BREAKING: {0}\n", angle);
                v = Vector3.zero;
            }
        }
*/

        Vector3 a = accel;
        float len0 = a.magnitude;
        if (epsilon < len0) {
            a = accel.normalized;
        } else {
            a = sv.vehicleAnalyzeData.front;
        }
        float len1 = input.magnitude;

        // ã}åÉÇ»ïœâªñhé~
        float vr = world.deltaTime * 5.0f;
        float tr = world.deltaTime * 5.0f;

        // êiçsï˚å¸ÇãÖñ ï‚äÆ
        if (epsilon < len1) {
            Quaternion q = Quaternion.FromToRotation(a, input);
            Quaternion qq = Quaternion.Slerp(Quaternion.identity, q, tr);
            a = qq * a;
        }
        accel = a * Mathf.Lerp(len0, len1, vr);
    }


    Quaternion GetBalanceRotation() {
        var a = sv.vehicleAnalyzeData;
        
        // épê®Ç™ïúå≥íÜÇæÇ¡ÇΩÇÁâΩÇ‡ÇµÇ»Ç¢
        float pa = Vector3.Dot(Vector3.up, a.old_back);
        float ca = Vector3.Dot(Vector3.up, a.back);
        if (pa < ca) { return Quaternion.identity; }

        float angle = AxisAngleOnAxisPlane(
            Vector3.zero, a.back, Vector3.up, a.front);

        if (a.left_grip == 0 && 0 < a.right_grip) {
            // ç∂ë´Ç™ïÇÇ¢ÇƒÇÈÇ∆Ç´ÇÕâEÇ…ÇÕåXÇ©Ç»Ç¢
            if (0 < angle) {
                return Quaternion.identity;
            }
        } else if (a.right_grip == 0 && 0 < a.left_grip) {
            // âEë´Ç™ïÇÇ¢ÇƒÇÈÇ∆Ç´ÇÕç∂Ç…ÇÕåXÇ©Ç»Ç¢
            if (angle < 0) {
                return Quaternion.identity;
            }
        }


        float angleMax =
            sv.vehicleParameter.balance_angle_max * world.deltaTime;
        angle = Mathf.Clamp(angle, -angleMax, angleMax);
        return Quaternion.AngleAxis(angle, a.front);
    }

    Quaternion GetTurningRotation() {
        var a = sv.vehicleAnalyzeData;
        
        // épê®Ç™ïúå≥íÜÇæÇ¡ÇΩÇÁâΩÇ‡ÇµÇ»Ç¢
        Vector3 an = accel.normalized;
        float pa = Vector3.Dot(an, a.old_front);
        float ca = Vector3.Dot(an, a.front);
        if (pa < ca) { return Quaternion.identity; }

        Quaternion q =  Quaternion.FromToRotation(
            sv.vehicleAnalyzeData.front, accel.normalized);
        return limitRotation(
            q, sv.vehicleParameter.turning_angle_max * world.deltaTime);
    }

    // http://tiri-tomato.hatenadiary.jp/entry/20121013/1350121871
    float AxisAngleOnAxisPlane( Vector3 origin, Vector3 fromDirection, Vector3 toDirection, Vector3 axis ) {
	fromDirection.Normalize();
	axis.Normalize();
	Vector3 toDirectionProjected = toDirection - axis * Vector3.Dot(axis,toDirection);
	toDirectionProjected.Normalize();
	return 
            Mathf.Acos(Mathf.Clamp(Vector3.Dot(fromDirection,toDirectionProjected),-1f,1f)) *
            (Vector3.Dot(Vector3.Cross(axis,fromDirection), toDirectionProjected) < 0f ? -Mathf.Rad2Deg : Mathf.Rad2Deg);
    }

    Quaternion limitRotation(Quaternion q, float max) {
        float angle;
        Vector3 axis;
        q.ToAngleAxis(out angle, out axis);
        if (max < angle) {
            return Quaternion.AngleAxis(max, axis);
        } else {
            return q;
        }
    }
}

