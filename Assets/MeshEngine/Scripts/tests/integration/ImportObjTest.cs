#if UNITY_EDITOR
using UnityEngine;

namespace MeshEngine.Test {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class ImportObjTest : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            //MeshController meshController = testManager.meshController;

            ImportObj importObj = new ImportObj();
            importObj.Load("test_files/vive_teleporter.obj");
            testManager.Assert(importObj.vertices.Count == 101);
            testManager.Assert(importObj.uvs.Count == 101);
            testManager.Assert(importObj.triangles.Count == 336);
            testManager.Assert(importObj.materialNames.Count == 112);
            testManager.Assert(importObj.materials.Count == 4);

            importObj.Clear();
            importObj.Load("test_files/pikachu.obj");
            testManager.Assert(importObj.vertices.Count == 208);
            testManager.Assert(importObj.uvs.Count == 0);
            testManager.Assert(importObj.triangles.Count == 412 * 3);
            testManager.Assert(importObj.materialNames.Count == 412);
            testManager.Assert(importObj.materials.Count == 2);

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif
