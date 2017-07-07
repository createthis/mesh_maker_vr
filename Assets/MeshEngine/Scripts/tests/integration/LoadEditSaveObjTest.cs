#if UNITY_EDITOR
using UnityEngine;
using System.IO;

namespace MeshEngine.Test {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class LoadEditSaveObjTest : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            Mesh mesh = testManager.mesh;

            // OBJ with UVs
            mesh.persistence.Load("test_files/vive_teleporter");
            testManager.Assert(mesh.vertices.vertices.Count == 101);
            testManager.Assert(mesh.triangles.triangles.Count == 336);
            testManager.Assert(mesh.materials.MaterialNames().Count == 112);
            testManager.Assert(mesh.materials.GetMaterials().Count == 4);

            mesh.vertices.BuildVertices();
            mesh.triangles.BuildTriangles();

            testManager.Assert(mesh.uMesh.uv.Length == 101);

            testManager.CreateBox();

            testManager.Assert(mesh.vertices.vertices.Count == 109);
            testManager.Assert(mesh.triangles.triangles.Count == 372);
            testManager.Assert(mesh.triangles.triangleInstances.Count == 124);
            testManager.Assert(mesh.materials.GetMaterials().Count == 5);
            testManager.Assert(mesh.materials.GetTriangleIndexMaterialIndex().Count == 124);

            mesh.persistence.Save("Temp/test.obj");

            ImportObj importObj = new ImportObj();
            importObj.Load("Temp/test.obj");
            testManager.Assert(importObj.vertices.Count == 109);
            testManager.Assert(importObj.uvs.Count == 109);
            testManager.Assert(importObj.triangles.Count == 372);
            testManager.Assert(importObj.materialNames.Count == 124);
            testManager.Assert(importObj.materials.Count == 5);

            testManager.Assert(File.Exists("Temp/test.obj"));
            File.Delete("Temp/test.obj");
            testManager.Assert(File.Exists("Temp/test.mtl"));
            File.Delete("Temp/test.mtl");

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif