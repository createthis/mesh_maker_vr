using UnityEngine;
using MeshEngine;
using MeshEngine.Controller;

namespace MMVR.Interact {
    public class PrimitiveSphereDefaultTriggerable : PrimitiveBaseDefaultTriggerable {
        protected override GameObject GetPrimitivePrefab() {
            return Meshes.primitiveSpherePrefab;
        }

        protected override void BuildPrimitiveController() {
            BuildPrimitiveController<PrimitiveSphereController>();
        }
    }
}