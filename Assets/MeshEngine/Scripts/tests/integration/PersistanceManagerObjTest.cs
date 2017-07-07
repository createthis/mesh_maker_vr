#if UNITY_EDITOR
using UnityEngine;
using System.IO;

namespace MeshEngine.Test {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class PersistanceManagerObjTest : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            Mesh mesh = testManager.mesh;

            // ------------------
            // OBJ with UVs
            // ------------------
            mesh.persistence.Load("test_files/vive_teleporter");
            testManager.Assert(mesh.vertices.vertices.Count == 101);
            testManager.Assert(mesh.triangles.triangles.Count == 336);
            testManager.Assert(mesh.materials.MaterialNames().Count == 112);
            testManager.Assert(mesh.materials.GetMaterials().Count == 4);

            mesh.vertices.BuildVertices();
            mesh.triangles.BuildTriangles();

            testManager.Assert(mesh.uMesh.uv.Length == 101);

            mesh.persistence.Save("Temp/test.obj");

            ImportObj importObj = new ImportObj();
            importObj.Load("Temp/test.obj");
            testManager.Assert(importObj.vertices.Count == 101);
            testManager.Assert(importObj.uvs.Count == 101);
            testManager.Assert(importObj.triangles.Count == 336);
            testManager.Assert(importObj.materialNames.Count == 112);
            testManager.Assert(importObj.materials.Count == 4);

            testManager.Assert(File.Exists("Temp/test.obj"));
            File.Delete("Temp/test.obj");
            testManager.Assert(File.Exists("Temp/test.mtl"));
            File.Delete("Temp/test.mtl");

            testManager.Clear();
            importObj.Clear();

            // ------------------
            // OBJ without UVs
            // ------------------
            mesh.persistence.Load("test_files/pikachu");
            testManager.Assert(mesh.vertices.vertices.Count == 208);
            testManager.Assert(mesh.triangles.triangles.Count == 412 * 3);
            testManager.Assert(mesh.materials.MaterialNames().Count == 412);
            testManager.Assert(mesh.materials.GetMaterials().Count == 2);

            mesh.vertices.BuildVertices();
            mesh.triangles.BuildTriangles();

            testManager.Assert(mesh.uMesh.uv.Length == 208);

            mesh.persistence.Save("Temp/test.obj");

            importObj.Load("Temp/test.obj");
            testManager.Assert(importObj.vertices.Count == 208);
            testManager.Assert(importObj.uvs.Count == 208);
            testManager.Assert(importObj.triangles.Count == 412 * 3);
            testManager.Assert(importObj.materialNames.Count == 412);
            testManager.Assert(importObj.materials.Count == 2);

            testManager.Assert(File.Exists("Temp/test.obj"));
            File.Delete("Temp/test.obj");
            testManager.Assert(File.Exists("Temp/test.mtl"));
            File.Delete("Temp/test.mtl");

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif