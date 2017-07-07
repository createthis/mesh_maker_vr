using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MeshEngine {
    public class Copy {
        public List<CopyVertex> vertices;
        public List<int> triangles;
        private List<int> triangleMaterials; // index points to triangle in triangles * 3, value is index in MaterialManager.materials. length = TrianglesManager.triangles.Count / 3.
        public List<PasteVertex> pasteVertices;
        public Mesh mesh;

        public Copy(Mesh mesh) {
            this.mesh = mesh;
            vertices = new List<CopyVertex>();
            triangles = new List<int>();
            triangleMaterials = new List<int>();
            pasteVertices = new List<PasteVertex>();
        }

        public Vector3[] GetVerticesArray() {
            return vertices.Select(x => x.position).ToArray();
        }

        public void Clear() {
            vertices.Clear();
            triangles.Clear();
            triangleMaterials.Clear();
            pasteVertices.Clear();
        }

        public void CopySelection(bool clearSelection = true) {
            Clear();
            CopySelectionVertices();
            CopySelectionTriangles();
            //Debug.Log("CopySelection vertices=" + meshController.verticesManager.VerticesToString(GetVerticesArray()));
            //Debug.Log("CopySelection triangles=" + meshController.trianglesManager.TrianglesToString(triangles.ToArray()));
            if (clearSelection) mesh.selection.Clear();
        }

        public void SelectVerticesFromCopy() {
            foreach (CopyVertex selectedVertex in vertices) {
                Vertex vertex = mesh.vertices.vertices[selectedVertex.indexFromMesh];
                mesh.selection.SelectVertex(vertex.instance);
            }
        }

        public void CopySelectionVertices() {
            vertices = new List<CopyVertex>();
            foreach (SelectedVertex selectedVertex in mesh.selection.selectedVertices) {
                CopyVertex copyVertex = new CopyVertex();
                copyVertex.position = mesh.vertices.vertices[selectedVertex.index].position;
                copyVertex.indexFromMesh = selectedVertex.index;
                vertices.Add(copyVertex);
            }
        }

        public int CopyVertexIndexOfMeshIndex(int meshVertexIndex) {
            for (int i = 0; i < vertices.Count; i++) {
                if (vertices[i].indexFromMesh == meshVertexIndex) return i;
            }
            return -1;
        }

        public int CopyVertexIndexOfMeshTriangleIndex(int triangleIndex) {
            return CopyVertexIndexOfMeshIndex(mesh.triangles.triangles[triangleIndex]);
        }

        public void CopySelectionTriangles() {
            //Debug.Log("triangles=" + meshController.trianglesManager.TrianglesToString(meshController.trianglesManager.triangles.ToArray()));
            //Debug.Log("CopySelectionTriangles selectedTriangles=" + meshController.trianglesManager.TrianglesToString(meshController.selectionManager.GetTrianglesArray()));
            triangles = new List<int>();
            foreach (Triangle triangle in mesh.selection.selectedTriangles) {
                int triangleIndex = triangle.index;

                triangleMaterials.Add(mesh.materials.MaterialIndexByTriangleIndex(triangleIndex));
                triangles.Add(CopyVertexIndexOfMeshTriangleIndex(triangleIndex));
                triangles.Add(CopyVertexIndexOfMeshTriangleIndex(triangleIndex + 1));
                triangles.Add(CopyVertexIndexOfMeshTriangleIndex(triangleIndex + 2));
            }
        }

        public void Paste(bool noOffset = false) {
            pasteVertices.Clear();
            PasteVertices(noOffset);
            PasteTriangles();
            SelectVertices();
        }

        public void SelectVertices() {
            for (int i = 0; i < pasteVertices.Count; i++) {
                mesh.selection.SelectVertex(pasteVertices[i].instance);
            }
        }

        public void PasteVertices(bool noOffset = false) {
            foreach (CopyVertex copyVertex in vertices) {
                PasteVertex pasteVertex = new PasteVertex();
                pasteVertex.copyVertex = copyVertex;

                Vector3 pastePosition = copyVertex.position;
                if (!noOffset) {
                    pastePosition = pastePosition + mesh.transform.rotation * new Vector3(0, 0, 0.01f);
                }

                GameObject vertexInstance = mesh.vertices.CreateVertexInstanceByLocalPosition(pastePosition);
                int indexInMesh = mesh.vertices.AddVertexWithInstance(pastePosition, vertexInstance, false);

                pasteVertex.instance = vertexInstance;
                pasteVertex.indexInMesh = indexInMesh;
                pasteVertices.Add(pasteVertex);
            }
            //meshController.verticesManager.BuildVertices();
        }

        public CopyVertex CopyVertexOfVertex(Vertex vertex) {
            foreach (CopyVertex copyVertex in vertices) {
                if (mesh.vertices.IndexOfVertex(vertex) == copyVertex.indexFromMesh) return copyVertex;
            }
            return null;
        }

        public PasteVertex PasteVertexOfCopyVertex(CopyVertex copyVertex) {
            foreach (PasteVertex pasteVertex in pasteVertices) {
                if (pasteVertex.copyVertex.ID == copyVertex.ID) return pasteVertex;
            }
            return null;
        }

        public PasteVertex PasteVertexOfTriangleIndex(int triangleIndex) {
            CopyVertex copyVertex = vertices[triangleIndex];
            return PasteVertexOfCopyVertex(copyVertex);
        }

        public void PasteTriangles() {
            for (int i = 0; i < triangles.Count; i++) {
                int triangleIndex = triangles[i];
                int triangleMaterialIndex = i / 3;
                PasteVertex pasteVertex = PasteVertexOfTriangleIndex(triangleIndex);
                mesh.triangles.AddTriangleVertexIndex(pasteVertex.indexInMesh, pasteVertex.instance, triangleMaterials[triangleMaterialIndex]);
            }
            mesh.triangles.trianglesChanged = true;
        }
    }
}