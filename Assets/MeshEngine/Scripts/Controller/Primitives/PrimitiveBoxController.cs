using UnityEngine;
using System.Linq;

namespace MeshEngine.Controller {
    public class PrimitiveBoxController : PrimitiveBaseController {
        private int vertexCountBeforeBox;
        private GameObject firstVertexInstance;
        private GameObject secondVertexInstance;
        private GameObject thirdVertexInstance;
        private GameObject fourthVertexInstance;
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

        public Vector3 MidPoint(Vector3 firstPosition, Vector3 secondPosition) {
            return Vector3.Lerp(firstPosition, secondPosition, 0.5f);
        }

        public Vector3 SidePosition(Vector3 midPoint, float distance, Vector3 up) {
            return midPoint + Quaternion.AngleAxis(90, up) * up * distance;
        }

        public void UpdatePositions(Transform controller, int controllerIndex) {
            if (triggerCount == 1) UpdatePlanePositions(controller, controllerIndex);
            if (triggerCount == 2) UpdateSelectionPositions(controller, controllerIndex);
        }

        public void UpdateSelectionPositions(Transform controller, int controllerIndex) {
            lastVertexInstance.GetComponent<VertexController>().DragUpdate(controller, controllerIndex);
        }

        public void UpdatePlanePositions(Transform controller, int controllerIndex) {
            Vector3 position = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, controller.position) : controller.position;

            fourthVertexInstance.GetComponent<VertexController>().UpdateLocalPositionFromWorldPosition(position);

            Vector3 midPoint = MidPoint(firstVertexInstance.transform.position, fourthVertexInstance.transform.position);
            float distance = Vector3.Distance(midPoint, fourthVertexInstance.transform.position);
            Vector3 leftPosition = SidePosition(midPoint, distance, -controller.right);
            Vector3 rightPosition = SidePosition(midPoint, distance, controller.right);
            Vector3 snappedLeftPosition = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, leftPosition) : leftPosition;
            Vector3 snappedRightPosition = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, rightPosition) : rightPosition;

            //Debug.Log("UpdatePositions first=" + firstVertexInstance.transform.position + ",fourth=" + fourthVertexInstance.transform.position + ",midPoint=" + midPoint + ",distance=" + distance + ",left=" + leftPosition + ",right=" + rightPosition);
            secondVertexInstance.GetComponent<VertexController>().UpdateLocalPositionFromWorldPosition(snappedLeftPosition);
            thirdVertexInstance.GetComponent<VertexController>().UpdateLocalPositionFromWorldPosition(snappedRightPosition);
        }

        public void CreateTriangles(bool up = true) {
            if (up) {
                mesh.triangles.AddTriangleVertex(fourthVertexInstance);
                mesh.triangles.AddTriangleVertex(secondVertexInstance);
                mesh.triangles.AddTriangleVertex(firstVertexInstance);
                mesh.triangles.AddTriangleVertex(thirdVertexInstance);
                mesh.triangles.AddTriangleVertex(fourthVertexInstance);
                mesh.triangles.AddTriangleVertex(firstVertexInstance);
            } else {
                mesh.triangles.AddTriangleVertex(firstVertexInstance);
                mesh.triangles.AddTriangleVertex(secondVertexInstance);
                mesh.triangles.AddTriangleVertex(fourthVertexInstance);
                mesh.triangles.AddTriangleVertex(firstVertexInstance);
                mesh.triangles.AddTriangleVertex(fourthVertexInstance);
                mesh.triangles.AddTriangleVertex(thirdVertexInstance);
            }
        }

        public void SelectVertices() {
            mesh.selection.SelectVertex(firstVertexInstance);
            mesh.selection.SelectVertex(secondVertexInstance);
            mesh.selection.SelectVertex(thirdVertexInstance);
            mesh.selection.SelectVertex(fourthVertexInstance);
        }

        public void SetSelectable(bool value) {
            firstVertexInstance.GetComponent<VertexController>().SetSelectable(value);
            secondVertexInstance.GetComponent<VertexController>().SetSelectable(value);
            thirdVertexInstance.GetComponent<VertexController>().SetSelectable(value);
            fourthVertexInstance.GetComponent<VertexController>().SetSelectable(value);
            if (lastVertexInstance != null) lastVertexInstance.GetComponent<VertexController>().SetSelectable(value);
        }

        public void FirstTrigger(Transform controller, int controllerIndex) {
            vertexCountBeforeBox = mesh.vertices.vertices.Count;
            mesh.selection.Clear();
            Vector3 position = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, controller.position) : controller.position;

            firstVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            secondVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            thirdVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            fourthVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            SetSelectable(false);
            lastVertexInstance = null;

            CreateTriangles();
            SelectVertices();
        }

        public void SecondTrigger(Transform controller, int controllerIndex) {
            UpdatePlanePositions(controller, controllerIndex);
            mesh.extrusion.ExtrudeSelected();
            // Avoid nuking the user's copy buffer by creating a local buffer.
            Copy copy = new Copy(mesh);
            copy.CopySelection();
            mesh.selection.Clear();
            SelectVertices();
            mesh.triangles.FlipNormalsOfSelection();
            mesh.selection.Clear();
            copy.SelectVerticesFromCopy();
            lastVertexInstance = mesh.selection.selectedVertices.Last().vertex.instance;
            SetSelectable(false);

            //Debug.Log("onSecondTrigger selectedVertices.Count="+ meshController.selectionManager.selectedVertices.Count + ",fifthVertexInstance=" + fifthVertexInstance.GetInstanceID());
            lastVertexInstance.GetComponent<VertexController>().DragStart(controller, controllerIndex);
        }

        public void ThirdTrigger(Transform controller, int controllerIndex) {
            //Debug.Log("onThirdTrigger");
            lastVertexInstance.GetComponent<VertexController>().DragEnd(controller, controllerIndex);
            SelectVertices();
            SetSelectable(true);
            Destroy(gameObject);
        }

        public void RemoveAllVertices() {
            for (int i = mesh.vertices.vertices.Count - 1; i >= vertexCountBeforeBox; i--) {
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