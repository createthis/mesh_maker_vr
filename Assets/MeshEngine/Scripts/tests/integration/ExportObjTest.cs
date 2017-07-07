#if UNITY_EDITOR
using UnityEngine;
using System.IO;

namespace MeshEngine {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class ExportObjTest : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            Mesh mesh = testManager.mesh;

            testManager.CreateBox();

            mesh.vertices.BuildVertices();
            mesh.triangles.BuildTriangles();

            ExportObj exportObj = new ExportObj();
            exportObj.vertices = mesh.uMesh.vertices;
            exportObj.triangles = mesh.triangles.triangles.ToArray();
            exportObj.materials = mesh.materials.GetMaterials();
            exportObj.materialNames = mesh.materials.MaterialNames();
            exportObj.Save("Temp/test.obj");

            ImportObj importObj = new ImportObj();
            importObj.Load("Temp/test.obj");
            testManager.Assert(importObj.vertices.Count == 8);
            testManager.Assert(importObj.triangles.Count == 12 * 3);
            testManager.Assert(importObj.materialNames.Count == 12);
            testManager.Assert(importObj.materials.Count == 1);

            testManager.Assert(File.Exists("Temp/test.obj"));
            File.Delete("Temp/test.obj");
            testManager.Assert(File.Exists("Temp/test.mtl"));
            File.Delete("Temp/test.mtl");

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif
