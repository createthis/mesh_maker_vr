using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CreateThis.VR.UI.Interact;
using MeshEngine.Controller;

/* Some rules to keep in mind:
 * 1. Unity3d GameObject.GetInstanceID() is a GUID and is therefore acceptable for comparisons and search operations.
 * 2. A VerticesManager.Vertex.instance is NOT required (Face Delete mode and Flip Normal mode, for example), so some other GUID is necessary.
 * 3. Each VerticesManager.Vertex.instance has only ONE VerticesManager.Vertex (1:1 relationship).
 * 4. VerticesManager.Vertex.position is NOT unique.
 */

namespace MeshEngine {
    public class Vertices {
        public List<Vertex> vertices { get; private set; }
        public Mesh mesh;
        public bool hideVertices;
        public bool verticesChanged;

        private bool vertexInstancesCreated;
        private List<GameObject> vertexInstancesPool;
        private bool vertexInstancesSelectable;
        private Dictionary<System.Guid, int> vertexIndex;
        private Dictionary<int, int> vertexInstanceIndex;

        public void DeactivateAndMoveToPool(GameObject vertexGameObject) {
            vertexGameObject.SetActive(false);
            vertexGameObject.transform.parent = null;
            vertexInstancesPool.Add(vertexGameObject);
        }

        public GameObject InstantiateOrGetFromPool(Vector3 position, Quaternion rotation) {
            if (vertexInstancesPool.Count > 0) {
                int index = vertexInstancesPool.Count - 1;
                GameObject vertexGameObject = vertexInstancesPool[index];
                vertexInstancesPool.RemoveAt(index);
                vertexGameObject.SetActive(true);
                vertexGameObject.transform.position = position;
                vertexGameObject.transform.rotation = rotation;
                return vertexGameObject;
            } else {
                return GameObject.Instantiate(mesh.vertexPrefab, position, rotation);
            }
        }

        public Vertices(Mesh mesh) {
            this.mesh = mesh;
            vertices = new List<Vertex>();
            vertexInstancesCreated = false;
            vertexInstancesPool = new List<GameObject>();
            verticesChanged = false;
            vertexIndex = new Dictionary<System.Guid, int>();
            vertexInstanceIndex = new Dictionary<int, int>();
        }

        public string VerticesToString(Vector3[] vertices = null) {
            List<string> result = new List<string>();

            foreach (Vector3 vertex in vertices) {
                result.Add(vertex.ToString());
            }
            return string.Join(",", result.ToArray());
        }

        public List<Vector3> GetVertexPositionsOfUnfinishedTriangles() {
            int remainder = mesh.triangles.triangles.Count % 3;
            //Debug.Log("GetVertexPositionsOfUnfinishedTriangles remainder=" + remainder + ",instance.newTriangles.Count=" + meshController.trianglesManager.triangles.Count);
            if (remainder == 0) return new List<Vector3>();
            List<int> triangleIndices = mesh.triangles.triangles.GetRange(mesh.triangles.triangles.Count - remainder, remainder);
            List<Vector3> vertexPositions = new List<Vector3>();
            foreach (int index in triangleIndices) {
                vertexPositions.Add(vertices[index].position);
            }
            //Debug.Log("GetVertexPositionsOfUnfinishedTriangles remainder=" + remainder + ",triangleIndices.Count=" + triangleIndices.Count + ",vertexPositions.Count=" + vertexPositions.Count);
            return vertexPositions;
        }

        public List<Vertex> ListVertexOfVector3Array(List<Vector3> positions, List<Vector2> uvs) {
            List<Vertex> myVertices = new List<Vertex>();
            for (int i = 0; i < positions.Count; i++) {
                Vector3 position = positions[i];
                Vertex vertex = new Vertex();
                vertex.position = position;
                if (i < uvs.Count) {
                    vertex.uv = uvs[i];
                }
                myVertices.Add(vertex);
            }
            return myVertices;
        }

        public GameObject CreateAndAddVertexInstanceByWorldPosition(Vector3 worldPosition) {
            Vector3 localPosition = mesh.transform.InverseTransformPoint(worldPosition);
            GameObject vertexInstance = CreateVertexInstanceByLocalPosition(localPosition);
            AddVertexWithInstance(localPosition, vertexInstance);
            return vertexInstance;
        }

        public GameObject CreateVertexInstanceByLocalPosition(Vector3 localPosition) {
            Vector3 position = mesh.transform.TransformPoint(localPosition);

            //Debug.Log("MeshController#CreateVertices localPosition="+localPosition+",position=" + position+",this.transform.position=" + this.transform.position);
            GameObject vertexInstance = InstantiateOrGetFromPool(position, mesh.transform.rotation);
            vertexInstance.transform.parent = mesh.transform;
            vertexInstance.GetComponent<Selectable>().Initialize();
            if (hideVertices) vertexInstance.SetActive(false);
            VertexController vertexController = vertexInstance.GetComponent<VertexController>();
            vertexController.SetSelectable(vertexInstancesSelectable);
            vertexController.SetStickySelected(false);
            vertexController.CreateFromLocalPosition(localPosition, mesh, false);
            return vertexInstance;
        }

        public void CreateVertexInstances(bool selectUnfinishedTriangles = true) {
            if (vertexInstancesCreated) return;
            vertexInstanceIndex.Clear();
            List<Vector3> positions = GetVertexPositionsOfUnfinishedTriangles();
            for (int i = 0; i < vertices.Count; i++) {
                Vector3 localPosition = vertices[i].position;
                vertices[i].instance = CreateVertexInstanceByLocalPosition(localPosition);
                vertexInstanceIndex.Add(vertices[i].instance.GetInstanceID(), i);
                if (selectUnfinishedTriangles && positions.Contains(localPosition)) {
                    mesh.selection.SelectVertex(vertices[i].instance);
                }
            }
            vertexInstancesCreated = true;
        }

        public void Clear() {
            DeleteVertexInstances();
            vertices.Clear();
            vertexIndex.Clear();
            vertexInstanceIndex.Clear();
            verticesChanged = true;
            mesh.persistence.changedSinceLastSave = true;
        }

        public void DeleteVertexInstances() {
            GameObject[] vertices = GameObject.FindGameObjectsWithTag("Vertex");
            foreach (GameObject vertex in vertices) {
                DeactivateAndMoveToPool(vertex);
            }
            mesh.selection.ClearSelectedVertices();
            vertexInstancesCreated = false;
        }

        public Vector3[] Vector3ArrayOfVertices(List<Vertex> myVertices) {
            return myVertices.Select(x => x.position).ToArray();
        }

        public Vector3[] GetVerticesArray() {
            return Vector3ArrayOfVertices(vertices);
        }

        private void BuildMeshVerticesAndUVs() {
            Vector3[] meshVertices = new Vector3[vertices.Count];
            Vector2[] meshUVs = new Vector2[vertices.Count];
            for (int i = 0; i < vertices.Count; i++) {
                meshVertices[i] = vertices[i].position;
                meshUVs[i] = vertices[i].uv;
            }
            mesh.uMesh.vertices = meshVertices;
            mesh.uMesh.uv = meshUVs;
        }

        public bool BuildVertices(bool rebuildWireframe = true) {
            if (!verticesChanged) return false;
            mesh.uMesh.Clear();
            BuildMeshVerticesAndUVs();
            mesh.triangles.trianglesChanged = true;
            verticesChanged = false;
            return true;
        }

        public int AddVertexWithInstance(Vector3 position, GameObject vertexInstance, bool buildVertices = true) {
            Vertex vertex = new Vertex();
            vertex.position = position;
            vertex.instance = vertexInstance;

            vertices.Add(vertex);
            vertexIndex.Add(vertex.ID, vertices.Count - 1);
            vertexInstanceIndex.Add(vertex.instance.GetInstanceID(), vertices.Count - 1);
            int index = vertices.Count - 1;
            verticesChanged = true;
            mesh.persistence.changedSinceLastSave = true;
            return index;
        }

        public int IndexOfInstance(GameObject vertexInstance) {
            if (vertexInstanceIndex.ContainsKey(vertexInstance.GetInstanceID())) return vertexInstanceIndex[vertexInstance.GetInstanceID()];
            return -1;
        }

        public Vertex VertexOfInstance(GameObject vertexInstance) {
            int index = IndexOfInstance(vertexInstance);
            if (index == -1) {
                Debug.Log("VertexOfInstance index=-1, vertexInstance.GetInstanceID()=" + vertexInstance.GetInstanceID());
                return null;
            }
            return vertices[index];
        }

        public bool SameVertex(Vertex a, Vertex b) {
            if (a.ID == b.ID) return true;
            return false;
        }

        public int IndexOfVertex(Vertex vertex) {
            if (vertexIndex.ContainsKey(vertex.ID)) return vertexIndex[vertex.ID];
            return -1;
        }

        public void ReplaceVertex(GameObject vertexInstance, Vector3 newPosition) {
            int index = IndexOfInstance(vertexInstance);
            if (index == -1) Debug.Log("ReplaceVertex index=-1, vertexInstance.GetInstanceID()=" + vertexInstance.GetInstanceID() + ", newPosition=" + newPosition);
            Vertex vertex = vertices[index];
            ReplaceVertexPositionAt(index, newPosition);
            vertex.CallOnUpdateVertex();
        }

        public void ReplaceVertexPositionAt(int index, Vector3 position) {
            //Debug.Log("MeshController: Replaced vertex[" + index + "]=" + vertex + " instance.mesh.vertices=" + instance.VerticesToString());
            Vertex vertex = vertices[index];
            vertices[index].position = position;
            verticesChanged = true;
            mesh.persistence.changedSinceLastSave = true;
            //meshController.wireframe.UpdateVertex(vertex, position);
        }

        public void TruncateToCount(int count) {
            for (int i = vertices.Count - 1; i >= count; i--) {
                vertexIndex.Remove(vertices[i].ID);
                vertexInstanceIndex.Remove(vertices[i].instance.GetInstanceID());
                vertices.RemoveAt(i);
            }
            verticesChanged = true;
            mesh.persistence.changedSinceLastSave = true;
        }

        public void SetVertices(List<Vertex> myVertices) {
            vertices = myVertices;
            RebuildVertexIndex();
        }

        public void RebuildVertexIndex() {
            vertexIndex.Clear();
            vertexInstanceIndex.Clear();
            for (int i = 0; i < vertices.Count; i++) {
                Vertex vertex = vertices[i];
                vertexIndex.Add(vertex.ID, i);
                if (vertex.instance) {
                    vertexInstanceIndex.Add(vertex.instance.GetInstanceID(), i);
                }
            }
        }

        public void RemoveVertex(Vertex vertex) {
            int index = IndexOfVertex(vertex);
            //Debug.Log("MeshController: Remove index=" + index + ",ID=" + vertex.instance.GetInstanceID());
            if (index != -1) {
                mesh.triangles.RemoveTrianglesWithVertexIndex(index);
                mesh.selection.RemoveVertexByIndex(index);
                mesh.selection.RemovedVerticesManagerVertexAtIndex(index);
            }
            vertices.Remove(vertex);
            RebuildVertexIndex();
            verticesChanged = true;
            mesh.persistence.changedSinceLastSave = true;
        }

        public void RemoveByVertexInstance(GameObject vertexInstance) {
            //Debug.Log("RemoveByVertexInstance ID="+vertexInstance.GetInstanceID());
            RemoveVertex(VertexOfInstance(vertexInstance));
        }

        public void MergeFirstVertexToSecond(Vertex first, Vertex second) {
            mesh.triangles.MoveAllTrianglesFromFirstToSecondVertex(first, second);
            RemoveVertex(first);
            if (first.instance != null) DeactivateAndMoveToPool(first.instance);
            mesh.triangles.DeleteTriangleInstances();
            mesh.triangles.CreateTriangleInstances();
        }

        public void MergeSelected() {
            List<SelectedVertex> selectedVertices = mesh.selection.selectedVertices;
            if (selectedVertices.Count != 2) return;
            MergeFirstVertexToSecond(selectedVertices[0].vertex, selectedVertices[1].vertex);
            mesh.selection.Clear();
        }

        public void UpdateAllVertexInstancesSelectable() {
            foreach (Vertex vertex in vertices) {
                GameObject vertexInstance = vertex.instance;
                VertexController vertexController = vertexInstance.GetComponent<VertexController>();
                vertexController.SetSelectable(vertexInstancesSelectable);
            }
        }

        public void SetVertexInstancesSelectable(bool value) {
            if (vertexInstancesSelectable == value) return;
            vertexInstancesSelectable = value;
            UpdateAllVertexInstancesSelectable();
        }
    }
}