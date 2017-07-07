using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MeshEngine.Controller;

namespace MeshEngine {
    public class Selection {
        public Mesh mesh;
        public List<SelectedVertex> selectedVertices;
        public List<Triangle> selectedTriangles;
        private Dictionary<int, int> selectedVertexIndexByVertexIndex;

        public Selection(Mesh mesh) {
            this.mesh = mesh;
            selectedVertices = new List<SelectedVertex>();
            selectedTriangles = new List<Triangle>();
            selectedVertexIndexByVertexIndex = new Dictionary<int, int>();
        }

        public string SelectedVerticesString() {
            List<string> result = new List<string>();
            for (int i = 0; i < selectedVertices.Count; i++) {
                SelectedVertex x = selectedVertices[i];
                result.Add("(i=" + i + ",index:" + x.index + ",ID:" + x.vertex.instance.GetInstanceID().ToString() + ")");
            }
            return string.Join(",", result.ToArray());
        }

        public Vector3[] GetVerticesArray() {
            return selectedVertices.Select(x => x.vertex.position).ToArray();
        }

        public int[] GetTrianglesArray() {
            return selectedTriangles.Select(x => x.index).ToArray();
        }

        public void Clear() {
            //Debug.Log("SelectionManager#Clear");
            ClearSelectedTriangles();
            ClearSelectedVertices();
        }

        public void BroadcastDragStart(Transform controller, int controllerIndex, GameObject instanceOfOrigin, bool forceDrag = false) {
            //Debug.Log("BroadcastDragStart selectedVertices= " + meshController.verticesManager.VerticesToString(GetVerticesArray()));
            foreach (SelectedVertex selectedVertex in selectedVertices) {
                if (selectedVertex.vertex.instance != instanceOfOrigin) {
                    selectedVertex.vertex.instance.GetComponent<VertexController>().SelectionDragStart(controller, controllerIndex, instanceOfOrigin, false, forceDrag);
                }
            }
        }

        public void BroadcastDragUpdate(Transform controller, int controllerIndex, GameObject instanceOfOrigin) {
            foreach (SelectedVertex selectedVertex in selectedVertices) {
                if (selectedVertex.vertex.instance != instanceOfOrigin) {
                    selectedVertex.vertex.instance.GetComponent<VertexController>().SelectionDragUpdate(controller, controllerIndex, instanceOfOrigin, false);
                }
            }
        }

        public void BroadcastDragEnd(Transform controller, int controllerIndex, GameObject instanceOfOrigin) {
            foreach (SelectedVertex selectedVertex in selectedVertices) {
                if (selectedVertex.vertex.instance != instanceOfOrigin) {
                    selectedVertex.vertex.instance.GetComponent<VertexController>().SelectionDragEnd(controller, controllerIndex, instanceOfOrigin, false);
                }
            }
        }

        public void SelectionTransformDirection(Vector3 directionVector) {
            GameObject mockController = new GameObject();
            mockController.transform.position = Vector3.zero;

            GameObject instanceOfOrigin = selectedVertices.First().vertex.instance;
            VertexController vertexController = instanceOfOrigin.GetComponent<VertexController>();

            vertexController.SelectionDragStart(mockController.transform, 0, instanceOfOrigin, false, true);
            BroadcastDragStart(mockController.transform, 0, instanceOfOrigin, true);

            mockController.transform.position = mockController.transform.position + directionVector;

            vertexController.SelectionDragUpdate(mockController.transform, 0, instanceOfOrigin, false);
            BroadcastDragUpdate(mockController.transform, 0, instanceOfOrigin);
            vertexController.SelectionDragEnd(mockController.transform, 0, instanceOfOrigin, false);
            BroadcastDragEnd(mockController.transform, 0, instanceOfOrigin);
        }

        public void RebuildSelectedVertexIndexByVertexIndex() {
            selectedVertexIndexByVertexIndex.Clear();
            for (int i = 0; i < selectedVertices.Count; i++) {
                SelectedVertex selectedVertex = selectedVertices[i];
                selectedVertexIndexByVertexIndex.Add(selectedVertex.index, i);
            }
        }

        public void RemovedVerticesManagerVertexAtIndex(int index) {
            selectedVertexIndexByVertexIndex.Clear();
            for (int i = 0; i < selectedVertices.Count; i++) {
                SelectedVertex selectedVertex = selectedVertices[i];
                if (selectedVertex.index > index) {
                    //Debug.Log("RemovedVerticesManagerVertexAtIndex minus minus index=" + index + ",i="+i+",selectedVertex.index=" + selectedVertex.index);
                    selectedVertex.index--;
                }
                //Debug.Log("RemovedVerticesManagerVertexAtIndex Adding index=" + index + ",i=" + i + ",selectedVertex.index=" + selectedVertex.index);
                selectedVertexIndexByVertexIndex.Add(selectedVertex.index, i);
            }
        }

        public void RemoveVertexByIndex(int index) {
            if (!selectedVertexIndexByVertexIndex.ContainsKey(index)) return;
            int selectedVertexIndex = selectedVertexIndexByVertexIndex[index];
            //Debug.Log("RemoveVertexByIndex index=" + index + ",selectedVertexIndex=" + selectedVertexIndex);
            selectedVertexIndexByVertexIndex.Remove(index);
            selectedVertices.RemoveAt(selectedVertexIndex);
        }

        public void ClearSelectedTriangles() {
            foreach (Triangle triangle in selectedTriangles) {
                triangle.instance.GetComponent<TriangleController>().SetStickySelected(false);
            }
            selectedTriangles.Clear();
        }

        public void ClearSelectedVertices() {
            foreach (SelectedVertex selectedVertex in selectedVertices) {
                selectedVertex.vertex.instance.GetComponent<VertexController>().SetStickySelected(false);
            }
            selectedVertices.Clear();
            selectedVertexIndexByVertexIndex.Clear();
        }

        public void DeleteSelectedTriangles() {
            while (selectedTriangles.Count > 0) {
                Triangle triangle = selectedTriangles[0];
                triangle.instance.GetComponent<TriangleController>().RemoveTriangle();
                selectedTriangles.Remove(triangle);
            }
        }

        public void DeleteSelectedVertices() {
            //Debug.Log("DeleteSelectedVertices before selectedVertices.Count=" + selectedVertices.Count + ",selectedVertices="+ SelectedVerticesString());
            while (selectedVertices.Count > 0) {
                SelectedVertex selectedVertex = selectedVertices[0];
                //Debug.Log("DeleteSelectedVertices index="+selectedVertex.index+",ID=" + selectedVertex.vertex.instance.GetInstanceID());
                selectedVertex.vertex.instance.GetComponent<VertexController>().RemoveVertex();
            }
            //Debug.Log("DeleteSelectedVertices after selectedVertices.Count=" + selectedVertices.Count);
            selectedVertexIndexByVertexIndex.Clear();
        }

        public bool SelectedVerticesContainIndex(int index) {
            return selectedVertexIndexByVertexIndex.ContainsKey(index);
        }

        public bool VertexSelected(Vertex vertex) {
            int vertexIndex = mesh.vertices.IndexOfVertex(vertex);
            return SelectedVerticesContainIndex(vertexIndex);
        }

        public bool VerticesSelected(Vertex[] vertices) {
            foreach (Vertex vertex in vertices) {
                if (!VertexSelected(vertex)) return false;
            }
            return true;
        }

        public void StickySelectTriangles() {
            List<int> triangles = mesh.triangles.triangles;
            //Debug.Log("StickySelectTriangles triangles.Count=" + triangles.Count);
            for (int i = 0; i + 2 < triangles.Count; i += 3) {
                if (SelectedVerticesContainIndex(triangles[i]) &&
                    SelectedVerticesContainIndex(triangles[i + 1]) &&
                    SelectedVerticesContainIndex(triangles[i + 2])) {
                    Triangle triangle = mesh.triangles.TriangleByIndex(i);
                    //Debug.Log("StickySelectTriangles sticky i=" + i);
                    if (triangle != null) {
                        triangle.instance.GetComponent<TriangleController>().SetStickySelected(true);
                        selectedTriangles.Add(triangle);
                    }
                }
            }
        }

        public void DeleteSelected() {
            DeleteSelectedTriangles();
            DeleteSelectedVertices();
        }

        public void FillSelected() {
            foreach (Triangle triangle in selectedTriangles) {
                MaterialUtils.FillTriangle(mesh, triangle);
            }
        }

        public void SelectVertex(GameObject vertexInstance) {
            SelectedVertex selectedVertex = SelectedVertexByInstance(vertexInstance);
            if (selectedVertex != null) return; // already selected
            selectedVertex = new SelectedVertex();
            selectedVertex.index = mesh.vertices.IndexOfInstance(vertexInstance);
            if (selectedVertex.index == -1) {
                Debug.Log("SelectVertex index=-1, vertexInstance.GetInstanceID=" + vertexInstance.GetInstanceID() + ",position=" + vertexInstance.GetComponent<VertexController>().lastPosition);
                return;
            }
            selectedVertex.vertex = mesh.vertices.vertices[selectedVertex.index];
            selectedVertices.Add(selectedVertex);
            selectedVertexIndexByVertexIndex.Add(selectedVertex.index, selectedVertices.Count - 1);
            //Debug.Log("SelectVertex selectedVertex.index=" + selectedVertex.index + ",selectedVertices.Count-1=" + (selectedVertices.Count - 1));
            vertexInstance.GetComponent<VertexController>().SetStickySelected(true);
            selectedTriangles.Clear();
            StickySelectTriangles();
        }

        public void SelectVertices(Vertex[] vertices) {
            foreach (Vertex vertex in vertices) {
                SelectVertex(vertex.instance);
            }
        }

        public void SelectTriangleByVertices(Vertex[] vertices) {
            Triangle triangle = mesh.triangles.FindTriangleByVertices(vertices);
            if (triangle != null) {
                triangle.instance.GetComponent<TriangleController>().SetStickySelected(true);
                selectedTriangles.Add(triangle);
            }
        }

        public void DeselectTriangleByVertices(Vertex[] vertices) {
            Triangle triangle = mesh.triangles.FindTriangleByVertices(vertices);
            if (triangle != null) {
                triangle.instance.GetComponent<TriangleController>().SetStickySelected(false);
                selectedTriangles.Remove(triangle);
            }
        }

        public bool TriangleSelectedByVertices(Vertex[] vertices) {
            Triangle triangle = mesh.triangles.FindTriangleByVertices(vertices);
            if (triangle != null) {
                triangle.instance.GetComponent<TriangleController>().SetStickySelected(false);
                return selectedTriangles.Contains(triangle);
            }
            return false;
        }

        public SelectedVertex SelectedVertexByInstance(GameObject vertexInstance) {
            foreach (SelectedVertex selectedVertex in selectedVertices) {
                if (selectedVertex.vertex.instance == vertexInstance) return selectedVertex;
            }
            return null;
        }

        public void DeselectVertex(GameObject vertexInstance) {
            SelectedVertex selectedVertex = SelectedVertexByInstance(vertexInstance);
            if (selectedVertex == null) return; // not selected
            //Debug.Log("UnSelectVertex selectVertex.index=" + selectedVertex.index + ",selectedVertexIndexByVertexIndex[selectedVertex.index]=" + selectedVertexIndexByVertexIndex[selectedVertex.index]);
            int selectedVertexIndex = selectedVertexIndexByVertexIndex[selectedVertex.index];
            bool isLastItem = selectedVertexIndex == selectedVertices.Count - 1;
            selectedVertices.Remove(selectedVertex);
            selectedVertexIndexByVertexIndex.Remove(selectedVertex.index);
            if (!isLastItem) RebuildSelectedVertexIndexByVertexIndex();
            vertexInstance.GetComponent<VertexController>().SetStickySelected(false);
            ClearSelectedTriangles();
            StickySelectTriangles();
        }

        public void DeselectVertices(Vertex[] vertices) {
            foreach (Vertex vertex in vertices) {
                DeselectVertex(vertex.instance);
            }
        }
    }
}