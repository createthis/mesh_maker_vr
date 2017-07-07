using System.Collections.Generic;

namespace MeshEngine {
    public class Edges {
        public List<Edge> edges;
        public Mesh mesh;

        public Edges(Mesh mesh) {
            this.mesh = mesh;
            edges = new List<Edge>();
        }

        public bool[] EdgeInTriangle(bool[] line_exists, Edge edge, Vertex a, Vertex b, Vertex c) {
            Vertices vm = mesh.vertices;
            if (vm.SameVertex(edge.a, a)) {
                if (vm.SameVertex(edge.b, b)) {
                    line_exists[0] = true;
                    edge.interior = true;
                } else if (vm.SameVertex(edge.b, c)) {
                    line_exists[2] = true;
                    edge.interior = true;
                }
            } else if (vm.SameVertex(edge.a, b)) {
                if (vm.SameVertex(edge.b, a)) {
                    line_exists[0] = true;
                    edge.interior = true;
                } else if (vm.SameVertex(edge.b, c)) {
                    line_exists[1] = true;
                    edge.interior = true;
                }
            } else if (vm.SameVertex(edge.a, c)) {
                if (vm.SameVertex(edge.b, a)) {
                    line_exists[2] = true;
                    edge.interior = true;
                } else if (vm.SameVertex(edge.b, b)) {
                    line_exists[1] = true;
                    edge.interior = true;
                }
            }
            return line_exists;
        }

        public void FindEdgesInSelection() {
            edges.Clear();

            List<int> triangles = mesh.triangles.triangles;
            List<Triangle> selectedTriangles = mesh.selection.selectedTriangles;

            for (int i = 0; i < selectedTriangles.Count; i++) {
                Triangle triangle = selectedTriangles[i];
                int triangleIndex = triangle.index;

                Vertex a = mesh.vertices.vertices[triangles[triangleIndex]];
                Vertex b = mesh.vertices.vertices[triangles[triangleIndex + 1]];
                Vertex c = mesh.vertices.vertices[triangles[triangleIndex + 2]];
                int materialIndex = mesh.materials.MaterialIndexByTriangleIndex(triangleIndex);

                /* Make the Lines:
                    every line may border two triangles
                    so to not render every line twice
                    compare new lines to existing */
                bool[] line_exists = new bool[] { false, false, false };
                for (int j = 0; j < edges.Count; j++) {
                    Edge edge = edges[j];
                    line_exists = EdgeInTriangle(line_exists, edge, a, b, c);
                }
                // only add lines if they dont yet exist
                if (!line_exists[0]) {
                    Edge edge = new Edge();
                    edge.a = a;
                    edge.b = b;
                    edge.interior = false;
                    edge.materialIndex = materialIndex;
                    edges.Add(edge);
                } else {
                    // do not add; edge already exists
                }
                if (!line_exists[1]) {
                    Edge edge = new Edge();
                    edge.a = b;
                    edge.b = c;
                    edge.interior = false;
                    edge.materialIndex = materialIndex;
                    edges.Add(edge);
                } else {
                    // do not add; edge already exists
                }
                if (!line_exists[2]) {
                    Edge edge = new Edge();
                    edge.a = c;
                    edge.b = a;
                    edge.interior = false;
                    edge.materialIndex = materialIndex;
                    edges.Add(edge);
                } else {
                    // do not add; edge already exists
                }
            }
        }

        public List<Edge> ExteriorEdges() {
            List<Edge> exteriorEdges = new List<Edge>();
            foreach (Edge edge in edges) {
                if (edge.interior == false) exteriorEdges.Add(edge);
            }
            return exteriorEdges;
        }
    }
}