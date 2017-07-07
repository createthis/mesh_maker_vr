#if UNITY_EDITOR
using UnityEngine;

namespace MeshEngine.Test {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class ExtrudeFaceWithDeletedTriangleTest : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            Mesh mesh = testManager.mesh;
            testManager.Assert(mesh.materials.GetMaterials().Count == 0);
            testManager.Assert(mesh.vertices.vertices.Count == 0);
            testManager.Assert(mesh.triangles.triangles.Count == 0);

            testManager.CreateBox();

            testManager.Assert(mesh.vertices.vertices.Count == 8);
            testManager.Assert(mesh.triangles.triangles.Count == 12 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 12);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 12);

            testManager.DeleteTriangle();

            testManager.Assert(mesh.vertices.vertices.Count == 8);
            testManager.Assert(mesh.triangles.triangles.Count == 11 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 11);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 11);

            testManager.SelectLastQuad();

            testManager.Assert(mesh.selection.selectedVertices.Count == 4);
            testManager.Assert(mesh.selection.selectedTriangles.Count == 1);

            testManager.Extrude();

            testManager.Assert(mesh.selection.selectedVertices.Count == 4);
            testManager.Assert(mesh.selection.selectedTriangles.Count == 1);

            testManager.DeleteSelection();

            testManager.Assert(mesh.vertices.vertices.Count == 8);
            testManager.Assert(mesh.triangles.triangles.Count == 11 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 11);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 11);

            testManager.SelectAll();
            testManager.DeleteSelection();

            testManager.Assert(mesh.vertices.vertices.Count == 0);
            testManager.Assert(mesh.triangles.triangles.Count == 0);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 0);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 0);

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif