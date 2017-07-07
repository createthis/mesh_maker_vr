using UnityEngine;
using MeshEngine;
using MeshEngine.Controller;

namespace MMVR.Interact {
    public class PrimitiveBoxDefaultTriggerable : PrimitiveBaseDefaultTriggerable {
        protected override GameObject GetPrimitivePrefab() {
            return Meshes.primitiveBoxPrefab;
        }

        protected override void BuildPrimitiveController() {
            BuildPrimitiveController<PrimitiveBoxController>();
        }
    }
}