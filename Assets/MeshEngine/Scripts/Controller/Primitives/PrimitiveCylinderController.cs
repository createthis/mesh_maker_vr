using System.Collections.Generic;
using UnityEngine;

namespace MeshEngine.Controller {
    public class PrimitiveCylinderController : PrimitiveBaseController {
        public List<GameObject> vertexInstances;

        private int vertexCountBeforeCylinder;
        private Vector3 center;
        private GameObject centerVertexInstance;
        private int minNumberOfSides = 5;
        private int maxNumberOfSides = 64;
        private int lastNumberOfSides;
        private GameObject lastVertexInstance;

        public override void OnTriggerDown(Transform controller, int controllerIndex) {
            base.OnTriggerDown(controller, controllerIndex);
            triggerCount++;
            if (triggerCount == 1) FirstTrigger(controller, controllerIndex);
            if (triggerCount == 2) SecondTrigger(controller, controllerIndex);
            if (triggerCount == 3) ThirdTrigger(controller, controllerIndex);
        }

        public override void OnTriggerUpdate(Transform controller, int controllerIndex) {
            UpdatePositions(controller, controllerIndex);
        }

        public void ClearVertexInstances() {
            foreach (GameObject vertexInstance in vertexInstances) {
                mesh.vertices.RemoveByVertexInstance(vertexInstance);
                mesh.vertices.DeactivateAndMoveToPool(vertexInstance);
            }
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

        public void DestroyAndRecreateVertices(int numberOfSides, Vector3 controllerPosition, Vector3 controllerUp) {
            ClearVertexInstances();

            Quaternion startRotation = Quaternion.identity;
            Quaternion rotation = Quaternion.LookRotation(controllerPosition - center, controllerUp);

            float radius = Vector3.Distance(center, controllerPosition); // distance between center and controller

            float _2pi = Mathf.PI * 2f;
            int numberVertices = numberOfSides;
            int count = 0;
            float radianOffset = (90.0f * Mathf.Deg2Rad); /* NOTE: I don't understand why this 90 degree offset is necessary, but it is. */

            while (count < numberVertices) {
                float rad = ((float)count / numberVertices * _2pi) + radianOffset;
                Vector3 position = new Vector3(Mathf.Cos(rad) * radius, 0f, Mathf.Sin(rad) * radius) + center;
                position = RotateAround(startRotation, rotation, position, center);
                GameObject vertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
                vertexInstances.Add(vertexInstance);

                if (count > 0) {
                    mesh.triangles.AddTriangleByVertices(centerVertexInstance, vertexInstances[count], vertexInstances[count - 1]);
                }
                count++;
            }
            mesh.triangles.AddTriangleByVertices(centerVertexInstance, vertexInstances[0], vertexInstances[numberVertices - 1]);
            SetSelectable(false);
        }

        public Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c) {
            Vector3 planeNormal = Vector3.Cross(c - a, b - a).normalized * 0.025f;
            return planeNormal;
        }

        public void MoveVertices(int numberOfSides, Vector3 controllerPosition, Vector3 controllerUp) {
            Quaternion startRotation = Quaternion.identity;
            Quaternion rotation = Quaternion.LookRotation(controllerPosition - center, controllerUp);

            float radius = Vector3.Distance(center, controllerPosition); // distance between center and controller

            float _2pi = Mathf.PI * 2f;
            int numberVertices = numberOfSides;
            int count = 0;
            float radianOffset = (90.0f * Mathf.Deg2Rad); /* NOTE: I don't understand why this 90 degree offset is necessary, but it is. */

            while (count < numberVertices) {
                float rad = ((float)count / numberVertices * _2pi) + radianOffset;
                Vector3 position = new Vector3(Mathf.Cos(rad) * radius, 0f, Mathf.Sin(rad) * radius) + center;
                position = RotateAround(startRotation, rotation, position, center);
                GameObject vertexInstance = vertexInstances[count];
                VertexController vertexController = vertexInstance.GetComponent<VertexController>();
                vertexController.UpdateLocalPositionFromWorldPosition(position);
                count++;
            }
        }

        public void UpdateCirclePositions(Transform controller, int controllerIndex) {
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

        public void UpdateSelectionPositions(Transform controller, int controllerIndex) {
            lastVertexInstance.GetComponent<VertexController>().DragUpdate(controller, controllerIndex);
        }

        public void UpdatePositions(Transform controller, int controllerIndex) {
            if (triggerCount == 1) UpdateCirclePositions(controller, controllerIndex);
            if (triggerCount == 2) UpdateSelectionPositions(controller, controllerIndex);
        }

        public void SelectVertices() {
            mesh.selection.SelectVertex(centerVertexInstance);
            foreach (GameObject vertex in vertexInstances) {
                mesh.selection.SelectVertex(vertex);
            }
        }

        public void SetSelectable(bool value) {
            centerVertexInstance.GetComponent<VertexController>().SetSelectable(value);
            foreach (GameObject vertex in vertexInstances) {
                vertex.GetComponent<VertexController>().SetSelectable(value);
            }
            if (lastVertexInstance != null) lastVertexInstance.GetComponent<VertexController>().SetSelectable(value);
        }

        public void FirstTrigger(Transform controller, int controllerIndex) {
            vertexCountBeforeCylinder = mesh.vertices.vertices.Count;
            vertexInstances = new List<GameObject>();
            mesh.selection.Clear();
            center = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, controller.position) : controller.position;
            centerVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(center);
            lastNumberOfSides = 0;
            lastVertexInstance = null;
            transform.position = center;
            SetSelectable(false);
        }

        public void SecondTrigger(Transform controller, int controllerIndex) {
            UpdateCirclePositions(controller, controllerIndex);
            SelectVertices();
            // Avoid nuking the user's copy buffer by creating a local buffer.
            Copy copyManagerFirstEndCap = new Copy(mesh);
            copyManagerFirstEndCap.CopySelection(false);

            mesh.extrusion.ExtrudeSelected();

            // Avoid nuking the user's copy buffer by creating a local buffer.
            Copy copyManagerSecondEndCap = new Copy(mesh);
            copyManagerSecondEndCap.CopySelection();

            copyManagerFirstEndCap.SelectVerticesFromCopy();
            mesh.triangles.FlipNormalsOfSelection();
            mesh.selection.Clear();

            copyManagerSecondEndCap.SelectVerticesFromCopy();

            lastVertexInstance = mesh.selection.selectedVertices[1].vertex.instance;
            SetSelectable(false);
            Debug.Log("SecondTrigger");
            lastVertexInstance.GetComponent<VertexController>().DragStart(controller, controllerIndex);
        }

        public void ThirdTrigger(Transform controller, int controllerIndex) {
            lastVertexInstance.GetComponent<VertexController>().DragEnd(controller, controllerIndex);
            SelectVertices();
            SetSelectable(true);
            Destroy(gameObject);
        }

        public void RemoveAllVertices() {
            for (int i = mesh.vertices.vertices.Count - 1; i >= vertexCountBeforeCylinder; i--) {
                Vertex vertex = mesh.vertices.vertices[i];
                vertex.instance.GetComponent<VertexController>().RemoveVertex();
            }
        }

        public void Cancel() {
            RemoveAllVertices();
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