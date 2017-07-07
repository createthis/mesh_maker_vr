#if UNITY_EDITOR
using UnityEngine;

namespace MeshEngine.Test {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class SharedEdgeVertexMergeTest : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            Mesh mesh = testManager.mesh;
            Vector3 offset = new Vector3(0, 2, 0);

            Vector3 topPosition = new Vector3(0, 0, 0.25f);
            GameObject topVertex = testManager.CreateVertex(topPosition + offset);

            Vector3 middleLeftPosition = new Vector3(-0.25f, 0, 0);
            GameObject middleLeftVertex = testManager.CreateVertex(middleLeftPosition + offset);

            Vector3 middlePosition = Vector3.zero;
            GameObject middleVertex = testManager.CreateVertex(middlePosition + offset);

            Vector3 bottomPosition = new Vector3(0, 0, -0.25f);
            GameObject bottomVertex = testManager.CreateVertex(bottomPosition + offset);

            mesh.triangles.AddTriangleByVertices(middleLeftVertex, topVertex, middleVertex);
            mesh.triangles.AddTriangleByVertices(bottomVertex, middleLeftVertex, middleVertex);

            testManager.Assert(mesh.vertices.vertices.Count == 4);
            testManager.Assert(mesh.triangles.triangles.Count == 2 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 2);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 2);

            mesh.selection.SelectVertex(middleLeftVertex);
            mesh.selection.SelectVertex(middleVertex);

            mesh.vertices.MergeSelected();

            testManager.Assert(mesh.vertices.vertices.Count == 3);
            testManager.Assert(mesh.triangles.triangles.Count == 0);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 0);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 0);

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif