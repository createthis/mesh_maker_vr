using UnityEngine;
using System.Collections.Generic;
using CreateThis.VR.UI.Interact;
using MeshEngine.Controller;

namespace MeshEngine {
    public class Triangles {
        class TriangleToRemove {
            public int[] triangle;
        }
        public List<int> triangles;
        public List<Triangle> triangleInstances;
        public Mesh mesh;
        public bool autoCreateTriangleObjects;
        public bool selectVerticesWhileAddingTriangles;
        public bool hideTriangles;
        private bool triangleInstancesSelectable;
        private bool triangleInstancesCreated;
        private Dictionary<int, int> triangleInstanceIndexByTriangleIndex;
        private bool triangleInstanceIndexByTriangleIndexValid;
        private List<GameObject> triangleInstancesPool;
        public bool log;
        public bool trianglesChanged;

        public void DeactivateAndMoveToPool(GameObject triangleGameObject) {
            triangleGameObject.SetActive(false);
            triangleGameObject.transform.parent = null;
            triangleInstancesPool.Add(triangleGameObject);
        }

        public GameObject InstantiateOrGetFromPool(Vector3 position, Quaternion rotation) {
            if (triangleInstancesPool.Count > 0) {
                int index = triangleInstancesPool.Count - 1;
                GameObject triangleGameObject = triangleInstancesPool[index];
                triangleInstancesPool.RemoveAt(index);
                triangleGameObject.SetActive(true);
                triangleGameObject.transform.position = position;
                triangleGameObject.transform.rotation = rotation;
                return triangleGameObject;
            } else {
                return GameObject.Instantiate(mesh.trianglePrefab, position, rotation);
            }
        }

        public Triangles(Mesh mesh) {
            this.mesh = mesh;
            triangles = new List<int>();
            triangleInstancesPool = new List<GameObject>();
            triangleInstances = new List<Triangle>();
            triangleInstancesCreated = false;
            selectVerticesWhileAddingTriangles = true;
            triangleInstanceIndexByTriangleIndex = new Dictionary<int, int>();
            triangleInstanceIndexByTriangleIndexValid = false;
            log = true;
            trianglesChanged = false;
        }

        public string TriangleInstancesToString() {
            List<string> result = new List<string>();
            for (int i = 0; i < triangleInstances.Count; i++) {
                Triangle triangle = triangleInstances[i];
                result.Add("([" + i + "].index=" + triangle.index + ")");
            }
            return string.Join(",", result.ToArray());
        }

        public string TrianglesToString(int[] triangles = null) {
            List<string> result = new List<string>();

            if (triangles == null) {
                triangles = mesh.uMesh.triangles;
            }
            for (int i = 0; i + 2 < triangles.Length; i += 3) {
                result.Add("[" + i + "](" + triangles[i] + "," + triangles[i + 1] + "," + triangles[i + 2] + ")");
            }
            return string.Join(",", result.ToArray());
        }

        public Triangle TriangleByIndex(int index) {
            if (triangleInstanceIndexByTriangleIndexValid) {
                if (triangleInstanceIndexByTriangleIndex.ContainsKey(index)) {
                    return triangleInstances[triangleInstanceIndexByTriangleIndex[index]];
                }
            } else {
                foreach (Triangle triangle in triangleInstances) {
                    if (triangle.index == index) return triangle;
                }
            }
            return null;
        }

        public void CreateTriangleInstance(int triangleIndex) {
            Vertex a = mesh.vertices.vertices[triangles[triangleIndex]];
            Vertex b = mesh.vertices.vertices[triangles[triangleIndex + 1]];
            Vertex c = mesh.vertices.vertices[triangles[triangleIndex + 2]];
            Vector3 center = mesh.Center(a.position, b.position, c.position);

            GameObject triangleInstance = InstantiateOrGetFromPool(center, mesh.transform.rotation);
            triangleInstance.transform.parent = mesh.transform;
            triangleInstance.transform.localPosition = center;
            TriangleController triangleController = triangleInstance.GetComponent<TriangleController>();
            triangleController.mesh = mesh;
            triangleController.Initialize();

            //Debug.Log("Creating triangle a=" + a + ",b=" + b + ",c=" + c);
            triangleController.Populate(a, b, c, center);

            Triangle triangle = new Triangle();
            triangle.index = triangleIndex;
            triangle.instance = triangleInstance;
            Material mmMaterial = mesh.materials.MaterialByTriangleIndex(triangleIndex);
            triangle.material = MaterialUtils.MaterialToInstance(mmMaterial, true, true, true);
            //Debug.Log("CreateTriangleInstance fillColor=" + ColorUtility.ToHtmlStringRGBA(meshController.settingsPanelController.GetComponent<SettingsController>().fillColor) + ",mmMaterial.color=" + ColorUtility.ToHtmlStringRGBA(mmMaterial.color) + ",triangle.material.color="+ ColorUtility.ToHtmlStringRGBA(triangle.material.color));

            triangleController.material = triangle.material;
            triangleController.SyncMaterials();

            Selectable selectable = triangleInstance.GetComponent<Selectable>();
            selectable.Initialize();
            selectable.renderMesh = true;
            selectable.renderWireframe = true;
            selectable.renderNormals = true;
            triangleController.SetSelectable(triangleInstancesSelectable);
            triangleController.SetStickySelected(false);
            triangleInstance.SetActive(!hideTriangles);
            triangleInstances.Add(triangle);
            //if (log) Debug.Log("CreateTriangleInstance triangle.index=" + triangle.index+ ", triangleInstances.Count - 1="+ (triangleInstances.Count - 1)+", triangle.instance.ID=" + triangle.instance.GetInstanceID());
            triangleInstanceIndexByTriangleIndex.Add(triangle.index, triangleInstances.Count - 1);
        }

        public void UpdateAllTriangleInstancesSelectable() {
            foreach (Triangle triangle in triangleInstances) {
                GameObject triangleInstance = triangle.instance;
                TriangleController triangleController = triangleInstance.GetComponent<TriangleController>();
                triangleController.SetSelectable(triangleInstancesSelectable);
            }
        }

        public void SetTriangleInstancesSelectable(bool value) {
            if (triangleInstancesSelectable == value) return;
            triangleInstancesSelectable = value;
            UpdateAllTriangleInstancesSelectable();
        }

        public void CreateTriangleInstances() {
            if (mesh.materials.materialsChanged) trianglesChanged = true;
            if (triangleInstancesCreated) return;
            triangleInstanceIndexByTriangleIndex.Clear();
            for (int i = 0; i + 2 < triangles.Count; i += 3) {
                CreateTriangleInstance(i);
            }
            triangleInstancesCreated = true;
        }

        public void SubtractThreeFromTriangleInstanceIndexAboveIndex(int index) {
            foreach (Triangle triangle in triangleInstances) {
                if (triangle.index >= index) {
                    triangle.index -= 3;
                }
            }
            //if (log) Debug.Log("SubtractThreeFromTriangleInstanceIndexAboveIndex index=" + index + ",triangleInstances.Count=" + triangleInstances.Count + ",triangleInstances=" + TriangleInstancesToString());
        }

        public void RebuildTriangleInstanceIndex() {
            //if (log) Debug.Log("RebuildTriangleInstanceIndex triangleInstances.Count=" + triangleInstances.Count + ",triangleInstances=" + TriangleInstancesToString());
            triangleInstanceIndexByTriangleIndex.Clear();
            for (int i = 0; i < triangleInstances.Count; i++) {
                Triangle triangle = triangleInstances[i];
                triangleInstanceIndexByTriangleIndex.Add(triangle.index, i);
            }
            triangleInstanceIndexByTriangleIndexValid = true;
        }

        public void Clear() {
            DeleteTriangleInstances();
            triangles.Clear();
            triangleInstanceIndexByTriangleIndex.Clear();
            trianglesChanged = true;
        }

        public void TruncateToCount(int count) {
            for (int i = triangles.Count - 1; i >= count; i--) {
                if (triangleInstanceIndexByTriangleIndex.ContainsKey(i)) triangleInstanceIndexByTriangleIndex.Remove(i);
                triangles.RemoveAt(i);
            }
            trianglesChanged = true;
        }

        public void DeleteTriangleInstances() {
            foreach (Triangle triangle in triangleInstances) {
                DeactivateAndMoveToPool(triangle.instance);
            }
            triangleInstances.Clear();
            triangleInstanceIndexByTriangleIndex.Clear();
            mesh.selection.ClearSelectedTriangles();
            triangleInstancesCreated = false;
        }

        public void AddQuadByVertices(GameObject vertexInstanceA, GameObject vertexInstanceB, GameObject vertexInstanceC, GameObject vertexInstanceD, int materialIndex = -1) {
            AddTriangleByVertices(vertexInstanceA, vertexInstanceB, vertexInstanceC, materialIndex);
            AddTriangleByVertices(vertexInstanceA, vertexInstanceC, vertexInstanceD, materialIndex);
        }

        public void AddTriangleByVertices(GameObject vertexInstanceA, GameObject vertexInstanceB, GameObject vertexInstanceC, int materialIndex = -1) {
            bool oldValue = selectVerticesWhileAddingTriangles;
            selectVerticesWhileAddingTriangles = false;
            AddTriangleVertex(vertexInstanceA, materialIndex);
            AddTriangleVertex(vertexInstanceB, materialIndex);
            AddTriangleVertex(vertexInstanceC, materialIndex);
            selectVerticesWhileAddingTriangles = oldValue;
        }

        public void AddTriangleVertex(GameObject vertexInstance, int materialIndex = -1) {
            int index = mesh.vertices.IndexOfInstance(vertexInstance);
            AddTriangleVertexIndex(index, vertexInstance, materialIndex);
        }

        public void AddTriangleVertexIndex(int vertexIndex, GameObject vertexInstance, int materialIndex = -1) {
            if (triangles.Count % 3 != 0) {
                int lastVertexIndex = triangles[triangles.Count - 1];
                if (lastVertexIndex == vertexIndex) {
                    // don't allow user to select same vertex twice, unless immediately after completing a triangle.
                    return;
                }
            }

            if ((triangles.Count == 2 || triangles.Count % 3 == 2) &&
                vertexIndex == triangles[triangles.Count - 2]) {
                // don't allow user to select the first vertex same as the last vertex in a triangle (this would make a straight line, not a triangle).
                return;
            }

            triangles.Add(vertexIndex);
            if (selectVerticesWhileAddingTriangles) mesh.selection.SelectVertex(vertexInstance);
            //Debug.Log("MeshController: instance.newTriangles.Count=" + instance.newTriangles.Count + ",instance.newTriangles.Count % 3 = " + instance.newTriangles.Count % 3);
            if (triangles.Count % 3 == 0) {
                if (materialIndex == -1) {
                    mesh.materials.AppendTriangleUsingFillColorMaterial();
                } else {
                    mesh.materials.AppendTriangleUsingMaterialIndex(materialIndex);
                }
                trianglesChanged = true;
                mesh.persistence.changedSinceLastSave = true;
                if (selectVerticesWhileAddingTriangles) mesh.selection.ClearSelectedVertices();
                if (autoCreateTriangleObjects) CreateTriangleInstance(triangles.Count - 3);
            }
            //Debug.Log("MeshController: Added triangle index[" + index + "]=" + vertexIndex);
            //Debug.Log("vertices=" + VerticesToString());
            //Debug.Log("triangles=" + TrianglesToString());
        }



        public bool BuildTriangles() {
            if (!trianglesChanged && !mesh.materials.materialsChanged) return false;
            List<UnityEngine.Material> materials = MaterialUtils.GetMaterials(mesh);
            UnityEngine.Material[] materialArray = MaterialUtils.AssignMaterials(mesh, materials);
            mesh.uMesh.subMeshCount = materials.Count;
            List<Materials.MaterialTriangles> materialTriangles = mesh.materials.GetTriangles();
            for (int i = 0; i < materials.Count; i++) {
                mesh.uMesh.SetTriangles(materialTriangles[i].triangles.ToArray(), i);
            }
            Selectable selectable = mesh.GetComponent<Selectable>();
            mesh.materials.materialsChanged = false;
            if (selectable != null) {
                UnityEngine.Mesh outlineMesh = new UnityEngine.Mesh();
                outlineMesh.vertices = mesh.uMesh.vertices;
                outlineMesh.triangles = mesh.triangles.triangles.ToArray();
                outlineMesh.RecalculateNormals();
                selectable.outlineMesh = outlineMesh;
                selectable.unselectedMaterials = materialArray;
                selectable.UpdateSelectedMaterials();
            } else {
                // BoxSelectionController mesh only
                mesh.uMesh.triangles = triangles.ToArray();
            }
            trianglesChanged = false;
            return true;
        }

        public List<int> SubtractFromEachTriangleVertexIndexAboveVertexIndex(int index, int subtract, List<int> myTriangles) {
            //if (log) Debug.Log("SubtractFromEachTriangleVertexIndexAboveVertexIndex before index=" + index + ",myTriangles=" + TrianglesToString(myTriangles.ToArray()));
            List<int> myTriangles2 = new List<int>();
            foreach (int value in myTriangles) {
                myTriangles2.Add(value > index ? value - subtract : value);
            }
            //if (log) Debug.Log("SubtractFromEachTriangleVertexIndexAboveVertexIndex after index=" + index + ",myTriangles2=" + TrianglesToString(myTriangles2.ToArray()));
            return myTriangles2;
        }

        public void RemoveTrianglesWithVertexIndex(int index) {
            //if (log) Debug.Log("RemoveTrianglesWithVertexIndex index="+index+", triangles=" + TrianglesToString());
            List<int> myTriangles = new List<int>();
            List<int> triangleIndexesToRemove = new List<int>();
            for (int i = 0; i + 2 < triangles.Count; i += 3) {
                int first = triangles[i];
                int second = triangles[i + 1];
                int third = triangles[i + 2];

                if (first == index || second == index || third == index) {
                    // remove by doing nothing
                    //Debug.Log("RemoveTrianglesWithVertexIndex removing triangle with vertex i=" + i + ",indices first=" + first + ",second=" + second + ",third=" + third);
                    //if (log) Debug.Log("RemoveTriangle i=" + i + ",first=" + first + ",second=" + second + ",third=" + third);
                    RemoveTriangleGameObjectByIndex(i);
                    triangleIndexesToRemove.Add(i);
                } else {
                    myTriangles.Add(first);
                    myTriangles.Add(second);
                    myTriangles.Add(third);
                }
            }
            List<int> myTriangles2 = SubtractFromEachTriangleVertexIndexAboveVertexIndex(index, 1, myTriangles);

            for (int i = triangleIndexesToRemove.Count - 1; i >= 0; i--) {
                int triangleIndex = triangleIndexesToRemove[i];
                SubtractThreeFromTriangleInstanceIndexAboveIndex(triangleIndex);
                mesh.materials.RemoveTriangleByIndex(triangleIndex);
            }
            //Debug.Log("RemoveTrianglesWithVertexIndex vertex index=" + index + ", numberTrianglesRemoved=" + numberTrianglesRemoved);
            //Debug.Log("RemoveTrianglesWithVertexIndex myTriangles after=" + TrianglesToString(myTriangles2.ToArray()));
            triangles = myTriangles2;
            trianglesChanged = true;
            mesh.persistence.changedSinceLastSave = true;
            if (triangleIndexesToRemove.Count > 0) {
                // triangles were removed, so rebuild index
                RebuildTriangleInstanceIndex();
            }
            //Debug.Log("RemoveTrianglesWithVertexIndex triangles after=" + TrianglesToString());
        }

        public void RemoveTriangleByVertices(Vertex[] vertices) {
            int a_index = mesh.vertices.IndexOfVertex(vertices[0]);
            int b_index = mesh.vertices.IndexOfVertex(vertices[1]);
            int c_index = mesh.vertices.IndexOfVertex(vertices[2]);

            int[] triangleIndices = new int[] { a_index, b_index, c_index };
            RemoveTriangle(triangleIndices);
        }

        public void RemoveTriangleGameObjectByIndex(int index) {
            //if (log) Debug.Log("RemoveTriangleGameObjectByIndex before index=" + index + ",triangleInstances.Count=" + triangleInstances.Count + ",triangleInstances=" + TriangleInstancesToString());
            Triangle triangle = TriangleByIndex(index);
            DeactivateAndMoveToPool(triangle.instance);
            triangleInstances.Remove(triangle);
            triangleInstanceIndexByTriangleIndex.Remove(index);
            //if (log) Debug.Log("RemoveTriangleGameObjectByIndex after index=" + index + ",triangleInstances.Count=" + triangleInstances.Count + ",triangleInstances=" + TriangleInstancesToString());
            triangleInstanceIndexByTriangleIndexValid = false;
        }

        public void RemoveTriangle(int[] triangle) {
            //if (log) Debug.Log("RemoveTriangle before (" + triangle[0] + "," + triangle[1] + "," + triangle[2] + "), triangles=" + TrianglesToString());
            List<int> myTriangles = new List<int>();
            List<int> triangleIndexesToRemove = new List<int>();
            for (int i = 0; i + 2 < triangles.Count; i += 3) {
                int a = triangles[i];
                int b = triangles[i + 1];
                int c = triangles[i + 2];

                if (triangle[0] == a && triangle[1] == b && triangle[2] == c) {
                    // remove by doing nothing
                    //if (log) Debug.Log("RemoveTriangle i="+i+",a=" + a + ",b=" + b + ",c=" + c);
                    RemoveTriangleGameObjectByIndex(i);
                    triangleIndexesToRemove.Add(i);
                } else {
                    myTriangles.Add(a);
                    myTriangles.Add(b);
                    myTriangles.Add(c);
                }
            }
            for (int i = triangleIndexesToRemove.Count - 1; i >= 0; i--) {
                int triangleIndex = triangleIndexesToRemove[i];
                SubtractThreeFromTriangleInstanceIndexAboveIndex(triangleIndex);
                mesh.materials.RemoveTriangleByIndex(triangleIndex);
            }
            triangles = myTriangles;
            if (triangleIndexesToRemove.Count > 0) {
                // triangles were removed, so rebuild
                trianglesChanged = true;
                mesh.persistence.changedSinceLastSave = true;
                RebuildTriangleInstanceIndex();
            }
            //if (log) Debug.Log("RemoveTriangle after (" + triangle[0] + "," + triangle[1] + "," + triangle[2] + "), triangles=" + TrianglesToString());
        }

        public Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c) {
            Vector3 planeNormal = Vector3.Cross(b - a, c - a).normalized;
            return planeNormal;
        }

        public Vector3 CalculateNormalOfTriangle(Triangle triangle) {
            int triangleIndex = triangle.index;
            int a_index = triangles[triangleIndex];
            int b_index = triangles[triangleIndex + 1];
            int c_index = triangles[triangleIndex + 2];
            Vertex a = mesh.vertices.vertices[a_index];
            Vertex b = mesh.vertices.vertices[b_index];
            Vertex c = mesh.vertices.vertices[c_index];

            return triangle.instance.transform.rotation * CalculateNormal(a.position, b.position, c.position);
        }

        public void FlipNormalsOfSelection() {
            if (mesh.selection.selectedTriangles.Count < 1) return;

            foreach (Triangle triangle in mesh.selection.selectedTriangles) {
                FlipNormalByTriangle(triangle);
            }
        }

        public void FlipNormalByTriangle(Triangle triangle) {
            int triangleIndex = triangle.index;
            int a_index = triangles[triangleIndex];
            int b_index = triangles[triangleIndex + 1];
            int c_index = triangles[triangleIndex + 2];

            triangles[triangleIndex] = c_index;
            triangles[triangleIndex + 1] = b_index;
            triangles[triangleIndex + 2] = a_index;

            trianglesChanged = true;
            mesh.persistence.changedSinceLastSave = true;

            if (triangle.instance != null) {
                Vertex a = mesh.vertices.vertices[a_index];
                Vertex b = mesh.vertices.vertices[b_index];
                Vertex c = mesh.vertices.vertices[c_index];

                Vector3 center = mesh.Center(a.position, b.position, c.position);
                TriangleController triangleController = triangle.instance.GetComponent<TriangleController>();
                triangleController.Populate(c, b, a, center);
            }
        }

        public Triangle FindTriangleByVertices(Vertex[] vertices) {
            int a_index = mesh.vertices.IndexOfVertex(vertices[0]);
            int b_index = mesh.vertices.IndexOfVertex(vertices[1]);
            int c_index = mesh.vertices.IndexOfVertex(vertices[2]);

            if (a_index == -1 || b_index == -1 || c_index == -1) {
                Debug.Log("FindTriangleByVertices couldn't find a vertex");
                return null;
            }

            int triangleIndex = -1;

            for (int i = 0; i < triangles.Count; i += 3) {
                int a_index2 = triangles[i];
                int b_index2 = triangles[i + 1];
                int c_index2 = triangles[i + 2];
                if (a_index2 == a_index && b_index2 == b_index && c_index2 == c_index) {
                    triangleIndex = i;
                    break;
                }
            }

            if (triangleIndex == -1) {
                Debug.Log("FindTriangleByVertices couldn't find triangle");
                return null;
            }

            return TriangleByIndex(triangleIndex);
        }

        public void FlipNormalByVertices(Vertex[] vertices) {
            Triangle triangle = FindTriangleByVertices(vertices);
            if (triangle != null) FlipNormalByTriangle(triangle);
        }

        public void MoveTriangleIndexFromToVertexIndex(int triangleIndex, int triangleOffset, int vertexIndex, Vertex fromVertex, Vertex toVertex) {
            triangles[triangleIndex + triangleOffset] = vertexIndex;

            Triangle triangle = TriangleByIndex(triangleIndex);
            if (triangle == null) {
                Debug.Log("MoveTriangleIndexFromToVertexIndex no triangle at triangleIndex=" + triangleIndex + ",triangleOffset=" + triangleOffset + ",triangleInstancesCreated =" + triangleInstancesCreated);
                return;
            }
            triangle.index = vertexIndex;
            TriangleController triangleController = triangle.instance.GetComponent<TriangleController>();
            Vertex[] vertices = triangleController.GetTriangle();
            Vertex a = vertices[0];
            Vertex b = vertices[1];
            Vertex c = vertices[2];
            if (a.ID == fromVertex.ID) triangleController.SetA(toVertex.position);
            if (b.ID == fromVertex.ID) triangleController.SetB(toVertex.position);
            if (c.ID == fromVertex.ID) triangleController.SetC(toVertex.position);
        }

        public void MoveAllTrianglesFromFirstToSecondVertex(Vertex first, Vertex second) {
            int firstIndex = mesh.vertices.IndexOfVertex(first);
            if (firstIndex == -1) Debug.Log("MoveAllTrianglesFromFirstToSecondVertex firstIndex = -1");
            int secondIndex = mesh.vertices.IndexOfVertex(second);
            if (secondIndex == -1) Debug.Log("MoveAllTrianglesFromFirstToSecondVertex secondIndex = -1");

            List<TriangleToRemove> trianglesToRemove = new List<TriangleToRemove>();

            for (int i = 0; i + 2 < triangles.Count; i += 3) {
                int a = triangles[i];
                int b = triangles[i + 1];
                int c = triangles[i + 2];

                if ((a == firstIndex || b == firstIndex || c == firstIndex) && (a == secondIndex || b == secondIndex || c == secondIndex)) {
                    // triangle has both vertices, so it should be removed.
                    TriangleToRemove triangleToRemove = new TriangleToRemove();
                    triangleToRemove.triangle = new int[] { a, b, c };
                    trianglesToRemove.Add(triangleToRemove);
                    //Debug.Log("MoveAllTrianglesFromFirstToSecondVertex triangleToRemove a=" + a + ",b=" + b + ",c=" + c);
                } else {
                    if (a == firstIndex) MoveTriangleIndexFromToVertexIndex(i, 0, secondIndex, first, second);
                    if (b == firstIndex) MoveTriangleIndexFromToVertexIndex(i, 1, secondIndex, first, second);
                    if (c == firstIndex) MoveTriangleIndexFromToVertexIndex(i, 2, secondIndex, first, second);
                }
            }
            foreach (TriangleToRemove triangleToRemove in trianglesToRemove) {
                RemoveTriangle(triangleToRemove.triangle);
            }
        }
    }
}