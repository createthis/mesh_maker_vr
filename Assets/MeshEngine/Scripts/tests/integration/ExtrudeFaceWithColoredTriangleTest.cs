#if UNITY_EDITOR
using UnityEngine;

namespace MeshEngine.Test {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class ExtrudeFaceWithColoredTriangleTest : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            Mesh mesh = testManager.mesh;
            testManager.Assert(mesh.materials.GetMaterials().Count == 0);
            testManager.CreateBox();

            testManager.Assert(mesh.vertices.vertices.Count == 8);
            testManager.Assert(mesh.triangles.triangles.Count == 12 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 12);
            testManager.Assert(mesh.materials.GetMaterials().Count == 1);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 12);

            Color testColor = testManager.RandomFillColor();
            Color fillColor = Settings.fillColor;

            Triangle triangle = testManager.GetLastTriangle();
            testManager.FillTriangle(triangle, testColor);

            Triangle filledTriangle = mesh.triangles.triangleInstances[3];
            Color filledTriangleColor = testManager.GetColorOfTriangle(filledTriangle);
            testManager.Assert(testManager.SameColor(filledTriangleColor, testColor));

            testManager.Assert(mesh.vertices.vertices.Count == 8);
            testManager.Assert(mesh.triangles.triangles.Count == 12 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 12);
            testManager.Assert(mesh.materials.GetMaterials().Count == 2);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 12);

            testManager.SelectLastQuad();

            testManager.Assert(mesh.selection.selectedVertices.Count == 4);
            testManager.Assert(mesh.selection.selectedTriangles.Count == 2);

            Color newFillColor = testManager.RandomFillColor();
            Settings.fillColor = newFillColor;

            testManager.Extrude();

            Settings.fillColor = fillColor;

            testManager.Assert(mesh.triangles.triangles.Count == 22 * 3);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 22);

            testManager.Assert(mesh.selection.selectedVertices.Count == 4);
            testManager.Assert(mesh.selection.selectedTriangles.Count == 2);
            testManager.Assert(mesh.materials.GetMaterials().Count == 2);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 22);

            mesh.selection.Clear();

            Triangle extrudedTriangle = mesh.triangles.triangleInstances[13];
            Color extrudedTriangleColor = testManager.GetColorOfTriangle(extrudedTriangle);
            testManager.Assert(testManager.SameColor(extrudedTriangleColor, testColor));

            Triangle lastTriangle = mesh.triangles.triangleInstances[21];
            Color lastTriangleColor = testManager.GetColorOfTriangle(lastTriangle);
            testManager.Assert(testManager.SameColor(lastTriangleColor, testColor));

            Triangle secondExtrudedTriangle = mesh.triangles.triangleInstances[14];
            Color secondExtrudedTriangleColor = testManager.GetColorOfTriangle(secondExtrudedTriangle);
            testManager.Assert(testManager.SameColor(secondExtrudedTriangleColor, fillColor));

            testManager.SelectAll();
            testManager.DeleteSelection();

            testManager.Assert(mesh.vertices.vertices.Count == 0);
            testManager.Assert(mesh.triangles.triangles.Count == 0);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 0);
            testManager.Assert(mesh.materials.GetMaterials().Count == 2);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 0);

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif
