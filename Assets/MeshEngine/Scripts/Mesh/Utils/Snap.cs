using UnityEngine;
using System;

namespace MeshEngine {
    public static class Snap {
        public static Vector3 WorldPosition(Transform transform, Vector3 position) {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            Vector3 snappedLocalPosition = Vector3(localPosition);
            Vector3 snappedWorldPosition = transform.TransformPoint(snappedLocalPosition);
            return snappedWorldPosition;
        }

        public static Vector3 Vector3(Vector3 vector) {
            float snapIncrements = Settings.snapIncrements;
            return new Vector3(
                RoundByIncrements(vector.x, snapIncrements),
                RoundByIncrements(vector.y, snapIncrements),
                RoundByIncrements(vector.z, snapIncrements)
                );
        }

        public static float RoundByIncrements(float value, float increment) {
            //http://stackoverflow.com/questions/1329426/how-do-i-round-to-the-nearest-0-5
            float divisor = 1.0f / increment;
            return (float)Math.Round((double)(value * divisor)) / divisor;
        }

        public static Quaternion LocalRotationOfWorldRotation(Transform transform, Quaternion worldRotation) {
            return Quaternion.Inverse(transform.rotation) * worldRotation;
        }

        public static Quaternion RotationDelta(Quaternion rotationDelta) {
            if (!Settings.SnapEnabled()) return rotationDelta;
            float rotationSnapIncrements = Settings.rotationSnapIncrements;

            Vector3 eulerRotationDelta = rotationDelta.eulerAngles;
            Quaternion snappedRotationDelta = Quaternion.Euler(
                Mathf.Round(eulerRotationDelta.x / rotationSnapIncrements) * rotationSnapIncrements,
                Mathf.Round(eulerRotationDelta.y / rotationSnapIncrements) * rotationSnapIncrements,
                Mathf.Round(eulerRotationDelta.z / rotationSnapIncrements) * rotationSnapIncrements
            );
            return snappedRotationDelta;
        }

        //http://answers.unity3d.com/questions/47115/vector3-rotate-around.html
        //Returns the rotated Vector3 using a Quaterion
        public static Vector3 RotateAroundPivot(Transform transform, Vector3 point, Vector3 pivot, Quaternion angle) {
            // NOTE: (point - pivot) Gets a vector that points from the pivot's position to the point's.
            Vector3 localPoint = transform.InverseTransformPoint(point);
            Vector3 localPivot = transform.InverseTransformPoint(pivot);

            Vector3 newLocalPoint = angle * (localPoint - localPivot) + localPivot;
            return transform.TransformPoint(newLocalPoint);
        }

        public static Vector3 RotateAroundPivotWorld(Transform transform, Vector3 point, Vector3 pivot, Quaternion angle) {
            // NOTE: (point - pivot) Gets a vector that points from the pivot's position to the point's.
            return angle * (point - pivot) + pivot;
        }
    }
}