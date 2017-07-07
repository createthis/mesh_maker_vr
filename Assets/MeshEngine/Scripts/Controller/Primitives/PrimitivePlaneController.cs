using UnityEngine;

namespace MeshEngine.Controller {
    public class PrimitivePlaneController : PrimitiveBaseController {
        private GameObject firstVertexInstance;
        private GameObject secondVertexInstance;
        private GameObject thirdVertexInstance;
        private GameObject fourthVertexInstance;

        public override void OnTriggerDown(Transform controller, int controllerIndex) {
            base.OnTriggerDown(controller, controllerIndex);
            triggerCount++;
            if (triggerCount == 1) OnFirstTrigger(controller, controllerIndex);
            if (triggerCount == 2) OnSecondTrigger(controller, controllerIndex);
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
        }

        public void OnFirstTrigger(Transform controller, int controllerIndex) {
            this.controllerIndex = controllerIndex;
            mesh.selection.Clear();
            Vector3 position = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, controller.position) : controller.position;

            firstVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            secondVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            thirdVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            fourthVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);

            SetSelectable(false);

            CreateTriangles();
            SelectVertices();
        }

        public void OnSecondTrigger(Transform controller, int controllerIndex) {
            UpdatePositions(controller, controllerIndex);
            SetSelectable(true);
            Destroy(gameObject);
        }

        public void Cancel() {
            firstVertexInstance.GetComponent<VertexController>().RemoveVertex();
            secondVertexInstance.GetComponent<VertexController>().RemoveVertex();
            thirdVertexInstance.GetComponent<VertexController>().RemoveVertex();
            fourthVertexInstance.GetComponent<VertexController>().RemoveVertex();
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