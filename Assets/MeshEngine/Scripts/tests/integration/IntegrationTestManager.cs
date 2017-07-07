#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using MeshEngine.Controller;
using CreateThis.VR.UI.Interact;

namespace MeshEngine {
    public class IntegrationTestManager {
        public Mesh mesh;
        public class IntegrationTestFailure : System.Exception {
        }

        public IntegrationTestManager() {
            mesh = Meshes.GetSelectedMesh();
            Clear();
        }

        public void Clear() {
            mesh.uMesh.Clear();
            mesh.vertices.Clear();
            mesh.triangles.Clear();
            mesh.materials.Clear();
            Assert(mesh.vertices.vertices.Count == 0);
            Assert(mesh.triangles.triangles.Count == 0);
            Assert(mesh.triangles.triangleInstances.Count == 0);
        }

        public void Assert(bool value) {
            if (!value) throw (new IntegrationTestFailure());
        }

        public void CreateBox() {
            GameObject mockController = new GameObject();
            Quaternion rotation = Quaternion.identity;
            Quaternion secondRotation = Quaternion.Euler(new Vector3(0, 45, 180));
            Vector3 offset = new Vector3(0, 2, 0);
            Vector3 firstPoint = new Vector3(0, 0, 0) + offset;
            Vector3 secondPoint = new Vector3(0.25f, 0, 0.25f) + offset;
            Vector3 thirdPoint = new Vector3(0.25f, 0.25f, 0.25f) + offset;
            Mode.SetMode(ModeType.PrimitiveBox);
            GameObject primitiveBoxPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/MeshEngine/Prefab/PrimitiveBoxPrefab.prefab", typeof(GameObject));
            GameObject primitiveBoxInstance = GameObject.Instantiate(primitiveBoxPrefab, firstPoint, rotation);
            primitiveBoxInstance.transform.parent = mesh.gameObject.transform;
            primitiveBoxInstance.GetComponent<PrimitiveBoxController>().mesh = mesh;
            Triggerable triggerable = primitiveBoxInstance.GetComponent<Triggerable>();

            mockController.transform.position = firstPoint;
            triggerable.OnTriggerDown(mockController.transform, 1);

            mockController.transform.position = secondPoint;
            mockController.transform.rotation = secondRotation;
            triggerable.OnTriggerDown(mockController.transform, 1);

            mockController.transform.position = thirdPoint;
            triggerable.OnTriggerDown(mockController.transform, 1);
        }

        public void DeleteTriangle() {
            int count = mesh.vertices.vertices.Count;
            Vertex d = mesh.vertices.vertices[count - 1];
            Vertex c = mesh.vertices.vertices[count - 2];
            Vertex b = mesh.vertices.vertices[count - 3];
            Vertex a = mesh.vertices.vertices[count - 4];

            Vertex[] triangle = new Vertex[] { c, d, a };
            mesh.triangles.RemoveTriangleByVertices(triangle);
        }

        public Triangle GetLastTriangle() {
            int count = mesh.vertices.vertices.Count;
            Vertex d = mesh.vertices.vertices[count - 1];
            Vertex c = mesh.vertices.vertices[count - 2];
            Vertex b = mesh.vertices.vertices[count - 3];
            Vertex a = mesh.vertices.vertices[count - 4];

            Vertex[] triangleVertices = new Vertex[] { c, d, a };
            return mesh.triangles.FindTriangleByVertices(triangleVertices);
        }

        public Color RandomFillColor() {
            Color color = Random.ColorHSV();
            for (int i = 0; i < 10; i++) {
                Color fillColor = Settings.fillColor;
                if (!SameColor(color, fillColor)) return color;
            }
            Assert(false); // could not obtain a random color that was not the fill color. what are the odds?
            return color;
        }

        public void FillTriangle(Triangle triangle, Color color) {
            Color oldFillColor = Settings.fillColor;
            Settings.fillColor = color;

            mesh.materials.FillTriangle(triangle);
            Settings.fillColor = oldFillColor;
        }

        public Color GetColorOfTriangle(Triangle triangle) {
            return mesh.materials.GetMaterialByTriangleIndex(triangle.index).color;
        }

        public bool SameColor(Color a, Color b) {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }

        public void SelectLastQuad() {
            mesh.selection.Clear();
            int count = mesh.vertices.vertices.Count;
            Vertex d = mesh.vertices.vertices[count - 1];
            Vertex c = mesh.vertices.vertices[count - 2];
            Vertex b = mesh.vertices.vertices[count - 3];
            Vertex a = mesh.vertices.vertices[count - 4];

            mesh.selection.SelectVertex(a.instance);
            mesh.selection.SelectVertex(b.instance);
            mesh.selection.SelectVertex(c.instance);
            mesh.selection.SelectVertex(d.instance);
        }

        public Triangle FindTriangle() {
            int count = mesh.vertices.vertices.Count;
            Vertex d = mesh.vertices.vertices[count - 1];
            Vertex c = mesh.vertices.vertices[count - 2];
            Vertex b = mesh.vertices.vertices[count - 3];
            Vertex a = mesh.vertices.vertices[count - 4];

            Vertex[] triangle = new Vertex[] { d, b, a };
            return mesh.triangles.FindTriangleByVertices(triangle);
        }

        public Triangle FindTriangleWithNormalsFlipped() {
            int count = mesh.vertices.vertices.Count;
            Vertex d = mesh.vertices.vertices[count - 1];
            Vertex c = mesh.vertices.vertices[count - 2];
            Vertex b = mesh.vertices.vertices[count - 3];
            Vertex a = mesh.vertices.vertices[count - 4];

            Vertex[] triangle = new Vertex[] { a, b, d };
            return mesh.triangles.FindTriangleByVertices(triangle);
        }

        public GameObject CreateVertex(Vector3 position) {
            return mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
        }

        public void Copy() {
            mesh.copy.CopySelection();
        }

        public void Paste() {
            mesh.copy.Paste();
        }

        public void Extrude() {
            mesh.extrusion.ExtrudeSelected(true);
        }

        public void FlipNormals() {
            mesh.triangles.FlipNormalsOfSelection();
        }

        public void DeleteSelection() {
            mesh.selection.DeleteSelected();
        }

        public void SelectAll() {
            foreach (Vertex vertex in mesh.vertices.vertices) {
                mesh.selection.SelectVertex(vertex.instance);
            }
        }
    }
}
#endif