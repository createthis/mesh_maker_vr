#if UNITY_EDITOR
using UnityEngine;

namespace MeshEngine.Test {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class VertexMergeTest : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            Mesh mesh = testManager.mesh;
            Vector3 offset = new Vector3(0, 2, 0);

            Vector3 topPosition = new Vector3(0, 0, 0.25f);
            GameObject topVertex = testManager.CreateVertex(topPosition + offset);

            Vector3 topLeftPosition = new Vector3(-0.25f, 0, 0.15f);
            GameObject topLeftVertex = testManager.CreateVertex(topLeftPosition + offset);

            Vector3 middlePosition = Vector3.zero;
            GameObject middleVertex = testManager.CreateVertex(middlePosition + offset);

            Vector3 bottomPosition = new Vector3(0, 0, -0.25f);
            GameObject bottomVertex = testManager.CreateVertex(bottomPosition + offset);

            Vector3 bottomLeftPosition = new Vector3(-0.25f, 0, -0.15f);
            GameObject bottomLeftVertex = testManager.CreateVertex(bottomLeftPosition + offset);

            mesh.triangles.AddTriangleByVertices(bottomLeftVertex, middleVertex, bottomVertex);
            mesh.triangles.AddTriangleByVertices(topLeftVertex, topVertex, middleVertex);

            testManager.Assert(mesh.vertices.vertices.Count == 5);
            testManager.Assert(mesh.triangles.triangles.Count == 2 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 2);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 2);

            mesh.selection.SelectVertex(bottomLeftVertex);
            mesh.selection.SelectVertex(topLeftVertex);

            mesh.vertices.MergeSelected();

            testManager.Assert(mesh.vertices.vertices.Count == 4);
            testManager.Assert(mesh.triangles.triangles.Count == 2 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 2);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 2);

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif