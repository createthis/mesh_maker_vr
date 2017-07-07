using UnityEngine;
using System.Collections.Generic;

namespace MeshEngine {
    public class Extrusion {
        public Mesh mesh;

        public Extrusion(Mesh mesh) {
            this.mesh = mesh;
        }

        public bool SelectionHasTriangles() {
            return mesh.selection.selectedTriangles.Count > 0;
        }

        public void WallCopyPasteEdges(List<Edge> copyExteriorEdges, Copy copy) {
            for (int i = 0; i < copyExteriorEdges.Count; i++) {
                Edge edge = copyExteriorEdges[i];
                CopyVertex copyVertexA = copy.CopyVertexOfVertex(edge.a);
                CopyVertex copyVertexB = copy.CopyVertexOfVertex(edge.b);
                PasteVertex pasteVertexA = copy.PasteVertexOfCopyVertex(copyVertexA);
                PasteVertex pasteVertexB = copy.PasteVertexOfCopyVertex(copyVertexB);

                Vertex a = mesh.vertices.vertices[copyVertexA.indexFromMesh];
                Vertex b = mesh.vertices.vertices[pasteVertexA.indexInMesh];
                Vertex c = mesh.vertices.vertices[pasteVertexB.indexInMesh];
                Vertex d = mesh.vertices.vertices[copyVertexB.indexFromMesh];

                mesh.triangles.AddQuadByVertices(d.instance, c.instance, b.instance, a.instance, edge.materialIndex);
            }
        }

        public Vector3 AverageVector(List<Vector3> vectorList) {
            Vector3 acumulateVector = Vector3.zero;
            foreach (Vector3 v in vectorList) {
                acumulateVector += v;
            }
            return acumulateVector / vectorList.Count;
        }

        public void ExtrudeSelected(bool offset = false) {
            if (!SelectionHasTriangles()) return;

            Edges edges = new Edges(mesh);
            edges.FindEdgesInSelection();
            List<Edge> copyExteriorEdges = edges.ExteriorEdges();

            // Avoid nuking the user's copy buffer by creating a local buffer.
            Copy copy = new Copy(mesh);
            copy.CopySelection();

            mesh.selection.Clear();
            mesh.triangles.autoCreateTriangleObjects = true;
            copy.Paste(true);

            if (offset) {
                Vector3 averageNormal = Vector3.zero;
                List<Vector3> selectedNormals = new List<Vector3>();

                foreach (Triangle triangle in mesh.selection.selectedTriangles) {
                    Vector3 normal = mesh.triangles.CalculateNormalOfTriangle(triangle).normalized;
                    selectedNormals.Add(normal);
                }

                Vector3 finalNormal = AverageVector(selectedNormals);
                Vector3 direction = finalNormal.normalized * 0.025f;
                mesh.selection.SelectionTransformDirection(direction);
            }

            bool oldSelectVerticesWhileAddingTriangles = mesh.triangles.selectVerticesWhileAddingTriangles;
            mesh.triangles.selectVerticesWhileAddingTriangles = false;
            WallCopyPasteEdges(copyExteriorEdges, copy);
            mesh.triangles.selectVerticesWhileAddingTriangles = oldSelectVerticesWhileAddingTriangles;
        }
    }
}