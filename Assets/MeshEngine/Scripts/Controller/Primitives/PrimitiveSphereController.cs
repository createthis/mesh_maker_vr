using System.Collections.Generic;
using UnityEngine;

namespace MeshEngine.Controller {
    public class PrimitiveSphereController : PrimitiveBaseController {
        public List<GameObject> vertexInstances;

        private Vector3 center;
        private int minNumberOfSides = 6;
        private int maxNumberOfSides = 24;
        private int lastNumberOfSides;
        private int vertexCountBeforeSphere;
        private int triangleInstanceCountBeforeSphere;
        private int triangleCountBeforeSphere;
        private int triangleMaterialsCountBeforeSphere;

        public override void OnTriggerDown(Transform controller, int controllerIndex) {
            base.OnTriggerDown(controller, controllerIndex);
            triggerCount++;
            if (triggerCount == 1) FirstTrigger(controller, controllerIndex);
            if (triggerCount == 2) SecondTrigger(controller, controllerIndex);
        }

        public override void OnTriggerUpdate(Transform controller, int controllerIndex) {
            UpdatePositions(controller, controllerIndex);
        }

        public void ClearVertexInstances() {
            mesh.vertices.TruncateToCount(vertexCountBeforeSphere);
            for (int i = mesh.triangles.triangleInstances.Count - 1; i >= triangleInstanceCountBeforeSphere; i--) {
                mesh.triangles.DeactivateAndMoveToPool(mesh.triangles.triangleInstances[i].instance);
                mesh.triangles.triangleInstances.RemoveAt(i);
            }
            mesh.triangles.TruncateToCount(triangleCountBeforeSphere);
            foreach (GameObject vertexInstance in vertexInstances) {
                //meshController.verticesManager.RemoveByVertexInstance(vertexInstance);
                mesh.vertices.DeactivateAndMoveToPool(vertexInstance);
            }
            mesh.materials.TruncateToCount(triangleMaterialsCountBeforeSphere);
            vertexInstances.Clear();
        }

        private Vector3 RotateAround(Quaternion startRotation, Quaternion rotation, Vector3 point, Vector3 pivot) {
            // In order to add two quaternion rotations correctly you multiply them.
            // To subtract you need to multiply by the inverse.
            Quaternion rotationDelta = (rotation * Quaternion.Inverse(startRotation));
            return Snap.RotateAroundPivotWorld(mesh.transform, point, pivot, rotationDelta);
        }

        private float PercentageByAngle(float angle) {
            // angle   result
            // ----- = ---
            // 360     100
            //
            // result = angle / 360 * 100
            return angle / 360.0f * 100.0f;
        }

        private int NumSidesByPercentage(float percentage) {
            return Mathf.FloorToInt(percentage * (maxNumberOfSides - minNumberOfSides) / 100 + minNumberOfSides);
        }

        private int NumSidesByTwoVectors(Vector3 from, Vector3 to) {
            float angle = Vector3.Angle(from, to);
            return NumSidesByPercentage(PercentageByAngle(angle));
        }

        public void AddVertex(Vector3 position, Quaternion startRotation, Quaternion rotation) {
            position = RotateAround(startRotation, rotation, position, center);
            GameObject vertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            vertexInstances.Add(vertexInstance);
        }

        public void MoveVertex(int index, Vector3 position, Quaternion startRotation, Quaternion rotation) {
            position = RotateAround(startRotation, rotation, position, center);
            GameObject vertexInstance = vertexInstances[index];
            VertexController vertexController = vertexInstance.GetComponent<VertexController>();
            vertexController.UpdateLocalPositionFromWorldPosition(position);
        }

        public void DestroyAndRecreateVertices(int numberOfSides, Vector3 controllerPosition, Vector3 controllerUp) {
            ClearVertexInstances();

            Quaternion startRotation = Quaternion.identity;
            Quaternion rotation = Quaternion.LookRotation(controllerUp, controllerPosition - center);

            float radius = Vector3.Distance(center, controllerPosition); // distance between center and controller

            // NOTE: Generally a ratio of 3/2 is good for longitude/latitude, with 6/4 being the minimum recognizable sphere.

            // Longitude |||
            int numberOfLinesLongitude = numberOfSides;
            // Latitude ---
            int numberOfLinesLatitude = (int)(numberOfSides / 1.5f);

            float _pi = Mathf.PI;
            float _2pi = _pi * 2f;

            // north pole
            Vector3 firstPosition = Vector3.up * radius + center;
            AddVertex(firstPosition, startRotation, rotation);

            // middle
            for (int latitude = 0; latitude < numberOfLinesLatitude; latitude++) {
                float a1 = _pi * (float)(latitude + 1) / (numberOfLinesLatitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int longitude = 0; longitude <= numberOfLinesLongitude; longitude++) {
                    float a2 = _2pi * (float)(longitude == numberOfLinesLongitude ? 0 : longitude) / numberOfLinesLongitude;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    Vector3 position = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius + center;
                    AddVertex(position, startRotation, rotation);
                }
            }

            // south pole
            Vector3 lastPosition = Vector3.up * -radius + center;
            AddVertex(lastPosition, startRotation, rotation);

            // polar triangles
            for (int longitude = 0; longitude < numberOfLinesLongitude; longitude++) {
                // top cap
                mesh.triangles.AddTriangleByVertices(vertexInstances[longitude + 2], vertexInstances[longitude + 1], vertexInstances[0]);

                // bottom cap
                mesh.triangles.AddTriangleByVertices(
                    vertexInstances[vertexInstances.Count - 1],
                    vertexInstances[vertexInstances.Count - (longitude + 2) - 1],
                    vertexInstances[vertexInstances.Count - (longitude + 1) - 1]
                    );
            }
            // middle triangles
            for (int latitude = 0; latitude < numberOfLinesLatitude - 1; latitude++) {
                for (int longitude = 0; longitude < numberOfLinesLongitude; longitude++) {
                    int current = longitude + latitude * (numberOfLinesLongitude + 1) + 1;
                    int next = current + numberOfLinesLongitude + 1;

                    mesh.triangles.AddTriangleByVertices(vertexInstances[current], vertexInstances[current + 1], vertexInstances[next + 1]);
                    mesh.triangles.AddTriangleByVertices(vertexInstances[current], vertexInstances[next + 1], vertexInstances[next]);
                }
            }
            SetSelectable(false);
        }

        public Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c) {
            Vector3 planeNormal = Vector3.Cross(c - a, b - a).normalized * 0.025f;
            return planeNormal;
        }

        public void MoveVertices(int numberOfSides, Vector3 controllerPosition, Vector3 controllerUp) {
            Quaternion startRotation = Quaternion.identity;
            Quaternion rotation = Quaternion.LookRotation(controllerUp, controllerPosition - center);

            float radius = Vector3.Distance(center, controllerPosition); // distance between center and controller

            // NOTE: Generally a ratio of 3/2 is good for longitude/latitude, with 6/4 being the minimum recognizable sphere.

            // Longitude |||
            int numberOfLinesLongitude = numberOfSides;
            // Latitude ---
            int numberOfLinesLatitude = (int)(numberOfSides / 1.5f);

            float _pi = Mathf.PI;
            float _2pi = _pi * 2f;

            // north pole
            Vector3 firstPosition = Vector3.up * radius + center;
            MoveVertex(0, firstPosition, startRotation, rotation);

            // middle
            for (int latitude = 0; latitude < numberOfLinesLatitude; latitude++) {
                float a1 = _pi * (float)(latitude + 1) / (numberOfLinesLatitude + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int longitude = 0; longitude <= numberOfLinesLongitude; longitude++) {
                    float a2 = _2pi * (float)(longitude == numberOfLinesLongitude ? 0 : longitude) / numberOfLinesLongitude;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    int index = longitude + latitude * (numberOfLinesLongitude + 1) + 1;
                    Vector3 position = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius + center;
                    MoveVertex(index, position, startRotation, rotation);
                }
            }

            // south pole
            Vector3 lastPosition = Vector3.up * -radius + center;
            MoveVertex(vertexInstances.Count - 1, lastPosition, startRotation, rotation);
        }

        public void UpdatePositions(Transform controller, int controllerIndex) {
            Vector3 controllerPosition = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, controller.position) : controller.position;
            Quaternion snappedControllerRotation = Snap.RotationDelta(controller.rotation);
            Vector3 up = CalculateNormal(center, controllerPosition, controllerPosition + snappedControllerRotation * Vector3.right);
            int numberOfSides = NumSidesByTwoVectors(controller.up, up);

            if (numberOfSides != lastNumberOfSides) {
                DestroyAndRecreateVertices(numberOfSides, controllerPosition, up);
            } else {
                MoveVertices(numberOfSides, controllerPosition, up);
            }
            lastNumberOfSides = numberOfSides;
        }

        public void SelectVertices() {
            foreach (GameObject vertex in vertexInstances) {
                mesh.selection.SelectVertex(vertex);
            }
        }

        public void SetSelectable(bool value) {
            foreach (GameObject vertex in vertexInstances) {
                vertex.GetComponent<VertexController>().SetSelectable(value);
            }
        }

        public void FirstTrigger(Transform controller, int controllerIndex) {
            this.controllerIndex = controllerIndex;
            vertexInstances = new List<GameObject>();
            mesh.selection.Clear();
            center = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, controller.position) : controller.position;
            lastNumberOfSides = 0;
            transform.position = center;
            vertexCountBeforeSphere = mesh.vertices.vertices.Count;
            triangleCountBeforeSphere = mesh.triangles.triangles.Count;
            triangleInstanceCountBeforeSphere = mesh.triangles.triangleInstances.Count;
            triangleMaterialsCountBeforeSphere = mesh.materials.TriangleMaterialsCount();
            SetSelectable(false);
        }

        public void SecondTrigger(Transform controller, int controllerIndex) {
            UpdatePositions(controller, controllerIndex);
            SelectVertices();
            SetSelectable(true);
            Destroy(gameObject);
        }

        public void Cancel() {
            ClearVertexInstances();
            Destroy(gameObject);
        }

        private void HandleGrip() {
            Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
            SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)controllerIndex);

            if (controller.GetPress(gripButton)) {
                Cancel();
            }
        }

        // Update is called once per frame
        protected override void Update() {
            base.Update();
            HandleGrip();
        }
    }
}