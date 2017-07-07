#if UNITY_EDITOR
using UnityEngine;

namespace MeshEngine.Test {
    [IntegrationTest.DynamicTest("mesh-maker-scene")]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.Timeout(5)]

    public class MaterialManagerSameNameDifferentColor : MonoBehaviour {
        private void Start() {
            IntegrationTestManager testManager = new IntegrationTestManager();
            Mesh mesh = testManager.mesh;

            Color color1 = new Color(0.9019608f, 0, 1, 0);
            Color color2 = new Color(0.9019607f, 0, 1, 0);

            testManager.Assert(mesh.materials.FindOrCreateMaterialIndexByColor(color1) == 0);
            testManager.Assert(mesh.materials.FindOrCreateMaterialIndexByColor(color2) == 1);

            IntegrationTest.Pass(gameObject);
        }
    }
}
#endif