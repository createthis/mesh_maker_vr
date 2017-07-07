using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CreateThis.VR.UI.Interact;

namespace MeshEngine.Controller {
    public class BoxSelectionController : PrimitiveBaseController {
        public Mesh realMesh;
        public GameObject meshGameObject;

        private GameObject firstVertexInstance;
        private GameObject secondVertexInstance;
        private GameObject thirdVertexInstance;
        private GameObject fourthVertexInstance;
        private GameObject lastVertexInstance;
        private UnityEngine.Mesh collisionMesh;
        private Vector3 validationPoint;
        private int selectedVerticesIndexBeforeBoxSelect;

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
            UpdateMeshCollider();
        }

        public int[] TransposeAllTrianglesUsingVerticesRange(int startVertexIndex, int endVertexIndex) {
            List<int> selectedTriangles = new List<int>();
            List<int> triangles = mesh.triangles.triangles;
            for (int i = 0; i + 2 < triangles.Count; i += 3) {
                if (triangles[i] >= startVertexIndex && triangles[i] <= endVertexIndex &&
                    triangles[i + 1] >= startVertexIndex && triangles[i + 1] <= endVertexIndex &&
                    triangles[i + 2] >= startVertexIndex && triangles[i + 2] <= endVertexIndex) {

                    selectedTriangles.Add(triangles[i] - startVertexIndex);
                    selectedTriangles.Add(triangles[i + 1] - startVertexIndex);
                    selectedTriangles.Add(triangles[i + 2] - startVertexIndex);
                }
            }
            return selectedTriangles.ToArray();
        }

        public void UpdateMeshCollider() {
            collisionMesh = new UnityEngine.Mesh();
            int firstVertexIndex = mesh.vertices.IndexOfInstance(firstVertexInstance);
            int numberToTake = mesh.vertices.vertices.Count - firstVertexIndex;
            List<Vertex> vertices = new List<Vertex>(mesh.vertices.vertices.Skip(firstVertexIndex).Take(numberToTake).ToArray());

            collisionMesh.vertices = mesh.vertices.Vector3ArrayOfVertices(vertices);
            collisionMesh.triangles = TransposeAllTrianglesUsingVerticesRange(firstVertexIndex, mesh.vertices.vertices.Count - 1);
            
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();
            if (collisionMesh.vertices.Length == 8) {
                collisionMesh.RecalculateBounds();
                meshCollider.sharedMesh = collisionMesh;
            }
        }

        public void UpdateTransformFromPlane() {
            transform.position = mesh.transform.position;
            transform.rotation = mesh.transform.rotation;

            UpdateMeshCollider();
        }

        public Vector3 CalculateNormal() {
            Vector3 planeNormal = Vector3.Cross(thirdVertexInstance.transform.position - firstVertexInstance.transform.position, secondVertexInstance.transform.position - firstVertexInstance.transform.position).normalized * 0.025f;
            return planeNormal;
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

            secondVertexInstance.GetComponent<VertexController>().UpdateLocalPositionFromWorldPosition(snappedLeftPosition);
            thirdVertexInstance.GetComponent<VertexController>().UpdateLocalPositionFromWorldPosition(snappedRightPosition);

            UpdateTransformFromPlane();
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
            if (lastVertexInstance) lastVertexInstance.GetComponent<VertexController>().SetSelectable(value);
        }

        public void CacheSelectedVertices() {
            selectedVerticesIndexBeforeBoxSelect = realMesh.selection.selectedVertices.Count - 1;
        }
        
        public void FirstTrigger(Transform controller, int controllerIndex) {
            CacheSelectedVertices();
            mesh.selection.Clear();
            Vector3 position = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, controller.position) : controller.position;

            firstVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            secondVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            thirdVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            fourthVertexInstance = mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
            lastVertexInstance = null;

            CreateTriangles();
            SelectVertices();
            SetSelectable(false);
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
            lastVertexInstance.GetComponent<VertexController>().DragStart(controller, controllerIndex);
            SetSelectable(false);
        }

        public void ThirdTrigger(Transform controller, int controllerIndex) {
            lastVertexInstance.GetComponent<VertexController>().DragEnd(controller, controllerIndex);
            SetSelectable(true);
            Remove();
        }

        public void Remove() {
            Destroy(mesh.gameObject);
            Destroy(gameObject);
        }

        public void CreateMesh() {
            meshGameObject = new GameObject();
            meshGameObject.name = "Box Select";
            meshGameObject.AddComponent<MeshFilter>();
            meshGameObject.AddComponent<MeshRenderer>();
            meshGameObject.AddComponent<Mesh>();
            meshGameObject.AddComponent<BoxCollider>();
            Selectable realMeshSelectable = realMesh.GetComponent<Selectable>();
            Selectable selectable = meshGameObject.AddComponent<Selectable>();
            selectable.unselectedMaterials = new UnityEngine.Material[] { Meshes.unselectedMaterial };
            selectable.highlightMaterial = realMeshSelectable.highlightMaterial;
            selectable.outlineMaterial = realMeshSelectable.outlineMaterial;
            meshGameObject.transform.position = realMesh.gameObject.transform.position;
            meshGameObject.transform.rotation = realMesh.gameObject.transform.rotation;
            mesh = meshGameObject.GetComponent<Mesh>();
            mesh.vertexPrefab = realMesh.vertexPrefab;
            mesh.trianglePrefab = realMesh.trianglePrefab;
            mesh.load = false;
            MeshRenderer meshRenderer = meshGameObject.GetComponent<MeshRenderer>();
            meshRenderer.materials = realMesh.GetComponent<MeshRenderer>().materials;
            mesh.Initialize();
            mesh.SetRenderOptions(false, true, false);
            mesh.triangles.autoCreateTriangleObjects = true;
            mesh.triangles.hideTriangles = true;
            mesh.vertices.hideVertices = true;
            mesh.triangles.log = false;
        }        

        protected override void Update() {
            base.Update();
            validationPoint = transform.position + transform.forward * 10.0f; // some point outside the mesh
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();
        }


        private void OnTriggerEnter(Collider collider) {
            if (collider.GetComponent<VertexController>() && collider.tag == "Vertex") {
                realMesh.selection.SelectVertex(collider.gameObject);
            }
        }

        private void OnTriggerExit(Collider collider) {
            if (collider.GetComponent<VertexController>() && collider.tag == "Vertex") {
                if (!StillInside(collider.transform.position)) {
                    realMesh.selection.DeselectVertex(collider.gameObject);


                    List<SelectedVertex> selectedVerticesToUnselect = new List<SelectedVertex>();
                    int numberToTake = realMesh.selection.selectedVertices.Count - selectedVerticesIndexBeforeBoxSelect;
                    List<SelectedVertex> verticesSelectedDuringBoxSelect = new List<SelectedVertex>(realMesh.selection.selectedVertices.Skip(selectedVerticesIndexBeforeBoxSelect + 1).Take(numberToTake).ToArray());
                    // This foreach is necessary because the MeshCollider doesn't always call OnTriggerExit for each collider inside it, especially when the collider is changing shape quickly.
                    foreach (SelectedVertex selectedVertex in verticesSelectedDuringBoxSelect) {
                        if (!StillInside(selectedVertex.vertex.instance.transform.position)) {
                            selectedVerticesToUnselect.Add(selectedVertex);
                        }
                    }
                    foreach (SelectedVertex selectedVertex in selectedVerticesToUnselect) {
                        realMesh.selection.DeselectVertex(selectedVertex.vertex.instance.gameObject);
                    }

                }
            }
        }

        private bool StillInside(Vector3 target) {
            RaycastHit[] hits;

            Vector3 dir = target - validationPoint;
            float dist = dir.magnitude;
            //Debug.Log("Validating if the contact has really exited... Raycast with distance: " + dist);

            hits = Physics.RaycastAll(validationPoint, dir.normalized, dist);
            //Debug.DrawRay(validationPoint, dir.normalized * dist, Color.white, 40.0f);
            foreach (RaycastHit hit in hits) {
                if (hit.collider.gameObject == gameObject) {
                    //Debug.Log("The contact seems to still be inside.");
                    return true;
                }
            }

            //Debug.Log("The contact seems to have left");
            return false;
        }
    }
}